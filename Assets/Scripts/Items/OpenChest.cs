using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;
using Cinemachine;



public class OpenChest : MonoBehaviour
{
    //public GameObject player; 
    // Start is called before the first frame update
    public Camera mainCam;
    public CinemachineVirtualCamera dollyCam;
    //public SceneController sController;
    private bool nearChest;
    private bool keyPressed;
    void Start()
    {
        nearChest = false;
        keyPressed = false; 
    }

    private IEnumerator openingAnimation ()
    {
        GetComponent<Animator>().SetBool("isOpening", true);
        Debug.Log("Function Run");
        yield return new WaitForSeconds (5.0f);
        // sController.GetComponent<SceneController>().StartScoreScreen();
        GameManager.Instance.SceneController.StartScoreScreen();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (nearChest) {
            if(Input.GetKeyDown(KeyCode.Z)) keyPressed = true;
        }
    }
    
    private void OnTriggerEnter (Collider other) {
        if ((other.gameObject.tag == "Player")) {
           //  Debug.Log("ey");
            //nearChest = true; 
            if(Input.GetKeyDown(KeyCode.Z))
            {
                GameManager.Instance.LocalPlayer.playerOff = true; 
               
                other.gameObject.active = false;
                dollyCam.GetComponent<CinemachineVirtualCamera>().Priority = 11;
               
                StartCoroutine(openingAnimation());
               
                    
            }
           
        }

    }
}
