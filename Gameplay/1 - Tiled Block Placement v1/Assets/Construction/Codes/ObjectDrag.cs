/// 3D Grid Building System - Unity Tutorial | City Builder, RTS, Factorio
/// By: Tamara Makes Games
/// https://www.youtube.com/watch?v=rKp9fWvmIww

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 originalOffset;    // la posición de los objetos puede ser ajustada en su prefab, por lo que es necesario conservar la posición original
    private Vector3 offset;

    // Cuando apretamos el mouse, guardamos la distancia que hay entre el centro del objeto y el puntero del ratón en el mundo
    private void OnMouseDown()
    {
        offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    }

    // Cuando arrastremos el mouse, moveremos la posición del objeto al centro de la celda mas cercana al puntero del mouse (tomando en cuenta el offset de OnMouseDown())
    private void OnMouseDrag()
    {
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
        
        Vector3 snapedPos = BuildingSystem.current.SnapCoordinateToGrid(pos);
        snapedPos.y = originalOffset.y;

        transform.position = snapedPos;
    }

    // Debido a que el centro del objeto no es igual a la posición del prefab, es preferente guardar esta última para que los objetos no se metan debajo del suelo
    private void Awake()
    {
        originalOffset = transform.position;
    }
}
