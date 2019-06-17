﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [System.Serializable]
        public class MouseInput
        {
            public Vector2 Sensitivity; //Sensibilidad del ratón para rotar la cámara en el modo sable
        }

        [SerializeField] MouseInput MouseControl;

        [SerializeField] Vector3 sableCameraOffset; //Separación de la cámara con respecto al objetivo de apuntado en el modo sable
        [SerializeField] Vector3 gunCameraOffset;   //Separación de la cámara con respecto al objetivo de apuntado en el modo pistola
        [SerializeField] float damping;             //Capacidad de la cámara para llegar a la posición y rotación indicada

        Transform cameraLookTarget;                 //Objetivo al que apunta la cámara
        Transform sableCameraPosition;              //Posición del objetivo en el modo sable
        Transform gunCameraPosition;                //Posición del objetivo en el modo pistola
        PlayerBehaviour localPlayer;                         //Instancia del jugador
        
        public Transform cameraBase;                //Base localizada sobre el jugador alrededor de la cual orbitará la cámara en el modo sable

        InputController cameraInput;                //Instancia del InputController para detectar el movimiento del ratón en el modo sable

        private bool initialized = false;           //Indica si la cámara ha sido inicializada o no para activar el movimiento de la misma en el método Update

        private bool m_MovementMode;                //Indica el modo de movimiento que se está ejecutando: true equivale a modo pistola y false a modo sable
        public bool movementMode
        {
            get
            {
                return m_MovementMode;
            }
            set
            {
                m_MovementMode = value;
                StartCoroutine(ModeTransition(0.1f)); //En el momento que se realiza un cambio de valor, se debe ejecutar una transición en la posición y rotación de la cámara para recolocarla en la posición adecuada

                /* if(value) print("Gun mode, la camera no es hija de nadie");
                else print("Sable mode, la camera es hija de la cameraBase");*/
            }
        }

        private Vector3 dollyDir;
        public float minSableCameraOffsetZ;
        [SerializeField] LayerMask cameraCollisionMask;


        void Awake()
        {
            //GameManager.Instance.Camera = this; //Se le indica al GameManager la instancia de la cámara
            cameraInput = GameManager.Instance.InputController; //Se guarda la instancia del InputController necesaria para la cámara
        }

        public void InitializeCamera() //Método que inicializa lo necesario de la cámara y que será llamado desde el GameManager
        {
            dollyDir = transform.localPosition.normalized;

            localPlayer = GameManager.Instance.LocalPlayer;

            //localPlayer.CameraTransform = transform; //El jugador necesita las propiedades físicas de la cámara para conocer su orientación en el modo sable

            cameraLookTarget = localPlayer.transform.Find("cameraLookTarget");
            sableCameraPosition = localPlayer.transform.Find("sableCameraPosition");
            gunCameraPosition = localPlayer.transform.Find("gunCameraPosition");

            if(cameraLookTarget == null)
                cameraLookTarget = localPlayer.transform;

            if(sableCameraPosition == null)
                sableCameraPosition = localPlayer.transform;

            if(gunCameraPosition == null)
                gunCameraPosition = localPlayer.transform;

            initialized = true;

            StartCoroutine(ModeTransition(0.0f)); //Se realiza una primera transción para colocar la cámara en el lugar correcto inicialmente
        }

        public void SetInitialized(bool param)
        {
            initialized = param;
        }

        private IEnumerator ModeTransition(float time) //Corutina que permite transicionar la posición de la cámara de un modo de movimiento al otro
        {
            initialized = false; //La cámara no se moverá por el método Update durante la transición
            localPlayer.stopMovement = true; //El jugador no podrá moverse durante la transición

            float elapsedTime = 0.0f;

            Vector3 targetInitialPosition, targetFinalPosition;
            if(movementMode) //El modo al que se ha cambiado es el modo pistola
            {
                targetInitialPosition = sableCameraPosition.position;
                targetFinalPosition = gunCameraPosition.position;
                transform.parent = null; //La cámara no ha de ser hija de ningún otro objeto en el modo pistola

                //Nos interesa que la base de la cámara se recoloque en la orientación en Y del jugador para que al volver al modo sable no esté en una orientación no deseada
                cameraBase.rotation = Quaternion.Euler(cameraBase.eulerAngles.x, localPlayer.transform.eulerAngles.y, cameraBase.eulerAngles.z);
            }
            else //El modo al que se ha cambiado es el modo sable
            {
                targetInitialPosition = gunCameraPosition.position;
                targetFinalPosition = sableCameraPosition.position;
                transform.parent = cameraBase; //La cámara girará en torno a esta base en el modo sable
            }

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                //Realmente se modifica la posición del objetivo de la cámara
                //La separación de la cámara con respecto a dicho objetivo se establece una vez se "libera" el método Update
                cameraLookTarget.position = Vector3.Lerp(targetInitialPosition, targetFinalPosition, elapsedTime / time); 

                yield return null;
            }
            cameraLookTarget.position = targetFinalPosition;

            initialized = true; //Se "libera" el método Update
            localPlayer.stopMovement = false; 
        }

        // Update is called once per frame
        void Update()
        {
            if(initialized) //Mientras esta variable esté en false, este método queda inutilizado
            {
                /*Vector3 targetPosition;
                if(movementMode) //Modo pistola
                {
                    //Se calcula la posición objetivo en función de la posición del objetivo, la orientación del jugador y la separación de la cámara con respecto al objetivo
                    targetPosition = cameraLookTarget.position + localPlayer.transform.forward * gunCameraOffset.z +
                                        localPlayer.transform.up * gunCameraOffset.y +
                                        localPlayer.transform.right * gunCameraOffset.x;

                    Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget.position - targetPosition, Vector3.up); //Se calcula una orientación en función de la posición del objetivo y esa posición objetivo

                    transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime); //La cámara se mueve hacia esa posición objetivo
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping * Time.deltaTime); //La cámara gira hasta esa rotación objetivo
                    cameraBase.rotation = Quaternion.Slerp(cameraBase.rotation, localPlayer.transform.rotation, damping * Time.deltaTime); //Nos interesa que la base de la cámara rote en Y con el personaje para una vez se pase al modo sable
                }
                else //Modo sable
                {
                    float zDistance = CalculateCameraCollision();
                    //Se calcula la posición objetivo en función de la posición de la base de la cámara, la orientación de la misma y la separación de la cámara con respecto a esta base
                    targetPosition = cameraBase.position + cameraBase.forward * zDistance +
                                        cameraBase.up * sableCameraOffset.y +
                                        cameraBase.right * sableCameraOffset.x;

                    transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime); //Se mueve la cámara hacia esa posición
                    cameraBase.Rotate(Vector3.up * cameraInput.MouseInput.x * MouseControl.Sensitivity.x); //Es la base la que rota, haciendo orbitar a su alrededor a la cámara
                }
                targetPosition = cameraLookTarget.position; //Se guarda como posición objetivo la posición del objetivo

                cameraBase.position = Vector3.Lerp(cameraBase.position, targetPosition, damping * Time.deltaTime);  //La base de la cámara se mueve hacia esa posición
*/
                if (Input.GetKeyDown (KeyCode.End))
                {
                    Cursor.lockState = (Cursor.lockState != CursorLockMode.Locked) ? CursorLockMode.Confined : CursorLockMode.Locked;
                }
            }
        }

        float CalculateCameraCollision()
        {
            Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * Mathf.Abs(sableCameraOffset.z));
            float distance;

            RaycastHit hit;

            if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, cameraCollisionMask))
            {
                distance = - Mathf.Clamp (hit.distance * 0.8f, Mathf.Abs(minSableCameraOffsetZ), Mathf.Abs(sableCameraOffset.z));
            }
            else
            {
                distance = sableCameraOffset.z;
            }

            return distance;
        }

        public IEnumerator MoveCameraTo(float time, Vector3 finalPosition, Quaternion finalRotation)
        {
            initialized = false;

            transform.parent = null;

            Vector3 initialPosition = transform.position;
            Quaternion initialRotation = transform.rotation;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / time);
                transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, elapsedTime / time);
                yield return null;
            }
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }

        public IEnumerator ReturnCameraToLastPosition(float time, Vector3 finalPosition, Quaternion finalRotation, bool movementMode)
        {
            transform.parent = movementMode ? null : cameraBase;
            
            Vector3 initialPosition = transform.localPosition;
            Quaternion initialRotation = transform.localRotation;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / time);
                transform.localRotation = Quaternion.Slerp(initialRotation, finalRotation, elapsedTime / time);
                yield return null;
            }
            transform.localPosition = finalPosition;
            transform.localRotation = finalRotation;

            initialized = true;
        }
    }
}