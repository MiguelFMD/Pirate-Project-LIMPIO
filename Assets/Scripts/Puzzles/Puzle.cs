using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DefinitiveScript
{
    public class Puzle : MonoBehaviour
    {
        protected bool onPuzle;
        protected bool endedPuzle = false;

        public CinemachineVirtualCamera puzleCamera;
        protected Vector3 originalCameraLocalPosition;
        protected Quaternion originalCameraLocalRotation;

        protected PlayerBehaviour player;

        public PuzleController PuzleController;

        public int puzleID;

        public virtual void StartPuzle()
        {

        }

        protected virtual void FinishPuzle()
        {
            onPuzle = false;
        }

        protected virtual void InitializePuzle()
        {

        }

        public void SendInfo(PlayerBehaviour param0, Vector3 param1, Quaternion param2)
        {
            player = param0;
            originalCameraLocalPosition = param1;
            originalCameraLocalRotation = param2;
        }

        public void IntroducePuzle(PlayerBehaviour player)
        {
            this.player = player;
            this.player.playerOff = true;
            this.player.MakeVisible(false);

            GameManager.Instance.CursorController.UnlockCursor();            
            
            puzleCamera.m_Priority = 12;

            StartPuzle();
        }

        protected void ExitFromPuzle()
        {
            puzleCamera.Priority = 0;

            InitializePuzle();

            GameManager.Instance.CursorController.LockCursor();  

            player.playerOff = false;
            player.MakeVisible(true);
            player = null;
        }

        public bool GetEndedPuzle()
        {
            return endedPuzle;
        }

        public void SetEndedPuzle(bool value)
        {
            endedPuzle = value;
        }
    }
}
