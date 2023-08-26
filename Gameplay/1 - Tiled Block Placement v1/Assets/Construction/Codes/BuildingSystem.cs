/// 3D Grid Building System - Unity Tutorial | City Builder, RTS, Factorio
/// By: Tamara Makes Games
/// https://www.youtube.com/watch?v=rKp9fWvmIww

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;

    public GameObject prefab1;
    public GameObject prefab2;

    private PlaceableObject objectToPlace;

    #region Unity methods

    private void Awake()
    {
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        // Si no hay ninguna construcción para poner, esperamos que el usuario elija una
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

        // Rotamiento del objeto
        if (Input.GetKeyDown(KeyCode.R))
        {
            objectToPlace.Rotate();
        }
        // Posicionamiento "permanente" del objeto
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            // Si no hay celdas bloqueando el espacio, podemos poner la construcción...
            if (CanBePlaced(objectToPlace))
            {
                objectToPlace.Place();
                Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                TakeArea(start, objectToPlace.Size);

                objectToPlace = null;
            }
            // ... si no, lo destruimos
            else
            {
                Destroy(objectToPlace.gameObject);
                objectToPlace = null;
            }
        }
        // Podemos borrar el objeto que queríamos posicionar
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
            objectToPlace = null;
        }
    }

    #endregion

    #region Utils

    // Lanzamos un rayo para saber en que coordenadas del mundo tocó nuestro mouse (tiene que colisionar con un objeto)
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    // Devuelve las coordenadas del centro de la celda mas cercana de una posición determinada
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        Vector3 newPos = grid.GetCellCenterWorld(cellPos);

        return newPos;
    }

    // Devuelve el tipo de celdas de un área determinada
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

    #region Building Placement

    // Instancia un objeto en la escena y le agrega un componente para poder arrastrarlo
    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        //GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        GameObject obj = Instantiate(prefab, position + prefab.transform.position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        ObjectDrag drag = obj.AddComponent<ObjectDrag>();
        //drag.originalOffset = prefab.transform.position;
    }

    // Toma las coordenadas y área que está ocupando un objeto, y revisa que las celdas no estén ocupadas.
    // Devuelve verdadero si las celdas del área están vacías, y devuelve falso cuando por lo menos una celda está ocupada
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

    // Una vez puesto el objeto, vamos a rellenar las celdas vacías para indicar que ya está ocupado el espacio
    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        MainTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);
    }

    #endregion

}
