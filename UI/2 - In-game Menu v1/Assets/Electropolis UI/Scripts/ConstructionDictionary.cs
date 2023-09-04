using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

/**
    Holds all the buildings and brushes to place on the map (except roads)
    For the moment this class is a SINGLETON
*/

public class ConstructionDictionary : MonoBehaviour
{

    [SerializeField]
    private List<ConstructionInfo> construction_elements;

    void Awake()
    {
        if (construction_elements.Count == 0)
        {
            Debug.LogError("The dictionary of constructions is empty");
        }

        foreach (ConstructionInfo construction_element in construction_elements)
        {
            if (construction_element == null)
            {
                Debug.LogError("A ConstructionInfo is null in the ConstructionDictionary");
            }
        }
        
    }

    public ReadOnlyCollection<ConstructionInfo> get_construction_elements()
    {
        return construction_elements.AsReadOnly();
    }

}
