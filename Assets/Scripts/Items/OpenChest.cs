using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;
using Cinemachine;



public class OpenChest : MonoBehaviour
{
    public Camera mainCam;
    public CinemachineVirtualCamera dollyCam;

    private IEnumerator openingAnimation ()
    {
        GetComponent<Animator>().SetBool("isOpening", true);
        yield return new WaitForSeconds (5.0f);
        GameManager.Instance.SceneController.StartScoreScreen();
    }
    
    private void OnTriggerStay (Collider other) {

        if (other.gameObject.tag == "Player" && other.GetComponent<PlayerHealthController>().HasKey()) {
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
