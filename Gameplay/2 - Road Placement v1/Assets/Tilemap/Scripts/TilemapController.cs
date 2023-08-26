using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// Esta clase sirve para guardar el tilemap y los tiles que componen su paleta.
/// Cada tilemap heredará de esta clase para agregar nombres a tiles específicos de su paleta
public class TilemapController : MonoBehaviour
{

    private Tilemap tilemap;
    public TileBase[] palette;
    public Vector3Int upperCorner;
    public Vector3Int lowerCorner;

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

        // Inicializar el tilemap
        initTilemapController();

    }

    /// Debemos llamar esta función para ajustar algunas propiedades de los tilemaps
    public void initTilemapController(/*int worldWidth, int worldDepth*/)
    {
        //Vector3Int upperCorner = new Vector3Int();
        //Vector3Int lowerCorner
        resizeTilemap(/*upperCorner, lowerCorner*/);
    }

    /// Es necesario ajustar el tamaño del tilemap para pintar las celdas, de lo contrario
    /// no nos dejará pintar nada.
    /// El tamaño al que lo ajustaremos es al tamaño del mundo MÁS el borde del fin del mapa.
    /// Argumentos: las coordenadas de las esquianas opuestas del mapa para expandir el borde
    private void resizeTilemap(/*Vector3Int upperCorner, Vector3Int lowerCorner*/)
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

        resizeTilemap();
    }
    
}
