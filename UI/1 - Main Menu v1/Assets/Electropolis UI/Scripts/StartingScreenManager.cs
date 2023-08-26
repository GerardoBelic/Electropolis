using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;

/**
    Aclaraciones
    
    Aquí se mencionan algunos PREFIJOS de GameObjects para distinguir su funcionalidad esperada
        -Canvas: canvas donde la cámara ve para desplegar varios elementos de la UI
        -Panel/Subpanel: ventana dentro del canvas donde hay una serie de texto, imágenes, botones, etc.
        -Button: botón con el que podemos interaccionar (oprimiendo o posicionandonos sobre el)
        -Text: texto que podemos alterar (ej. fechas, valores, etc)
        -Layout: son secciones donde podemos agregar más contenido del UI y se van acomodando solos
        -Image: zona para poner una imagen de nuestra preferencia

        -All: puede referirse a todos los elementos de un mismo conjunto (ej. all_panels)
        -Prefab: un prefab el cual debemos instanciar y/o modificar

    Y aquí una lista de SUFIJOS
        -Enter: cuando entramos a un canvas
        -Pressed: principalmente usado en funciones, identifica la función asociada a un botón en específico
        -Open: para abrir un panel o ventana de un mismo canvas
        -Close: para cerrar un panel o ventana de un canvas

*/

public class StartingScreenManager : MonoBehaviour
{

    /// Canvas del menú principal
    [Header("MAIN MENU CANVAS")]
    public GameObject canv_main;

    /// La cámara donde hay componentes de interés como el animador
    [Header("CAMERA")]
    public GameObject camera;

    /// Paneles del menú principal
    [Header("MAIN MENU PANELS")]
    public GameObject panel_main;
    public GameObject panel_new_game;
    public GameObject panel_load_game;
    public GameObject panel_last_game;
    public GameObject panel_building_creator;
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
    public GameObject prefab_load_game_save_template;
    public GameObject subpanel_load_game_preview;
    public GameObject image_load_game_preview_city_portrait;
    public GameObject text_load_game_preview_city_name;
    public GameObject text_load_game_preview_population;
    public GameObject text_load_game_preview_money;

    /// Elementos del panel de carga de la última partida
    [Header("CONTINUE LAST GAME PANEL")]
    public GameObject subpanel_continue_game_warning;
    public GameObject button_continue_game_warning_ok;
    public GameObject subpanel_continue_game_preview;
    public GameObject button_continue_game_load;
    public GameObject button_continue_game_cancel;
    public GameObject image_continue_game_preview_city_portrait;
    public GameObject text_continue_game_city_name;
    public GameObject text_continue_game_population;
    public GameObject text_continue_game_money;
    public GameObject text_continue_game_last_saved;

    /// Elementos del panel de creación de construcciones
    [Header("BUILDING CREATOR")]
    public GameObject button_building_creator_ok;

    /// Elementos del panel de salida del juego
    [Header("EXIT GAME PANEL")]
    public GameObject button_exit_game_yes;
    public GameObject button_exit_game_no;

    /*----------------------------------------------------------------------------------
    ------------------------------------------------------------------------------------
                               .-""-.
                              (___/\ \
            ,                 (|^ ^ ) )
           /(                _)_\=_/  (
     ,..__/ `\          ____(_/_ ` \   )    Métodos para los paneles del menú principal
      `\    _/        _/---._/(_)_  `\ (
        '--\ `-.__..-'    /.    (_), |  )
            `._        ___\_____.'_| |__/
               `~----"`   `-.........'
    -------------------------------------------------------------------------------------
    -----------------------------------------------------------------------------------*/

    /// Deshabilita todos los paneles para que no se sobrepongan
    void all_panels_close()
    {
        panel_new_game.SetActive(false);
        panel_load_game.SetActive(false);
        panel_last_game.SetActive(false);
        panel_exit.SetActive(false);
    }    

    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
                ||||| 
               ||. .||
              |||\=/|||
              |.-- --.|
              /(.) (.)\
              \ ) . ( /
          _.--'(     \`-._                          Métodos para los elementos del panel de inicio
        .'      `.    )   `.
     .-'         /  .'      \
    '        _.-'  (         `-._
    \_)\_)\_)`._..  \ )\_)\_)\_)\_)\_)\_\_)\_)\_
    )\_)\_)\_ )\_)`--`_)\_)\_)\_)\_)\_)\_)\_)\_)
    \_)\_)\_)\_)\_)\_)\_)\_)\_)\_)\_)\_)\_)\_)\_
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    public void button_main_continue_last_game_pressed()
    {
        /// TODO: load the name and date of the files here or "OnEnable()" from "panel_last_game"

        all_panels_close();
        panel_last_game.SetActive(true);

        show_continue_game_save_preview();
        
    }

    public void button_main_new_game_pressed()
    {
        all_panels_close();
        panel_new_game.SetActive(true);
    }

    public void button_main_load_game_pressed()
    {
        all_panels_close();
        panel_load_game.SetActive(true);

        populate_saves_panel();
    }

    public void button_main_building_creator_pressed()
    {
        all_panels_close();
        panel_building_creator.SetActive(true);
    }

    public void button_main_options_pressed()
    {
        Animator camera_animator = camera.GetComponent<Animator>();
        camera_animator.SetInteger("State", 1);

    }

    public void button_main_credits_pressed()
    {
        /// TODO: change from "main" canvas to "credits" canvas with an animation
        Animator camera_animator = camera.GetComponent<Animator>();
        camera_animator.SetInteger("State", 2);
    }

    public void button_main_exit_game_pressed()
    {
        all_panels_close();
        panel_exit.SetActive(true);
    }

    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
                             sSSs
                            S{'SSS
                           sSS)sSSSs
                           SS(( S\SSs
                          sSSS) (\\SSS
     ~~_^~~~_~~^~~_^~^^^-^^SS/`-\//Ss~~^~~_^~~~^_^~^~^^
     -  _  - ~ -    - _ ~.-'  __//SSs -  _  -  _  ~- _  Métodos para los elementos del
    - ~_ -  _  -  _ - ,=(  -.;_,   `=, -   ~ _  - _  ~  panel de creación de juego
     _ -  -  _ ~-   ,='  ``',  (     `=, - _  - _-  -
       -~_  -  _   (         \(`        `)-  _     - 
      -   -      -~ `"~. _ ._ `_, _  _.=` ~     - 
       - _ ~- _-  _- ~-  _ -  _ ~ _ - - _ - _~ -~ -_ 
      _ -  -    _   -  _   -  _  -  _ -  - _  -  _
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    public void button_new_game_yes_pressed()
    {
        /// TODO: create game
        /// TODO: load the "loading" scene
    }

    public void button_new_game_no_pressed()
    {
        panel_new_game.SetActive(false);
    }

    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
          . o  O
        O  () o ().o
          o O.    _
     ___  .   ,~`'~`~._
    `-,_\  o(((\\ ~ _~`~._,,,_
        \\O . >>`-, ~ _~ ,~'~ 
         \`.o \__  /,-~,,~'`
          \ `.__\  )--.
           `.___ \/    `.
                )&,   ._ `.
                \_\_  ( `. \                Métodos para los elementos de
                  `.   `._`.\--,            carga de partidas
                    \_.-;;;.\,-'
                     \;;;;;;;.
                      \;;;;;;;\
                       `;;;;;;;\
                         `;;;;;;;. _______
                           `~;;;;;;.    _,'
                              `~.;;  ,-'
                                  \  ;
                                   \ ;      
                                    \j
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    /// This function add the save files to the vertical layout (names and last played dates)
    void populate_saves_panel()
    {

        /// First we delete the saves that we may previously loaded, EXCEPT for the save template
        foreach (Transform child in layout_load_game_saves.transform)
        {
            if (child.name != "Save_Template")
            {
                Destroy(child.gameObject);
            }
        }

        SaveInfo[] saves_info = SaveInfo.get_stored_saves_info();

        foreach(SaveInfo save in saves_info)
        {
            GameObject save_button = Instantiate(prefab_load_game_save_template, layout_load_game_saves.transform);

            TMP_Text save_button_name = save_button.transform.Find("Title").GetComponent<TMP_Text>();
            TMP_Text save_button_date = save_button.transform.Find("Date").GetComponent<TMP_Text>();

            save_button_name.text = Path.GetFileNameWithoutExtension(save.name);
            save_button_date.text = save.date.ToString();

            SaveInfo save_button_info = save_button.GetComponent<SaveInfo>();
            save_button_info.replace_values(save);

            save_button.SetActive(true);
        }

    }

    public void show_load_game_save_preview(SaveInfo save_to_preview)
    {

        /// 1.- Show the preview subpanel and the load button
        subpanel_load_game_preview.SetActive(true);
        button_load_game_load.SetActive(true);

        /// 2.- Load portrait from save directory (if it exists)
        Texture2D image_texture = new Texture2D(1920, 1080);
        Color[] default_pixels = Enumerable.Repeat(Color.red, image_texture.width * image_texture.height).ToArray();
        image_texture.SetPixels(default_pixels);
        image_texture.Apply();
        if (save_to_preview.portrait != null)
        {
            byte[] image_bytes = File.ReadAllBytes(save_to_preview.portrait.FullName);
            image_texture.LoadImage(image_bytes);
        }
        
        RawImage preview_portrait = image_load_game_preview_city_portrait.GetComponent<RawImage>();
        preview_portrait.texture = image_texture;
        AspectRatioFitter preview_portrait_fitter = image_load_game_preview_city_portrait.GetComponent<AspectRatioFitter>();
        float new_aspect_ratio = (float)image_texture.width / image_texture.height;
        preview_portrait_fitter.aspectRatio = new_aspect_ratio;

        /// 3.- Display save stats (city name, population and money)
        TMP_Text save_city_name = text_load_game_preview_city_name.GetComponent<TMP_Text>();
        TMP_Text save_population = text_load_game_preview_population.GetComponent<TMP_Text>();
        TMP_Text save_money = text_load_game_preview_money.GetComponent<TMP_Text>();

        /// TODO: load this fields from the file
        save_city_name.text = "???";
        save_population.text =  "???";
        save_money.text = "???";
        
    }

    public void button_load_game_load_pressed()
    {
        /// TODO: load selected game
        /// TODO: load "loading" scene
    }

    public void button_load_game_cancel_pressed()
    {
        panel_load_game.SetActive(false);
        subpanel_load_game_preview.SetActive(false);
        button_load_game_load.SetActive(false);
    }

    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
          .---.
         / /"\ \
         )/a a\(
        ( ( - ) )
         ) ) (  (
        (__)  \  )
       /,(@)~(@\__)
       \\ \   / //
        \\/\'/\//       Métodos para los elementos de continuación de la última partida
        (/^.^.^\)
         |^.^.^|
         |^.^.^|
         \^.^.^/
          \^.^/
           )^(
          /^.^\
         /.^,^.\ 
       ,/-,-|-,-\,
       ^~^~^`^~^~^
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    public void show_continue_game_save_preview()
    {

        /// 1.- Load the saves from disk
        SaveInfo[] saves_info = SaveInfo.get_stored_saves_info();

        /// If there is no saves, we load a messaage that says this to the user
        if (saves_info.Length == 0)
        {
            subpanel_continue_game_warning.SetActive(true);
            return;
        }

        SaveInfo save_to_preview = saves_info[0];

        /// 2.- Show the preview subpanel and the load button
        subpanel_continue_game_preview.SetActive(true);

        /// 3.- Load portrait from save directory (if it exists)
        Texture2D image_texture = new Texture2D(1920, 1080);
        Color[] default_pixels = Enumerable.Repeat(Color.red, image_texture.width * image_texture.height).ToArray();
        image_texture.SetPixels(default_pixels);
        image_texture.Apply();
        if (save_to_preview.portrait != null)
        {
            byte[] image_bytes = File.ReadAllBytes(save_to_preview.portrait.FullName);
            image_texture.LoadImage(image_bytes);
        }
        
        RawImage preview_portrait = image_continue_game_preview_city_portrait.GetComponent<RawImage>();
        preview_portrait.texture = image_texture;
        AspectRatioFitter preview_portrait_fitter = image_continue_game_preview_city_portrait.GetComponent<AspectRatioFitter>();
        float new_aspect_ratio = (float)image_texture.width / image_texture.height;
        preview_portrait_fitter.aspectRatio = new_aspect_ratio;

        /// 4.- Display save stats (city name, population and money)
        TMP_Text save_city_name = text_continue_game_city_name.GetComponent<TMP_Text>();
        TMP_Text save_population = text_continue_game_population.GetComponent<TMP_Text>();
        TMP_Text save_money = text_continue_game_money.GetComponent<TMP_Text>();
        TMP_Text save_last_saved = text_continue_game_last_saved.GetComponent<TMP_Text>();

        /// TODO: load this fields from the file
        save_city_name.text = "???";
        save_population.text =  "???";
        save_money.text = "???";
        save_last_saved.text = save_to_preview.date.ToString();
        
    }

    public void button_continue_game_warning_ok_pressed()
    {
        subpanel_continue_game_warning.SetActive(false);
    }

    public void button_continue_game_load_pressed()
    {
        /// TODO: load "loading" scene and load the game
    }

    public void button_continue_game_cancel_pressed()
    {
        panel_last_game.SetActive(false);
        subpanel_continue_game_preview.SetActive(false);
    }


    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
    ()
    ||,/))),    
    ||)/e e\)   
    (((\ O /(( 
     \)_)'(_)) 
      (|)^(|)\\
       ) . (  \\  
      ((\ /))  \\_,
       \\|//    ~~
       ((|))    o       Métodos para los elementos del creador de construcciones
        \|/      o
        |||   
       //'\\    O
      ((   ))   
       )   (  mn
       o
      o    o
       o
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    public void button_building_creator_ok_pressed()
    {
        panel_building_creator.SetActive(false);
    }

    /*-------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------
                  .---.
                 (_,/\ \
                (`^ ^(  )
                ) \=  ) (
           |\_  (.-' '--.)
            \(  /(_)-(_) \
             \\/ /\   /`\ \
              \_/ / . \  //
                 /'---'\`/_                 Métodos para los elementos de la salida
               _/ ^   ^ ;--;                del juego
           .--`| ^  ^ /`    `),
          /`  . \  ^ /`  ) .   ').
     ~^~`/  (    \^ / (       '  \^-~`-~
    -  ^ ~^-    . )/   .    )  '-.;~^-~^~-
       ~^~-      / `\ - .  ~^~ ,-.`~~^~^~^
     ~- `^_~-~^-| \^ \~_~^ -~^~- ~^`~^ ^~
      ~_~^- .-./__/\__`\-. ~^_-~^- ~^- 
         ^~ `-^~=~-`=~-~=-'    ~
    ---------------------------------------------------------------------------------------------
    -------------------------------------------------------------------------------------------*/

    public void button_exit_game_no_pressed()
    {
        panel_exit.SetActive(false);
    }

    public void button_exit_game_yes_pressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    // Start is called before the first frame update
    void Start()
    {
        //all_panels_close();
        //panel_main.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
