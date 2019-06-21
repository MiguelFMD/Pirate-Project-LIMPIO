using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PuzleTrigger : MonoBehaviour
    {
        public VisualPuzle visualPuzle;
        public Puzle nextPuzle;

        private bool firstTime = true;
        public int puzleID;
        public MapUIController MapUIController;

        void OnTriggerStay(Collider other)
        {
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();
            if(other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.Space) && player.sableMode)
            {
                if(firstTime)
                {
                    MapUIController.EnablePuzleItemZone(puzleID);
                    firstTime = false;
                }

                if(!visualPuzle.GetEndedPuzle())
                {  
                    visualPuzle.IntroducePuzle(player);
                }
                else if(!nextPuzle.GetEndedPuzle())
                {
                    nextPuzle.IntroducePuzle(player);
                }
            }
        }
    }
}
