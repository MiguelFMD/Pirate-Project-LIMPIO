using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class GeneralUIController : MonoBehaviour
{
    private InputController m_InputController;
    public InputController InputController
    {
        get {
            if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
            return m_InputController;
        }
    }

    public PauseMenu pauseMenu;
    public MapUIController mapUIController;

    private bool pausedMenu;
    private bool opennedMap;

    void Start()
    {
        pausedMenu = opennedMap = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(InputController.ToMap && mapUIController != null && !pausedMenu)
        {
            if(opennedMap)
            {
                mapUIController.CloseMap();
                opennedMap = false;
            }
            else
            {
                mapUIController.OpenMap();
                opennedMap = true;
            }
        }

        if(InputController.Pause && !opennedMap)
        {
            if(pausedMenu)
            {
                pauseMenu.Resume();
                pausedMenu = false;
            }
            else
            {
                pauseMenu.Pause();
                pausedMenu = true;
            }
        }
    }
}
