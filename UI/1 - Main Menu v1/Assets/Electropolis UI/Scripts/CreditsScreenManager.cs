using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class CreditsScreenManager : MonoBehaviour
{

    /// Canvas del menú principal
    [Header("CREDITS CANVAS")]
    public GameObject canv_credits;

    /// La cámara donde hay componentes de interés como el animador
    [Header("CAMERA")]
    public GameObject camera;

    /// Elementos de la ventana de créditos
    [Header("CREDITS ELEMENTS")]
    public GameObject text_title;
    public GameObject layout_all_credits;
    //public GameObject button_return;

    public void canv_credits_enter()
    {
        text_title.SetActive(true);
        layout_all_credits.SetActive(true);
    }

    public void button_return_pressed()
    {
        Animator camera_animator = camera.GetComponent<Animator>();
        camera_animator.SetInteger("State", 0);

        text_title.SetActive(false);
        layout_all_credits.SetActive(false);
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
