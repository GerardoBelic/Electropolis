using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Class that holds the info about the construction element we want to place
    over the map.
    It is placed over each construction button that we display
    in the selection panel.
*/

public class SelectionPanelButtonInfo : MonoBehaviour
{
    
    private ConstructionInfo construction_info;

    public ConstructionInfo get_construction_info()
    {
        return construction_info;
    }

    public void set_construction_info(ConstructionInfo _construction_info)
    {
        construction_info = _construction_info;
    }

}
