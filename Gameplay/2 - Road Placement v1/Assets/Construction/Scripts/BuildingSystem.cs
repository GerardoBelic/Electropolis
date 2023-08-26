/// 3D Grid Building System - Unity Tutorial | City Builder, RTS, Factorio
/// By: Tamara Makes Games
/// https://www.youtube.com/watch?v=rKp9fWvmIww

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/**

    Building System
    This component let us put a variety of interactions with the tiles.
    In the next section we're discussing the steps it takes to complete the construction and placement of various types
    of blocks. The reason is if we change or cancel the current construction placement (e.g. changing from placing
    buildings to painting tiles), we need to CANCEL THE CURRENT OPERATION AT ANY TIME (its like we are in the middle of
    a "transaction" and we need to revert all non-commited changes) and revert to a safe state.


    Construction modes:

        1.- Block placement (e.g. buildings)
            a) Select block to place 
            b) - Move the block where the mouse is pointed
               - Rotate it (optional)
            c) Place the block if the space is available

        2.- Zone painting (to place proc buildings for example)
            a) Hold down click on the initial painting spot
            b) Drag mouse while click is down to paint an area
            c) Stop pressing on the final painting spot

        3.- Road placement
            a) Click on the initial road spot
            b) Mirror the road placement (optional)
            c) Click on the end road spot


*/

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] public Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;
    [SerializeField] private Tilemap selection_tilemap;
    [SerializeField] private TileBase selection_tile;

    public GameObject prefab1;
    public GameObject prefab2;

    private PlaceableObject objectToPlace;

    private enum Construction_Modes
    {
        None,
        Block_Placement,
        Zone_Painting,
        Road_Placement
    }

    Construction_Modes current_construction_mode = Construction_Modes.None;
    Construction_Modes previous_construction_mode = Construction_Modes.None;

    #region Unity methods

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        /// TODO: instead of reading and comparing the strings, we should compare keycodes for performance
        string input = Input.inputString;

        /// Change construction mode
        switch (input)
        {
        case "1":
            current_construction_mode = Construction_Modes.None;
            print("Construction mode selected: None");
            break;

        case "2":
            current_construction_mode = Construction_Modes.Block_Placement;
            print("Construction mode selected: Block Placement");
            break;

        case "3":
            current_construction_mode = Construction_Modes.Zone_Painting;
            print("Construction mode selected: Zone Painting");
            break;

        case "4":
            current_construction_mode = Construction_Modes.Road_Placement;
            print("Construction mode selected: Road Placement");
            break;
        }

        /// If we changed construction mode, we cancel the previous construction operation
        if (current_construction_mode != previous_construction_mode)
        {
            switch (previous_construction_mode)
            {
            case Construction_Modes.Block_Placement: 
                block_placement(true);
                break;

            case Construction_Modes.Zone_Painting: 
                zone_painting(true);
                break;

            case Construction_Modes.Road_Placement: 
                road_placement(true);
                break;
            }
        }

        /// Dispatch the operation on course
        switch (current_construction_mode)
        {
        case Construction_Modes.Block_Placement: 
            block_placement();
            break;

        case Construction_Modes.Zone_Painting: 
            zone_painting();
            break;

        case Construction_Modes.Road_Placement: 
            road_placement();
            break;
        }

        /// Update the previous mode
        previous_construction_mode = current_construction_mode;
    }

    #endregion

    #region Block placement

    public void block_placement(bool cancel_operation = false)
    {
        /// Default all variables
        if (cancel_operation)
        {
            if (objectToPlace)
            {
                Destroy(objectToPlace.gameObject);
                objectToPlace = null;
            }

            return;
        }

        /// a) Select block to place 
        /// If there is not a object selected, we wait until the user selects one
        if (!objectToPlace)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                InitializeWithObject(prefab1);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                InitializeWithObject(prefab2);
            }

            return;
        }

        /// We can rotate the object 90º
        if (Input.GetKeyDown(KeyCode.R))
        {
            objectToPlace.Rotate();
        }

        /// Make the position of the object permanent
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            /// Check if we can place the object (not occupying used space)
            /// TODO: mark in a custom material the object to show if it can be put down
            if (CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);

                objectToPlace = null;
            }
            /// If the space is taken, we destroy the object
            /// TODO: don't destory the object
            else
            {
                Destroy(objectToPlace.gameObject);
                objectToPlace = null;
            }
        }
        /// Deselect the object to be placed
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
            objectToPlace = null;
        }
    }

    /// Make an instance of a prefab and adds some components to drag and place the prefab in the map
    private void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        //GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        GameObject obj = Instantiate(prefab, position + prefab.transform.position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        ObjectDrag drag = obj.AddComponent<ObjectDrag>();
        //drag.originalOffset = prefab.transform.position;
    }

    /// Takes an object that is being dragged in the map, and checks if it can be placed (space not taken)
    /// Returns true if the cells in the object area are all empty, and false otherwise
    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeableObject.Size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);

        foreach (var b in baseArray)
        {
            if (b == whiteTile)
            {
                return false;
            }
        }

        return true;
    }

    /// Mark the area of an object as taken
    private void TakeArea(Vector3Int start, Vector3Int size)
    {
        //MainTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);

        /// Since BoxFill() doesn't work if a tile inside the box is not empty, we instead set tile by tile
        for (int i = 0; i <= size.x; ++i)
        {
            for (int j = 0; j <= size.y; ++j)
            {
                Vector3Int offset = new Vector3Int(i, j, 0);
                MainTilemap.SetTile(start + offset, whiteTile);
            }
        }
    }

    #endregion

    #region Zone painting

    private Vector3Int initial_painting_spot = Vector3Int.back;
    private Vector3Int final_painting_spot = Vector3Int.back;

    public void zone_painting(bool cancel_operation = false)
    {
        /// Default all variables
        if (cancel_operation)
        {
            initial_painting_spot = Vector3Int.back;
            final_painting_spot = Vector3Int.back;
            
            //selection_tilemap.ClearAllTiles();
            selection_tilemap.GetComponent<TilemapController>().clear_all_tiles();

            return;
        }

        /// a) Hold down click on the initial painting spot
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_world_position = GetMouseWorldPosition();

            /// If the ray didn't hit the plane surface, dont procede
            if (mouse_world_position == Vector3.zero)
            {
                return;
            }

            initial_painting_spot = gridLayout.WorldToCell(mouse_world_position);
        }

        /// b) Drag mouse while click is down to paint an area
        /// If the initial painting spot is invalid, we do not paint the selection tiles
        if (initial_painting_spot == Vector3Int.back)
            return;

        select_area_in_tilemap();

        /// c) Stop pressing on the final painting spot
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouse_world_position = GetMouseWorldPosition();

            if (mouse_world_position == Vector3.zero)
            {
                initial_painting_spot = Vector3Int.back;
                return;
            }

            final_painting_spot = gridLayout.WorldToCell(mouse_world_position);

            /// Make something with the inicial and final spot
            (Vector3Int start, Vector3Int size) = get_start_and_size_of_corners(initial_painting_spot, final_painting_spot);
            /// TODO: if there are tiles alredy in the main tile, if we take the area with a BoxFill() some tiles do not
            /// get marked, so we need to figure a way to mark every tile without problems
            TakeArea(start, size);

            /// Clear building mode
            zone_painting(true);
        }
    }

    /// Returns the start and size of an area between two opposite corners
    private (Vector3Int start, Vector3Int size) get_start_and_size_of_corners(Vector3Int first_corner, Vector3Int second_corner)
    {
        /// Calculate the area between the two corners and then make the sizes positive
        Vector3Int size = first_corner - second_corner;
        size.x = Math.Abs(size.x);
        size.y = Math.Abs(size.y);
        size.z = Math.Abs(size.z);

        /// Since the size must be always positive, the start must always be the inferior left corner
        Vector3Int start = new Vector3Int(first_corner.x, first_corner.y, first_corner.z);
        if (second_corner.x < first_corner.x)
        {
            start.x = second_corner.x;
        }
        if (second_corner.y < first_corner.y)
        {
            start.y = second_corner.y;
        }

        return (start: start, size: size);
    }

    private void select_area_in_tilemap()
    {
        /// To mark the selection area, we must first clear the previous selection
        selection_tilemap.GetComponent<TilemapController>().clear_all_tiles();

        /// The current mouse position is where the selection area must end
        Vector3Int current_painting_spot = gridLayout.WorldToCell(GetMouseWorldPosition());

        /// Since we need the start position and the sizes to fill the selection area, we call a function that converts
        /// two opposite corners
        (Vector3Int start, Vector3Int size) = get_start_and_size_of_corners(initial_painting_spot, current_painting_spot);

        /// We fill the selection area in the selection tilemap (not the main tilemap)
        take_selection_area(start, size);
    }

    private void take_selection_area(Vector3Int start, Vector3Int size)
    {
        selection_tilemap.BoxFill(start, selection_tile, start.x, start.y, start.x + size.x, start.y + size.y);
    }

    #endregion

    #region Road placement

    private enum Road_Direction
    {
        Left,
        Right
    }
    private Road_Direction current_road_direction = Road_Direction.Left;

    private Vector3Int initial_road_spot = Vector3Int.back;
    private Vector3Int final_road_spot = Vector3Int.back;

    public void road_placement(bool cancel_operation = false)
    {
        if (cancel_operation)
        {

        }

        /// a) Click on the initial road spot
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse_world_position = GetMouseWorldPosition();

            /// If the ray didn't hit the plane surface, dont procede
            if (mouse_world_position == Vector3.zero)
            {
                return;
            }

            initial_road_spot = gridLayout.WorldToCell(mouse_world_position);
        }

        /// b) Mirror the road placement (optional)
        if (initial_road_spot == Vector3Int.back)
            return;

        select_road_in_tilemap();

        /// c) Click on the end road spot

    }

    private class Road_Section
    {
        Vector3Int road_begin = Vector3Int.back;
        Vector3Int road_end = Vector3Int.back;
        Road_Direction road_direction = Road_Direction.Left;

        public Road_Section(Vector3Int _road_begin, Vector3Int _road_end, Road_Direction _road_direction)
        {
            road_begin = _road_begin;
            road_end = _road_end;
            road_direction = _road_direction;
        }

        public List<Vector3Int> get_road_tiles()
        {
            if (road_begin == Vector3Int.back || road_end == Vector3Int.back)
            {
                return new List<Vector3Int>();
            }

            Vector3Int first_section_begin = Vector3Int.back;
            Vector3Int first_section_end = Vector3Int.back;
            Vector3Int second_section_begin = Vector3Int.back;
            Vector3Int second_section_end = Vector3Int.back;

            List<Vector3Int> road_tiles;

            /// If the road is straight
            if (road_begin.x == road_end.x)
            {
                int min = Math.Min(road_begin.y, road_end.y);
                int max = Math.Max(road_begin.y, road_end.y);

                for (int i = min; i <= max; ++i)
                {
                    road_tiles.Add(new Vector3Int(road_begin.x, i, 0));
                }

                return road_tiles;
            }
            else if (road_begin.y == road_end.y)
            {
                int min = Math.Min(road_begin.x, road_end.x);
                int max = Math.Max(road_begin.x, road_end.x);

                for (int i = min; i <= max; ++i)
                {
                    road_tiles.Add(new Vector3Int(i, road_begin.y, 0));
                }
                    
                return road_tiles;
            }

            /// In other case, the road has a turn
            Vector3Int corner_tile;
            if (road_direction = Road_Direction.Left)
            {
                if (road_begin.x < road_end.x)
                {
                    corner_tile = new Vector3Int(road_begin.x, road_end.y, 0);
                }
                else
                {
                    corner_tile = new Vector3Int(road_end.x, road_begin.y, 0);
                }
            }
            else
            {
                if (road_begin.y < road_end.y)
                {
                    corner_tile = new Vector3Int(road_begin.x, road_end.y, 0);
                }
                else
                {
                    corner_tile = new Vector3Int(road_end.x, road_begin.y, 0);
                }
            }
            

        }
    }

    private void select_road_in_tilemap()
    {
        /// To mark the road area, we must first clear the previous selection (for now we use the selection tilemap)
        selection_tilemap.GetComponent<TilemapController>().clear_all_tiles();

        /// The current mouse position is where the selection area must end
        Vector3Int current_road_spot = gridLayout.WorldToCell(GetMouseWorldPosition());

        
    }

    #endregion

    #region Utils

    // Lanzamos un rayo para saber en que coordenadas del mundo tocó nuestro mouse (tiene que colisionar con un objeto)
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        /// TODO: put a layer mask to filter buildings
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// Devuelve las coordenadas del centro de la celda mas cercana de una posición determinada
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        Vector3 newPos = grid.GetCellCenterWorld(cellPos);

        return newPos;
    }

    /// Devuelve el tipo de celdas de un área determinada
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            ++counter;
        }

        return array;

    }

    #endregion

}
