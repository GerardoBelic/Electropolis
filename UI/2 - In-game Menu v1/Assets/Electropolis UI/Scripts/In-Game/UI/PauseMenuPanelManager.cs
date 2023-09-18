using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Manager for the pause menu :v
    TODO: when we open the pause menu, the game has to stop
*/

public class PauseMenuPanelManager : MonoBehaviour
{

    [SerializeField] private GameObject pause_menu_panel;

    [SerializeField] private GameObject quit_message_panel;
    
    #region Resume game

    public void resume_button_pressed()
    {
        /// TODO: unpause game

        pause_menu_panel.SetActive(false);
    }

    #endregion

    #region  Save game

    #endregion

    #region Load game

    #endregion

    #region Options

    #endregion

    #region Quit

    public void quit_button_pressed()
    {
        quit_message_panel.SetActive(true);
        pause_menu_panel.SetActive(false);
    }

    public void quit_panel_no_button_pressed()
    {
        quit_message_panel.SetActive(false);
        pause_menu_panel.SetActive(true);
    }

    public void quit_panel_yes_button_pressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    #endregion

}
