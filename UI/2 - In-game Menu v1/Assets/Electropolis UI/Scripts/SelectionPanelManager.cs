using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/**

    Selection Panel Manager purposes:
        -Stores all elements that can be constructed/painted on the map
        -Shows/hides elements and element groups depending on the type of constructions we want to place.
        -Waits until an element is selected from all the options in the panel and dispatch an event to the builder.

*/

public class SelectionPanelManager : MonoBehaviour
{

    /// Selection panel gameobject
    [SerializeField]
    private GameObject selection_panel;

    /// Dictionary where all the constructions and brushes are
    [SerializeField]
    private ConstructionDictionary construction_dictionary;

    /// Content panel where the groups of elements are to be instantiated
    [SerializeField]
    private GameObject content_panel;

    /// Elements group blueprint for making a group of various constructions
    [SerializeField]
    private GameObject element_group_blueprint;

    /// Element blueprint for each construction (which is a button with an image and a title)
    [SerializeField]
    private GameObject element_blueprint;

    /// Default texture to be placed in the element profile
    [SerializeField]
    private Sprite default_element_profile_image;

    /// Element groups to display in the panels, one at a given time
    /// The key is the the construction info tag (a construction can have many tags, thus appearing in multiple groups),
    /// and the value is a list of the constructions to show in the same group
    private Dictionary<string, GameObject> residence_groups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> commerce_groups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> industry_groups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> service_groups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> other_groups = new Dictionary<string, GameObject>();

    #region Initialization methods

    /// Returns true if there was an error, otherwise returns false
    private bool initialization_error_checker()
    {
        int number_of_errors = 0;

        /// Check that the selection panel is assigned in the inspector
        if (selection_panel == null)
        {
            Debug.LogError("No selection panel assigned");
            number_of_errors++;
        }

        /// Check that the construction dictionary is assigned in the inspector
        if (construction_dictionary == null)
        {
            Debug.LogError("No constructor dictionary assigned");
            number_of_errors++;
        }

        /// Chek if content panel is assigned in the inspector
        if (content_panel == null)
        {
            Debug.LogError("No content panel assigned (Selection Panel -> Panel Scroller -> Viewport -> Content)");
            number_of_errors++;
        }

        /// Check that the element group blueprint has the next children with the names and components:
        /// -Elements Group Blueprint
        ///     -Group Title (TMP Text)
        ///     -Elements (Grid Layout Group)
        if (element_group_blueprint == null)
        {
            Debug.LogError("No element group blueprint assigned");
            number_of_errors++;
        }

        TMP_Text group_title = element_group_blueprint.transform.Find("Group Title").GetComponent<TMP_Text>();
        if (group_title == null)
        {
            Debug.LogError("No group title found in Elements Group Blueprint (TMP Text)");
            number_of_errors++;
        }

        GridLayoutGroup elements = element_group_blueprint.transform.Find("Elements").GetComponent<GridLayoutGroup>();
        if (elements == null)
        {
            Debug.LogError("No elements found in Elements Group Blueprint (Grid Layout Group)");
            number_of_errors++;
        }

        /// Check that the element blueprint has the next children with the names and components:
        /// -Element profile (Button)
        ///     -Image Frame
        ///         -Image (Image)
        ///     -Name Frame
        ///         -Name (TMP Text)
        if (element_blueprint == null)
        {
            Debug.LogError("No element blueprint assigned");
            number_of_errors++;
        }

        Button element_profile_button = element_blueprint.GetComponent<Button>();
        if (element_profile_button == null)
        {
            Debug.LogError("No button found in Element Profile (Button)");
            number_of_errors++;
        }
    
        Image element_profile_image = element_blueprint.transform.Find("Image Frame/Image").GetComponent<Image>();
        if (element_profile_image == null)
        {
            Debug.LogError("No image found in Element Profile -> Image Frame -> Image (Image)");
            number_of_errors++;
        }

        TMP_Text element_profile_name = element_blueprint.transform.Find("Name Frame/Name").GetComponent<TMP_Text>();
        if (element_profile_name == null)
        {
            Debug.LogError("No name found in Element Profile -> Name Frame -> Name (TMP Text)");
            number_of_errors++;
        }


        if (number_of_errors > 0)
        {
            return true;
        }
        return false;

    }

    /// Add a element group to the selection panel with a title
    private GameObject instantiate_element_group(string group_name)
    {
        GameObject instance = Instantiate(element_group_blueprint, content_panel.transform);

        TMP_Text group_title = instance.transform.Find("Group Title").GetComponent<TMP_Text>();
        group_title.text = group_name;

        return instance;
    }

    /// Add an element with its respective button info, image portrait and name
    private void instantiate_element(ConstructionInfo construction_info, GameObject element_group)
    {
        GameObject instance = Instantiate(element_blueprint, element_group.transform.Find("Elements"));
        //instance.transform.parent = element_group.transform;

        SelectionPanelButtonInfo button_info = instance.AddComponent<SelectionPanelButtonInfo>();
        button_info.set_construction_info(construction_info);

        Image button_image = instance.transform.Find("Image Frame/Image").GetComponent<Image>();
        if (construction_info.construction_render == null)
        {
            button_image.sprite = default_element_profile_image;
        }
        else
        {
            button_image.sprite = construction_info.construction_render;
        }

        TMP_Text button_title = instance.transform.Find("Name Frame/Name").GetComponent<TMP_Text>();
        button_title.text = construction_info.construction_name;

        instance.SetActive(true);
    }

    /// Fills the groups dictionaries
    private void element_groups_initialization()
    {
        /// For each construction in the dictionary
        foreach (ConstructionInfo construction_info in construction_dictionary.get_construction_elements())
        {
            List<string> construction_tags = construction_info.tags;

            /// Each construction can go into multiple groups depending on its number of tags
            foreach (string tag in construction_tags)
            {
                GameObject element_group = null;

                switch (construction_info.construction_classification)
                {
                case ConstructionInfo.Construction_Classification.Residence:
                    
                    /// If the group doesn't exist (the tag is the group identifier), we create it
                    if (!residence_groups.TryGetValue(tag, out element_group))
                    {
                        GameObject group_panel_gameobject = instantiate_element_group(tag);
                        element_group = residence_groups[tag] = group_panel_gameobject;
                    }

                    instantiate_element(construction_info, element_group);

                    break;

                case ConstructionInfo.Construction_Classification.Commerce:
                    
                    /// If the group doesn't exist (the tag is the group identifier), we create it
                    if (!commerce_groups.TryGetValue(tag, out element_group))
                    {
                        GameObject group_panel_gameobject = instantiate_element_group(tag);
                        element_group = commerce_groups[tag] = group_panel_gameobject;
                    }

                    instantiate_element(construction_info, element_group);

                    break;

                case ConstructionInfo.Construction_Classification.Industry:
                    
                    /// If the group doesn't exist (the tag is the group identifier), we create it
                    if (!industry_groups.TryGetValue(tag, out element_group))
                    {
                        GameObject group_panel_gameobject = instantiate_element_group(tag);
                        element_group = industry_groups[tag] = group_panel_gameobject;
                    }

                    instantiate_element(construction_info, element_group);

                    break;

                case ConstructionInfo.Construction_Classification.Service:
                    
                    /// If the group doesn't exist (the tag is the group identifier), we create it
                    if (!service_groups.TryGetValue(tag, out element_group))
                    {
                        GameObject group_panel_gameobject = instantiate_element_group(tag);
                        element_group = service_groups[tag] = group_panel_gameobject;
                    }

                    instantiate_element(construction_info, element_group);

                    break;

                default:
                    
                    /// If the group doesn't exist (the tag is the group identifier), we create it
                    if (!other_groups.TryGetValue(tag, out element_group))
                    {
                        GameObject group_panel_gameobject = instantiate_element_group(tag);
                        element_group = other_groups[tag] = group_panel_gameobject;
                    }

                    instantiate_element(construction_info, element_group);

                    break;
                }
            }
        }
    }

    #endregion

    #region Show/hide groups

    private void open_selection_panel()
    {
        /// TODO: force rebuild on parents of layout groups so that they are shown correclty
        selection_panel.SetActive(true);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(selection_panel.GetComponent<RectTransform>());
    }

    public void close_selection_panel()
    {
        selection_panel.SetActive(false);
    }

    private void hide_all_groups()
    {
        Dictionary<string, GameObject>[] all_groups = {residence_groups, commerce_groups, industry_groups, service_groups, other_groups};

        foreach (Dictionary<string, GameObject> group_dictionary in all_groups)
        {
            foreach (KeyValuePair<string, GameObject> group in group_dictionary)
            {
                group.Value.SetActive(false);
            }
        }
    }

    public void show_residence_group()
    {
        hide_all_groups();

        foreach (KeyValuePair<string, GameObject> group in residence_groups)
        {
            group.Value.SetActive(true);
        }

        open_selection_panel();
    }

    public void show_commerce_group()
    {
        hide_all_groups();

        foreach (KeyValuePair<string, GameObject> group in commerce_groups)
        {
            group.Value.SetActive(true);
        }

        open_selection_panel();
    }

    public void show_industry_group()
    {
        hide_all_groups();

        foreach (KeyValuePair<string, GameObject> group in industry_groups)
        {
            group.Value.SetActive(true);
        }

        open_selection_panel();
    }

    public void show_service_group()
    {
        hide_all_groups();

        foreach (KeyValuePair<string, GameObject> group in service_groups)
        {
            group.Value.SetActive(true);
        }

        open_selection_panel();
    }

    public void show_others_group()
    {
        hide_all_groups();

        foreach (KeyValuePair<string, GameObject> group in other_groups)
        {
            group.Value.SetActive(true);
        }

        open_selection_panel();
    }

    #endregion

    #region Unity methods

    void Awake()
    {
        bool initialization_error = initialization_error_checker();

        element_groups_initialization();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Element selected

    public void element_selected_callback(GameObject element_selected)
    {
        /// TODO: do something with the element
        SelectionPanelButtonInfo button_info = element_selected.GetComponent<SelectionPanelButtonInfo>();

        if (button_info == null)
        {
            Debug.LogError("Element selected has no button info (SelectionPanelButtonInfo)");
        }

        ConstructionInfo construction_info = button_info.get_construction_info();

        print(construction_info.name);

        close_selection_panel();

    }

    #endregion

}
