using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInfo : MonoBehaviour
{

    public enum Construction_Classification
    {
        Residence,
        Commerce,
        Industry,
        Service,
        Other
    }

    public enum Construction_Placement_Type
    {
        Building,
        Brush
    }

    public enum Construction_Lock_Status
    {
        Unlocked,
        Locked,
    }

    public string construction_name = "construction name";
    public List<string> tags = new List<string>{"default tag"};
    public Construction_Classification construction_classification;
    public Construction_Placement_Type construction_placement_type;
    //public GameObject prefab;
    public Construction_Lock_Status construction_lock_status;
    [HideInInspector] public Sprite construction_render;

    /// TODO: import save file to read if we have unlocked some of the constructions
    void import_construction_status()
    {

    }

    /// TODO: to not render a construction over and over, read a render if it exist
    void import_construction_render()
    {

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
