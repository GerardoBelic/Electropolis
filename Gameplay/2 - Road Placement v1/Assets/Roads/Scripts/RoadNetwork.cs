using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

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

        /// The constructor does NOT update its nodes and does NOT instantiate a road
        public Road_Node(Vector3Int _road_position)
        {
            road_position = _road_position;
        }
        
    }

    private Dictionary<Vector3Int, Road_Node> road_graph;

    private void update_all_node_type_and_references()
    {
        /// For each node, we update its north, south, east and west references and its current type
        foreach (var key_value_pair in road_graph)
        {
            Road_Node road_node = key_value_pair.Value;
            Vector3Int road_position = key_value_pair.Key;

            Road_Node node_from_graph;

            int current_road_connections = 0;

            /// North reference
            if (road_graph.TryGetValue(road_position + Vector3Int.up, out node_from_graph))
            {
                road_node.north = node_from_graph;
                ++current_road_connections;
            }

            /// South reference
            if (road_graph.TryGetValue(road_position + Vector3Int.down, out node_from_graph))
            {
                road_node.south = node_from_graph;
                ++current_road_connections;
            }

            /// East reference
            if (road_graph.TryGetValue(road_position + Vector3Int.right, out node_from_graph))
            {
                road_node.east = node_from_graph;
                ++current_road_connections;
            }

            /// West reference
            if (road_graph.TryGetValue(road_position + Vector3Int.left, out node_from_graph))
            {
                road_node.west = node_from_graph;
                ++current_road_connections;
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
        }
    }

    [Serializable]
    public struct Road_Gameobject
    {
        public Road_Type type;
        public GameObject prefab;
    }

    public Road_Gameobject[] road_gameobjects;
    private Dictionary<Road_Type, GameObject> road_gameobjects_mapped;

    private void update_all_gameobjects()
    {
        /// For each node, we update its north, south, east and west references and its current type
        foreach (var key_value_pair in road_graph)
        {
            Road_Node road_node = key_value_pair.Value;
            Vector3Int road_position = key_value_pair.Key;

            Vector3 gameobject_position = BuildingSystem.current.MainTilemap.CellToWorld(road_position);

            GameObject road_gameobject = road_gameobjects_mapped[road_node.road_type];
            GameObject instance = Instantiate(road_gameobject, gameobject_position, Quaternion.identity);
            instance.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            instance.transform.parent = this.gameObject.transform;
            road_node.road_instance = instance;
        }
    }

    public void add_road(Vector3Int road_position)
    {
        road_graph[road_position] = Road_Node(road_position);
        update_all_node_type_and_references();
        update_all_gameobjects();
    }

    public void delete_road(Vector3Int road_position)
    {
        road_graph.Remove(road_position);
        update_all_node_type_and_references();
        update_all_gameobjects();
    }

    void Awake()
    {
        current = this;

        int road_types_count = Enum.GetNames(typeof(Road_Type)).Length;

        /// In the inspector we need to map each road type to a gameobject prefab, if we miss to map a type then
        /// the game crashes
        if (road_types_count != road_gameobjects.Length)
        {
            throw new Exception();
        }

        foreach (Road_Gameobject road_gameobject in road_gameobjects)
        {
            road_gameobjects_mapped.Add(road_gameobject.type, road_gameobject.prefab);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
