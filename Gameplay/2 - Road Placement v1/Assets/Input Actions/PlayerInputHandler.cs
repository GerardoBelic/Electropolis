using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    
    public void OnNoneSelected()
    {
        print("1");
    }

    public void OnBlockPlacement()
    {
        print("2");
    }

    public void OnZonePainting()
    {
        print("3");
    }

    public void OnRoadPlacement()
    {
        print("4");
    }

}
