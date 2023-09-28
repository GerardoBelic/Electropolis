using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField] private PlayerInput input;
    
    #region Select construction mode

    private void switch_to_select_construction_mode()
    {
        input.SwitchCurrentActionMap("Select Construction Mode");
    }

    private void switch_to_block_placement_mode()
    {
        input.SwitchCurrentActionMap("1 - Block Placement");
    }

    private void switch_to_zone_painting_mode()
    {
        input.SwitchCurrentActionMap("2 - Zone Painting");
    }

    private void switch_to_road_placement_mode()
    {
        input.SwitchCurrentActionMap("3 - Road Placement");
    }

    #endregion

    #region Change selection mode

    /// Can be called externally
    public void go_to_selection_mode()
    {
        switch_to_select_construction_mode();
    }

    #endregion

    #region Player input events subscriber

    [SerializeField] private BuildingSystem building_system;

    /// Map every action of the map 'Select Construction Mode' to the building system
    private void map_select_construction_mode_actions()
    {
        InputActionAsset input_action_asset = input.actions;
        InputActionMap input_action_map = input_action_asset.FindActionMap("Select Construction Mode");
        InputAction input_action;

        input_action = input_action_map.FindAction("None Selected");
        input_action.started += context => switch_to_select_construction_mode();

        input_action = input_action_map.FindAction("Block Placement");
        input_action.started += context => switch_to_block_placement_mode();

        input_action = input_action_map.FindAction("Zone Painting");
        input_action.started += context => switch_to_zone_painting_mode();

        input_action = input_action_map.FindAction("Road Placement");
        input_action.started += context => switch_to_road_placement_mode();
    }

    private void map_block_placement_actions()
    {
        InputActionAsset input_action_asset = input.actions;
        InputActionMap input_action_map = input_action_asset.FindActionMap("1 - Block Placement");
        InputAction input_action;

        input_action = input_action_map.FindAction("Select Object A");
        input_action.started += context => building_system.select_object_A();

        input_action = input_action_map.FindAction("Select Object B");
        input_action.started += context => building_system.select_object_B();

        input_action = input_action_map.FindAction("Rotate");
        input_action.started += context => building_system.rotate_object();

        input_action = input_action_map.FindAction("Place Object");
        input_action.started += context => building_system.place_object();

        input_action = input_action_map.FindAction("Cancel Placement / Exit Mode");
        input_action.started += context => building_system.cancel_placement_or_exit_mode();
    }

    private void map_zone_painting_actions()
    {
        InputActionAsset input_action_asset = input.actions;
        InputActionMap input_action_map = input_action_asset.FindActionMap("2 - Zone Painting");
        InputAction input_action;

        input_action = input_action_map.FindAction("Start Paint Zone");
        input_action.started += context => building_system.start_paint_zone();

        input_action = input_action_map.FindAction("End Paint Zone");
        input_action.canceled += context => building_system.end_paint_zone();

        input_action = input_action_map.FindAction("Cancel Placement / Exit Mode");
        input_action.started += context => building_system.cancel_zone_painting_or_exit_mode();
    }

    private void map_road_placement_actions()
    {
        InputActionAsset input_action_asset = input.actions;
        InputActionMap input_action_map = input_action_asset.FindActionMap("3 - Road Placement");
        InputAction input_action;

        input_action = input_action_map.FindAction("Start Road Placement");
        input_action.started += context => building_system.start_road_placement();

        input_action = input_action_map.FindAction("End Road Placement");
        input_action.canceled += context => building_system.end_road_placement();

        input_action = input_action_map.FindAction("Cancel Placement / Exit Mode");
        input_action.started += context => building_system.cancel_road_placement_or_exit_mode();

        input_action = input_action_map.FindAction("Rotate Road");
        input_action.started += context => building_system.rotate_road();
    }

    #endregion

    #region Unity methods

    void Awake()
    {
        /// 'input' must be assigned in the inspector
        if (input == null)
        {
            Debug.LogError("'input' must be assigned in the inspector");
        }

    }

    void Start()
    {
        map_select_construction_mode_actions();
        map_block_placement_actions();
        map_zone_painting_actions();
        map_road_placement_actions();
    }

    #endregion

}
