using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUIController : MonoBehaviour
{
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
        if(Input.GetKeyDown(KeyCode.M) && mapUIController != null && !pausedMenu)
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

        if(Input.GetKeyDown(KeyCode.P) && !opennedMap)
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
