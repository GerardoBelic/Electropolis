using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Aclaraciones
    
    Aquí se mencionan algunos prefijos de GameObjects para distinguir su funcionalidad esperada
        -Canvas: ventana a donde la cámara ve para desplegar varios elementos de la UI
        -Panel: subventana dentro del canvas donde hay una serie de texto, imágenes, botones, etc.
        -Button: botón con el que podemos interaccionar (oprimiendo o posicionandonos sobre el)
        -Text: texto que podemos alterar (ej. fechas, valores, etc)
        -Layout: son secciones donde podemos agregar más contenido del UI y se van acomodando solos

*/

/// Este espacio es para todas las pantallas de la pantalla de inicio (menú principal, opciones, créditos, ...)
namespace Starting_Screen
{

    public class Canvas_Main_Menu : MonoBehaviour
    {

        /// Canvas del menú principal
        [Header("MAIN MENU CANVAS")]
        [Tooltip("Canvas donde están las opciones del menú principal")]
        public GameObject canv_main;

        /// Elementos del menú principal
        [Header("MAIN MENU PANELS")]
        [Tooltip("El meú principal con todas las opciones para jugar")]
        public GameObject panel_main;
        [Tooltip("Menú para crear una nueva partida")]
        public GameObject panel_new_game;
        [Tooltip("Menú para cargar una partida")]
        public GameObject panel_load_game;
        [Tooltip("Menú para cargar la última partida jugada")]
        public GameObject panel_last_game;
        [Tooltip("Menú para salir del juego")]
        public GameObject panel_exit;

        /// Elementos del panel de inicio
        [Header("MAIN PANEL")]
        public GameObject button_main_continue_last_game;
        public GameObject button_main_new_game;
        public GameObject button_main_load_game;
        public GameObject button_main_building_creator;
        public GameObject button_main_options;
        public GameObject button_main_credits;
        public GameObject button_main_exit_game;

        /// Elementos del panel de creación de juego
        [Header("NEW GAME PANEL")]
        public GameObject button_new_game_yes;
        public GameObject button_new_game_no;

        /// Elementos del panel de carga de partidas
        [Header("LOAD GAME PANEL")]
        public GameObject button_load_game_load;
        public GameObject button_load_game_cancel;
        public GameObject layout_load_game_saves;
        public GameObject text_load_game_city_name;
        public GameObject text_load_game_population;
        public GameObject text_load_game_money;

        /// Elementos del panel de carga de la última partida
        [Header("CONTINUE LAST GAME PANEL")]
        public GameObject button_continue_game_load;
        public GameObject button_continue_game_cancel;
        public GameObject text_continue_game_city_name;
        public GameObject text_continue_game_population;
        public GameObject text_continue_game_money;
        public GameObject text_continue_game_last_saved;

        /// Elementos del panel de salida del juego
        [Header("EXIT GAME PANEL")]
        public GameObject button_exit_game_yes;
        public GameObject button_exit_game_no;

        /// Botones del panel 

        // Deshabilita todos los paneles para que no se sobrepongan
        void disable_panels()
        {
            panel_new_game.SetActive(false);
            panel_load_game.SetActive(false);
            panel_last_game.SetActive(false);
            panel_exit.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            disable_panels();
            panel_main.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void open_panel_last_game()
        {
            disable_panels();
            panel_last_game.SetActive(true);

            /// TODO: cargar las partidas y ver cual es la más reciente para desplegarla
        }

        void open_panel_new_game()
        {
            disable_panels();
            panel_new_game.SetActive(true);

            /// TODO: crear la partida
        }

        void open_panel_load_game()
        {
            disable_panels();
            panel_load_game.SetActive(true);

            /// TODO: cargar todas las partidas de un directorio con todo y fecha
        }

        void open_panel_exit()
        {
            disable_panels();
            panel_exit.SetActive(true);
            
            /*#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif*/
        }
    }

    class Option_Menu_Canvas
    {

    }

    class Credit_Menu_Canvas
    {

    }

}

public class UI_MainScreen_Manager : MonoBehaviour
{

    


}
