using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class SimonSays : Puzle
    {
        public GameObject[] portions; //0: Red; 1: Blue; 2: Green; 3: Yellow

        private Material[] portionMaterials;

        public string[] colorSequences;
        public float timeBetweenLights;

        private bool playerTurn;
        private bool onSequence;
        private int currentSequence;
        private int currentWaitingColor;

        private GameObject pointedPortion;

        [SerializeField] LayerMask portionMask;

        private InputController m_InputController;
        public InputController InputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        private SimonSaysSoundController m_SimonSaysSoundController;
        public SimonSaysSoundController SimonSaysSoundController
        {
            get {
                if(m_SimonSaysSoundController == null) m_SimonSaysSoundController = GetComponent<SimonSaysSoundController>();
                return m_SimonSaysSoundController; 
            }
        }

        //Heredado: protected bool onPuzle;

        void Awake()
        {
            portionMaterials = new Material[portions.Length];
            for(int i = 0; i < portions.Length; i++)
            {
                portionMaterials[i] = portions[i].GetComponent<MeshRenderer>().material;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            InitializePuzle();
        }

        protected override void InitializePuzle()
        {
            playerTurn = false;
            onSequence = false;
            pointedPortion = null;
            currentSequence = 0;
            currentWaitingColor = 0;
        }

        public override void StartPuzle()
        {
            onPuzle = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                if(!onSequence)
                {
                    if(playerTurn)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if(Physics.Raycast(ray, out hit, Mathf.Infinity, portionMask))
                        {
                            if(pointedPortion == null || (pointedPortion != null && pointedPortion != hit.collider.gameObject))
                            {
                                if(pointedPortion != null) pointedPortion.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                                pointedPortion = hit.collider.gameObject;
                                pointedPortion.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                            }
                        }
                        else if(pointedPortion != null)
                        {
                            pointedPortion = null;
                            for(int i = 0; i < portionMaterials.Length; i++)
                            {
                                portionMaterials[i].DisableKeyword("_EMISSION");
                            }
                        }

                        if(InputController.Shooting && pointedPortion != null)
                        {
                            int i = GetPortionID(pointedPortion);
                            if(i != -1) StartCoroutine(CheckSelectedPortion(i));
                        }
                        
                    }
                    else
                    {
                        StartCoroutine(ReproduceSequence(colorSequences[currentSequence]));
                        onSequence = true;
                    }
                }

                if(InputController.ExitFromPuzle)
                {
                    FinishPuzle();
                }
            }
        }

        int GetPortionID(GameObject portion)
        {
            for(int i = 0; i < portions.Length; i++)
            {
                if(portion == portions[i]) return i;
            }
            return -1;
        }

        IEnumerator ReproduceSequence(string sequence)
        {
            for(int i = 0; i < sequence.Length; i++)
            {
                int j = int.Parse(sequence[i].ToString());

                Material portionMaterial = portionMaterials[j];
                
                SimonSaysSoundController.PlaySimonSound(j);

                yield return StartCoroutine(BrightPortionForTime(portionMaterial, timeBetweenLights));
            }

            onSequence = false;
            playerTurn = true;
            currentWaitingColor = 0;
        }

        IEnumerator BrightPortionForTime(Material portionMaterial, float time)
        {
            portionMaterial.EnableKeyword("_EMISSION");

            yield return new WaitForSeconds(time);

            portionMaterial.DisableKeyword("_EMISSION");

            yield return new WaitForSeconds(time);
        }

        IEnumerator CheckSelectedPortion(int i)
        {
            onSequence = true;

            bool correct = int.Parse(colorSequences[currentSequence][currentWaitingColor].ToString()) == i;

            if(correct) SimonSaysSoundController.PlaySimonSound(i);
            else SimonSaysSoundController.PlayWrongSound();

            yield return StartCoroutine(BrightPortionForTime(portionMaterials[i], timeBetweenLights));
            if(correct)
            {
                currentWaitingColor++;
                if(currentWaitingColor == colorSequences[currentSequence].Length)
                {
                    currentSequence++;
                    playerTurn = false;
                    if(currentSequence == colorSequences.Length) 
                    {
                        endedPuzle = true;
                        PuzleController.PuzleResolved(puzleID);
                        SimonSaysSoundController.PlaySuccessSound();
                        FinishPuzle();
                    }
                }
            }
            else
            {
                playerTurn = false;
            }

            onSequence = false;
        }

        protected override void FinishPuzle()
        {
            base.FinishPuzle();

            ExitFromPuzle();
        }
    }
}
