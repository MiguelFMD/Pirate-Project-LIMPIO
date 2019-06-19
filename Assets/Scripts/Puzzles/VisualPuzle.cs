﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DefinitiveScript
{
    public class VisualPuzle : Puzle
    {
        public int[] secretNumbers;
        public TextMeshProUGUI[] numberTexts;
        private int[] currentNumbers;

        private bool buttonBeingPressed;
        //Heredado: protected bool onPuzle;

        [SerializeField] LayerMask arrowLayer;

        private InputController m_InputController;
        public InputController InputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        private VisualPuzleSoundController m_VisualPuzleSoundController;
        public VisualPuzleSoundController VisualPuzleSoundController
        {
            get {
                if(m_VisualPuzleSoundController == null) m_VisualPuzleSoundController = GetComponent<VisualPuzleSoundController>();
                return m_VisualPuzleSoundController;
            }
        }

        public GameObject baseObj;
        private Transform baseTrans;

        public Puzle nextPuzle;

        public Animator[] arrowAnimators;

        private int numSelected;

        // Start is called before the first frame update
        void Awake()
        {
            baseTrans = baseObj.transform;
            InitializePuzle();
        }

        public override void StartPuzle()
        {
            onPuzle = true;
        }

        protected override void InitializePuzle()
        {
            currentNumbers = new int[] {0, 0, 0};
            for(int i = 0; i < currentNumbers.Length; i++)
            {
                numberTexts[i].text = currentNumbers[i].ToString();
                numberTexts[i].color = Color.white;
            }
            ChangeSelectedNumber(0, 0);
            
            buttonBeingPressed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                if(InputController.IncreaseNumber && !InputController.DecreaseNumber && !InputController.ChangeSelectedNumberRight && !InputController.ChangeSelectedNumberLeft)
                {
                    arrowAnimators[numSelected * 2].SetTrigger("PressedButton");
                    ChangeNumber(numSelected, 1);
                    VisualPuzleSoundController.PlaySelectSound();
                }
                else if(InputController.DecreaseNumber && !InputController.ChangeSelectedNumberRight && !InputController.ChangeSelectedNumberLeft)
                {
                    arrowAnimators[numSelected * 2 + 1].SetTrigger("PressedButton");
                    ChangeNumber(numSelected, -1);
                    VisualPuzleSoundController.PlaySelectSound();
                }
                else if(InputController.ChangeSelectedNumberRight && !InputController.ChangeSelectedNumberLeft)
                {
                    ChangeSelectedNumber(numSelected, numSelected + 1);
                    VisualPuzleSoundController.PlaySelectSound();
                }
                else if(InputController.ChangeSelectedNumberLeft)
                {
                    ChangeSelectedNumber(numSelected, numSelected - 1);
                    VisualPuzleSoundController.PlaySelectSound();
                }

                if(InputController.CheckNumbers)
                {
                    if(CheckNumbers()) 
                    {
                        PuzleController.VisualPuzleResolved(puzleID);
                        StartCoroutine(OpenPuzle(1.0f));
                        VisualPuzleSoundController.PlaySuccessSound();
                    }
                    else
                    {
                        VisualPuzleSoundController.PlayWrongSound();
                    }
                }
                
                if(InputController.ExitFromPuzle)
                {
                    FinishPuzle();
                }


            }
        }

        void ChangeSelectedNumber(int lastNumber, int newNumber)
        {
            if(newNumber > 2) newNumber = 0;
            else if(newNumber < 0) newNumber = 2;

            numberTexts[lastNumber].color = Color.white;
            numberTexts[newNumber].color = Color.yellow;

            numSelected = newNumber;
        }

        void ChangeNumber(int number, int increase)
        {
            currentNumbers[number] += increase;
            if(currentNumbers[number] > 9) currentNumbers[number] = 0;
            else if(currentNumbers[number] < 0) currentNumbers[number] = 9;

            numberTexts[number].text = currentNumbers[number].ToString();
        }

        IEnumerator PressArrow(float time)
        {
            yield return null;
        }

        bool CheckNumbers()
        {
            bool result = true;
            for(int i = 0; i < currentNumbers.Length; i++)
            {
                result = result && currentNumbers[i] == secretNumbers[i];
            }

            return result;
        }

        public void SetButtonBeingPressed(bool param)
        {
            buttonBeingPressed = param;
        }

        protected override void FinishPuzle()
        {
            base.FinishPuzle();
            ExitFromPuzle();
        }

        IEnumerator OpenPuzle(float time)
        {
            onPuzle = false;

            Quaternion initialRotation = baseTrans.localRotation;
            Vector3 eulerAngles = initialRotation.eulerAngles;
            Quaternion finalRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                baseTrans.localRotation = Quaternion.Slerp(initialRotation, finalRotation, elapsedTime / time);
                yield return null;
            }

            baseTrans.localRotation = finalRotation;

            endedPuzle = true;

            nextPuzle.SendInfo(player, originalCameraLocalPosition, originalCameraLocalRotation);
            nextPuzle.StartPuzle();
        }

        public void InstantlyOpenPuzle()
        {
            baseTrans.localRotation = Quaternion.Euler(baseTrans.localRotation.eulerAngles.x, baseTrans.localRotation.eulerAngles.y + 90f, baseTrans.localRotation.eulerAngles.z);
        }
    }
}
