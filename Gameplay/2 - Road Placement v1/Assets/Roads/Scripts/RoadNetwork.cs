using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.AI.Navigation;

public class RoadNetwork : MonoBehaviour
{

    public static RoadNetwork current;

    public enum Road_Type
    {
        Closed,     /// Road with no connections in any direction
        End,        /// A road with only one connection
        Straight,   /// A road with two opposite connections
        Turn,       /// A road with two adjacent connections
        Triple_Intersection,    /// Road with three connections
        Quad_Intersection   /// Road with all four connections
    }

    private class Road_Node
    {
        public Vector3Int road_position = Vector3Int.back;

        public Road_Type road_type = Road_Type.Closed;

        public Road_Node north = null;
        public Road_Node south = null;
        public Road_Node east = null;
        public Road_Node west = null;

        public GameObject road_instance = null;

        public Road_Placement_Mode road_placement_mode = Road_Placement_Mode.Permanent;

        /// The constructor does NOT update its nodes and does NOT instantiate a road
        public Road_Node(Vector3Int _road_position, Road_Placement_Mode _road_placement_mode = Road_Placement_Mode.Permanent)
        {
            road_position = _road_position;
            road_placement_mode = _road_placement_mode;
        }

        /// This constructor is for cloning another node
        public Road_Node(Road_Node other_road_node)
        {
            copy_node(other_road_node);
        }

        /// Steal his look
        public void copy_node(Road_Node node_to_copy)
        {
            road_position = node_to_copy.road_position;

            road_type = node_to_copy.road_type;

            north = node_to_copy.north;
            south = node_to_copy.south;
            east = node_to_copy.east;
            west = node_to_copy.west;

            road_instance = node_to_copy.road_instance;

            road_placement_mode = node_to_copy.road_placement_mode;
        }
        
    }

    private Dictionary<Vector3Int, Road_Node> road_graph = new Dictionary<Vector3Int, Road_Node>();

    [Serializable]
    public struct Road_Gameobject
    {
        public Road_Type type;
        public GameObject prefab;
    }

    public Road_Gameobject[] road_gameobjects;
    private Dictionary<Road_Type, GameObject> road_gameobjects_mapped = new Dictionary<Road_Type, GameObject>();
    
    #region Temporal and permanent road placement

    public enum Road_Placement_Mode
    {
        Permanent,  /// If a road is placed, it stays that way until user manually destroys it
        Temporal    /// When a road is constructed, in the beginning of the next frame the road is destroyed
    }

    private Road_Placement_Mode current_road_placement_mode = Road_Placement_Mode.Permanent;

    #endregion

    #region Update road node graph and change gameobjects and its orientation

    private ObjectOrientation.Cardinal_Direction get_cardinal_direction(Road_Node road_node)
    {
        switch (road_node.road_type)
        {
        /// A closed road has no pivot (north for default)
        case Road_Type.Closed:
            return ObjectOrientation.Cardinal_Direction.North;

            //break;

        /// The opposite direction of the road that has a connection is the pivot
        case Road_Type.End:
            if (road_node.north != null)
            {
                return ObjectOrientation.Cardinal_Direction.South;
            }
            else if (road_node.south != null)
            {
                return ObjectOrientation.Cardinal_Direction.North;
            }
            else if (road_node.east != null)
            {
                return ObjectOrientation.Cardinal_Direction.West;
            }
            else    /// road_node.west != null
            {
                return ObjectOrientation.Cardinal_Direction.East;
            }

            //break;

        /// Here the pivot will have a preference for north (north-south road) or east (east-west) road
        case Road_Type.Straight:
            if (road_node.north != null /* && road_node.south != null */)
            {
                return ObjectOrientation.Cardinal_Direction.North;
            }
            else    /// road_node.east != null && road_node.west != null
            {
                return ObjectOrientation.Cardinal_Direction.East;
            }
            
            //break;

        /// Going clockwise, if a turn is north-east, the pivot is north (east-south is east...)
        case Road_Type.Turn:
            if (road_node.north != null && road_node.east != null)
            {
                return ObjectOrientation.Cardinal_Direction.North;
            }
            else if (road_node.east != null && road_node.south != null)
            {
                return ObjectOrientation.Cardinal_Direction.East;
            }
            else if (road_node.south != null && road_node.west != null)
            {
                return ObjectOrientation.Cardinal_Direction.South;
            }
            else    /// road_node.west != null && road_node.north != null
            {
                return ObjectOrientation.Cardinal_Direction.West;
            }

            //break;

        /// For easiness, the direction that is not connected is the pivot (east-north-west road is south)
        case Road_Type.Triple_Intersection:
            if (road_node.north == null)
            {
                return ObjectOrientation.Cardinal_Direction.North;
            }
            else if (road_node.south == null)
            {
                return ObjectOrientation.Cardinal_Direction.South;
            }
            else if (road_node.east == null)
            {
                return ObjectOrientation.Cardinal_Direction.East;
            }
            else    /// road_node.west == null
            {
                return ObjectOrientation.Cardinal_Direction.West;
            }
            
            //break;

        /// No pivot (north for default)
        case Road_Type.Quad_Intersection:
            return ObjectOrientation.Cardinal_Direction.North;
            
            //break;
        }

        /// Unreachable point
        return ObjectOrientation.Cardinal_Direction.North;
    }

    private void update_node_gameobject(Road_Node road_node)
    {
        if (road_node.road_instance != null)
        {
            Destroy(road_node.road_instance);
        }

        Vector3Int road_position = road_node.road_position;

        Vector3 gameobject_position = BuildingSystem.current.MainTilemap.CellToWorld(road_position) + new Vector3(5.0f, 0.0f, 5.0f);

        GameObject road_gameobject = road_gameobjects_mapped[road_node.road_type];
        GameObject instance = Instantiate(road_gameobject, gameobject_position, Quaternion.identity);
        instance.transform.position += road_gameobject.transform.position;
        //instance.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        instance.transform.parent = this.transform;

        road_node.road_instance = instance;

        /// Rotate the road to its correct position
        ObjectOrientation road_orientation = instance.GetComponent<ObjectOrientation>();
        ObjectOrientation.Cardinal_Direction road_cardinal_direction = get_cardinal_direction(road_node);
        road_orientation.change_cardinal_direction(road_cardinal_direction);
    }

    /// Updates the references and gameobject of a single road node
    /// Returns true if the node was updated, and false if the node did not change at all
    private bool update_node_type_and_gameobject(Road_Node road_node_original)
    {
        /// 1- Recalculates the references and the road type of the road node
        /// Updated road node
        Road_Node road_node = new Road_Node(road_node_original);

        Vector3Int road_position = road_node.road_position;

        int current_road_connections = 0;
        Road_Node node_from_graph;

        /// North reference
        if (road_graph.TryGetValue(road_position + Vector3Int.up, out node_from_graph))
        {
            road_node.north = node_from_graph;
            ++current_road_connections;
        }
        else
        {
            road_node.north = null;
        }

        /// South reference
        if (road_graph.TryGetValue(road_position + Vector3Int.down, out node_from_graph))
        {
            road_node.south = node_from_graph;
            ++current_road_connections;
        }
        else
        {
            road_node.south = null;
        }

        /// East reference
        if (road_graph.TryGetValue(road_position + Vector3Int.right, out node_from_graph))
        {
            road_node.east = node_from_graph;
            ++current_road_connections;
        }
        else
        {
            road_node.east = null;
        }

        /// West reference
        if (road_graph.TryGetValue(road_position + Vector3Int.left, out node_from_graph))
        {
            road_node.west = node_from_graph;
            ++current_road_connections;
        }
        else
        {
            road_node.west = null;
        }

        switch (current_road_connections)
        {
        case 0:
            road_node.road_type = Road_Type.Closed;
            break;

        case 1:
            road_node.road_type = Road_Type.End;
            break;

        case 2:
            /// Straight road 1st case
            if (road_node.north != null && road_node.south != null)
            {
                road_node.road_type = Road_Type.Straight;
            }
            /// Straight road 2nd case
            else if (road_node.east != null && road_node.west != null)
            {
                road_node.road_type = Road_Type.Straight;
            }
            /// Any of the 4 possible turns (north-east, south-east, ...)
            else
            {
                road_node.road_type = Road_Type.Turn;
            }

            break;

        case 3:
            road_node.road_type = Road_Type.Triple_Intersection;
            break;

        case 4:
            road_node.road_type = Road_Type.Quad_Intersection;
            break;
        }

        /// 2- Compare the original road node to the updated road node to see if there are changes, if they are equal,
        ///    return false and do not update the graph
        if (road_node_original == road_node)
        {
            return false;
        }

        /// 3- Update the node gameobject
        update_node_gameobject(road_node);

        /// 4- Update the graph with the updated node
        //road_graph[road_position] = road_node;
        road_node_original.copy_node(road_node);

        /// 5- Return true to notify that the node changed
        return true;

    }

    /// Updates the 4 tiles (NSEW) (if they exists) around the tile in the argument and the tile itself
    private void update_node_types_and_gameobjects_around_tile(Road_Node center_road_node)
    {
        /// Update NORTH node (if exists)
        if (center_road_node.north != null)
        {
            update_node_type_and_gameobject(center_road_node.north);
        }
        /// Update SOUTH node (if exists)
        if (center_road_node.south != null)
        {
            update_node_type_and_gameobject(center_road_node.south);
        }
        /// Update EAST node (if exists)
        if (center_road_node.east != null)
        {
            update_node_type_and_gameobject(center_road_node.east);
        }
        /// Update WEST node (if exists)
        if (center_road_node.west != null)
        {
            update_node_type_and_gameobject(center_road_node.west);
        }
    }

    private List<Road_Node> get_adjacent_permanent_road_nodes(Road_Node road_node)
    {
        List<Road_Node> adjacent_permanent_road_nodes = new List<Road_Node>();

        if (road_node.north != null && road_node.north.road_placement_mode == Road_Placement_Mode.Permanent)
        {
            adjacent_permanent_road_nodes.Add(road_node.north);
        }

        if (road_node.south != null && road_node.south.road_placement_mode == Road_Placement_Mode.Permanent)
        {
            adjacent_permanent_road_nodes.Add(road_node.south);
        }

        if (road_node.east != null && road_node.east.road_placement_mode == Road_Placement_Mode.Permanent)
        {
            adjacent_permanent_road_nodes.Add(road_node.east);
        }

        if (road_node.west != null && road_node.west.road_placement_mode == Road_Placement_Mode.Permanent)
        {
            adjacent_permanent_road_nodes.Add(road_node.west);
        }

        return adjacent_permanent_road_nodes;
    }

    public void flush_temporal_roads()
    {
        List<Vector3Int> keys_to_delete = new List<Vector3Int>();
        HashSet<Road_Node> adjacent_permanent_roads = new HashSet<Road_Node>();

        /// Add temporal roads to a list to be deleted later
        foreach (KeyValuePair<Vector3Int, Road_Node> entry in road_graph)
        {
            Vector3Int current_road_position = entry.Key;
            Road_Node current_road_node = entry.Value;

            if (current_road_node.road_placement_mode == Road_Placement_Mode.Permanent)
            {
                continue;
            }

            keys_to_delete.Add(current_road_position);
            adjacent_permanent_roads.UnionWith(get_adjacent_permanent_road_nodes(current_road_node));
        }

        /// Deletes the roads (dictionary entry and gameobject instance)
        foreach (Vector3Int key in keys_to_delete)
        {
            delete_road(key);
        }

        /// Recalculates gameobjects and references of the permanent roads adjacent to the temporal ones
        foreach (Road_Node road_node in adjacent_permanent_roads)
        {
            update_node_type_and_gameobject(road_node);
        }
    }

    #endregion

    #region Add/Delete roads

    private void add_road(Vector3Int road_position)
    {
        /// If the road exists, don't add anything
        if (road_graph.ContainsKey(road_position))
        {
            return;
        }

        Road_Node road_node = new Road_Node(road_position, current_road_placement_mode);
        //print(road_node.road_position);
        road_graph[road_position] = road_node;

        update_node_type_and_gameobject(road_node);
        update_node_types_and_gameobjects_around_tile(road_node);

    }

    private List<Vector3Int> road_positions_queue = new List<Vector3Int>();

    public void add_roads(List<Vector3Int> road_positions, Road_Placement_Mode placement_mode)
    {
        /// We add the road positions in a late step, so for now they go in a queue
        road_positions_queue.AddRange(road_positions);

        /// Change the road placement state (cheap solution)
        current_road_placement_mode = placement_mode;

        /// IF we add the roads for good, we rebuild the navmeshes
        if (placement_mode == Road_Placement_Mode.Permanent)
        {
            rebuild_nav_mesh(walkable_ground_navmesh);
            rebuild_nav_mesh(vehicle_roads_navmesh);
        }
    }

    public void delete_road(Vector3Int road_position)
    {
        Road_Node road_node;
        /// If node exist, delete graph entry and gameobject instance
        if (road_graph.TryGetValue(road_position, out road_node))
        {
            Destroy(road_node.road_instance);

            road_graph.Remove(road_position);
        }
    }

    #endregion

    #region Update NavMeshSurfaces

    /// TODO: think if its best to have a list of navmeshes to update or each navmesh should have its own name
    [SerializeField] private NavMeshSurface vehicle_roads_navmesh;
    [SerializeField] private NavMeshSurface walkable_ground_navmesh;

    private void rebuild_nav_mesh(NavMeshSurface navmesh)
    {
        navmesh.BuildNavMesh();
    }

    #endregion

    #region Unity methods

    void Awake()
    {
        current = this;

        int road_types_count = Enum.GetNames(typeof(Road_Type)).Length;

        /// In the inspector we need to map each road type to a gameobject prefab, if we miss to map a type then
        /// it is an error
        if (road_types_count != road_gameobjects.Length)
        {
            //throw new Exception();
            Debug.LogError("A road type is missing a gameobject prefab");
        }

        /// Map the prefabs to a enum for quick access
        foreach (Road_Gameobject road_gameobject in road_gameobjects)
        {
            /// If a gameobject has no ObjectDirection component, is an error and it must be assigned
            if (road_gameobject.prefab.GetComponent<ObjectOrientation>() == null)
            {
                Debug.LogError("A road prefab is missing a ObjectORientation component");
            }

            road_gameobjects_mapped.Add(road_gameobject.type, road_gameobject.prefab);
        }

        /// Check if the navmeshes are assigned in the inspector
        if (walkable_ground_navmesh == null || vehicle_roads_navmesh == null)
        {
            Debug.LogError("A navmesh reference is missing");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void Update()
    {
        /// Each frame we flush the temporal roads (TODO: find way to not do this every frame)
        flush_temporal_roads();
    }

    void LateUpdate()
    {
        /// If there is a road in the queue, we now add it to the map (we do this here because earlier in the Update()
        /// method we delete roads
        foreach (Vector3Int road_position in road_positions_queue)
        {
            add_road(road_position);
        }

        road_positions_queue.Clear();
    }

    #endregion

}
