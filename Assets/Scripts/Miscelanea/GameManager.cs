using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class GameManager
    {
        private GameObject gameObject;
        public GameObject GameObject
        {
            get {
                return gameObject;
            }
        }

        private static GameManager m_Instance; //Instancia estática del GameManager. Es singleton
        public static GameManager Instance {
            get{
                if(m_Instance == null)
                {
                    m_Instance = new GameManager();
                    m_Instance.gameObject = new GameObject("_gameManager");

                    m_Instance.gameObject.AddComponent<CursorController>();
                    m_Instance.gameObject.AddComponent<InputController>();
                }

                return m_Instance;
            }
        }

        private InputController m_InputController; //Instancia del InputController
        public InputController InputController {
            get{
                if(m_InputController == null)
                {
                    m_InputController = m_Instance.gameObject.GetComponent<InputController>();
                }
                return m_InputController;
            }
        }

        private PlayerBehaviour m_LocalPlayer; //Instancia del Player
        public PlayerBehaviour LocalPlayer {
            get {
                if(m_LocalPlayer == null) {
                    GameObject aux = GameObject.Find("Player");

                    if(aux != null) m_LocalPlayer = aux.GetComponent<PlayerBehaviour>();
                }
                return m_LocalPlayer;
            }
        }

        private CursorController m_CursorController;
        public CursorController CursorController
        {
            get {
                if(m_CursorController == null) m_CursorController = m_Instance.gameObject.GetComponent<CursorController>();
                return m_CursorController;
            }
        }

        private SceneController m_SceneController;
        public SceneController SceneController {
            get {
                return m_SceneController;
            }

            set {
                m_SceneController = value;
            }
        }
    }
}

