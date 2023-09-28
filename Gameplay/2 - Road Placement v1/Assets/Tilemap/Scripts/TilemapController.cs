using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// Esta clase sirve para guardar el tilemap y los tiles que componen su paleta.
/// Cada tilemap heredará de esta clase para agregar nombres a tiles específicos de su paleta
public class TilemapController : MonoBehaviour
{

    [SerializeField] private GridLayout gridLayout;
    private Grid grid;

    private Tilemap tilemap;
    [SerializeField] private TileBase[] palette;
    [SerializeField] private Vector3Int upperCorner;
    [SerializeField] private Vector3Int lowerCorner;

    /// Al iniciar el componente, este busca cual es el tilemap al que está asignado
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        /// Si no asignamos el componente a un tilemap, se considera un error
        if (tilemap == null)
        {
            throw new Exception();
        }

        /// Si la paleta está vacía, también es un error
        if (palette.Length == 0)
        {
            throw new Exception();
        }

        grid = gridLayout.gameObject.GetComponent<Grid>();

        // Inicializar el tilemap
        initTilemapController();
    }

    /// Debemos llamar esta función para ajustar algunas propiedades de los tilemaps
    public void initTilemapController(/*int worldWidth, int worldDepth*/)
    {
        //Vector3Int upperCorner = new Vector3Int();
        //Vector3Int lowerCorner
        resize_tilemap(/*upperCorner, lowerCorner*/);
    }

    /// Es necesario ajustar el tamaño del tilemap para pintar las celdas, de lo contrario
    /// no nos dejará pintar nada.
    /// El tamaño al que lo ajustaremos es al tamaño del mundo MÁS el borde del fin del mapa.
    /// Argumentos: las coordenadas de las esquianas opuestas del mapa para expandir el borde
    private void resize_tilemap(/*Vector3Int upperCorner, Vector3Int lowerCorner*/)
    {
        /// Este truco es para expandir los limites del TileMap, consiste en marcar las esquinas
        /// opuestas manualmente
        tilemap.SetTile(upperCorner, palette[0]);
        tilemap.SetTile(lowerCorner, palette[0]);

        tilemap.SetTile(upperCorner, null);
        tilemap.SetTile(lowerCorner, null);
    }

    /// Clear the tilemap tiles. The difference between this function and "Tilemap.ClearAllTiles()" is that this
    /// function preserves the tilemap size.
    public void clear_all_tiles()
    {
        tilemap.ClearAllTiles();

        resize_tilemap();
    }

    #region Verify if space is taken and take space

    private bool is_area_inside_bounds(BoundsInt area)
    {
        /// Start position of the area
        Vector3Int position = area.position;
        /// Size of the area (their values are positive)
        Vector3Int size = area.size;

        /// Check if the starting position is outside bounds
        if (position.x < lowerCorner.x || position.x > upperCorner.x ||
            position.y < lowerCorner.y || position.y > upperCorner.y)
        {
            return false;
        }

        /// Since the 'position' + 'size' can exceed the upper corner, we only
        /// check if the area exceeds the upper corner
        if (position.x + size.x - 1 > upperCorner.x ||
            position.y + size.y - 1 > upperCorner.y)
        {
            return false;
        }

        return true;
    }

    /// The 'PlaceableObject' script contains a way to calculate a prefab area size, and then we check if we it
    /// can be placed
    public bool can_be_placed(PlaceableObject placeable_object)
    {
        BoundsInt area = new BoundsInt();

        area.position = gridLayout.WorldToCell(placeable_object.GetStartPosition());
        area.size = placeable_object.Size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        if (!is_area_inside_bounds(area))
        {
            return false;
        }

        TileBase[] baseArray = get_tiles_block(area);

        foreach (var b in baseArray)
        {
            if (b != null)
            {
                return false;
            }
        }

        return true;
    }

    public void take_area(PlaceableObject placeable_object, int palette_index)
    {
        Vector3Int start = gridLayout.WorldToCell(placeable_object.GetStartPosition());
        Vector3Int size = placeable_object.Size;

        /// Since BoxFill() doesn't work if a tile inside the box is not empty, we instead set tile by tile
        for (int i = 0; i <= size.x; ++i)
        {
            for (int j = 0; j <= size.y; ++j)
            {
                Vector3Int offset = new Vector3Int(i, j, 0);
                tilemap.SetTile(start + offset, palette[palette_index]);
            }
        }
    }

    /// Checks if we can place the rectangle made by two opposite corners
    public bool can_be_placed(Vector3Int first_corner, Vector3Int second_corner)
    {
        (Vector3Int start, Vector3Int size) = get_start_and_size_of_corners(first_corner, second_corner);

        BoundsInt area = new BoundsInt();
        area.position = start;
        area.size = new Vector3Int(size.x + 1, size.y + 1, 1);

        if (!is_area_inside_bounds(area))
        {
            return false;
        }

        TileBase[] baseArray = get_tiles_block(area);

        foreach (var b in baseArray)
        {
            if (b != null)
            {
                return false;
            }
        }

        return true;
    }

    public void take_area(Vector3Int first_corner, Vector3Int second_corner, int palette_index)
    {
        (Vector3Int start, Vector3Int size) = get_start_and_size_of_corners(first_corner, second_corner);

        /// Since BoxFill() doesn't work if a tile inside the box is not empty, we instead set tile by tile
        for (int i = 0; i <= size.x; ++i)
        {
            for (int j = 0; j <= size.y; ++j)
            {
                Vector3Int offset = new Vector3Int(i, j, 0);
                tilemap.SetTile(start + offset, palette[palette_index]);
            }
        }
    }

    /// Checks if we can place all tiles in a list
    public bool can_be_placed(List<Vector3Int> tiles)
    {
        foreach (Vector3Int tile_position in tiles)
        {
            if (tilemap.GetTile(tile_position) != null)
            {
                return false;
            }

            /// If a tile its outside bounds, the tiles cannot be placed
            if (tile_position.x < lowerCorner.x || tile_position.x > upperCorner.x ||
                tile_position.y < lowerCorner.y || tile_position.y > upperCorner.y)
            {
                return false;
            }
        }

        return true;
    }

    public void take_area(List<Vector3Int> tiles, int palette_index)
    {
        foreach (Vector3Int tile_position in tiles)
        {
            tilemap.SetTile(tile_position, palette[palette_index]);
        }
    }

    #endregion

    #region Util

    /// Return the TileBases of an area
    private TileBase[] get_tiles_block(BoundsInt area)
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

    #endregion
    
}
