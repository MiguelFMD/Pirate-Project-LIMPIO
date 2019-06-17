﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace DefinitiveScript 
{
    public class SceneController : MonoBehaviour
    {
        private bool created;
        private PlayerBehaviour PlayerBehaviour;
        private Transform BoatTransform;
        private GameObject GM;

        public Image blackScreen;
        public TextMeshProUGUI deathText;

        public float fadingTime = 0.5f;

        private int lastScene;
        private int dockID;
        
        private bool changedScene = false;

        private bool resolvedPuzles = false;

        private DockController[] BoatDocks;
        private DockController[] IslandDocks;

        private Transform boatInitialPoint;
        private Transform exitFromCavernSpawnPoint;
        private Transform enterIntoCavernSpawnPoint;

        private int mainMenuID = 0;
        private int boatSceneID = 1;
        private int islandSceneID = 2;
        private int cavernSceneID = 3;

        private GeneralSoundController soundController;

        void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneController");

            if(objs.Length > 1 && !created)
            {
                Destroy(this.gameObject);
            }

            if(!created) lastScene = -1;

            DontDestroyOnLoad(GameManager.Instance.GameObject);
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += InitializeScene;

            created = true;
        }

        void Start()
        {
            GameManager.Instance.SceneController = this;

            if(SceneManager.GetActiveScene().buildIndex == mainMenuID) GameManager.Instance.CursorController.UnlockCursor();
            else GameManager.Instance.CursorController.LockCursor();

            blackScreen.enabled = false;
            InitializeScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        void Update() {
            /*if(changedScene)
            {
                InitializeScene();
                changedScene = false;
            }*/
        }

        private void FindPlayer()
        {
            GameObject aux = GameObject.Find("Player");
            if(aux != null)
            {
                PlayerBehaviour = aux.GetComponent<PlayerBehaviour>();
                
                PlayerBehaviour.enabled = true;
            }
        }

        private void FindBoat()
        {
            GameObject aux = GameObject.Find("Boat");
            if(aux != null) BoatTransform = aux.transform;
        }

        private void FindBoatInitialPoint()
        {
            GameObject aux = GameObject.Find("BoatInitialPoint");
            if(aux != null) boatInitialPoint = aux.transform;
        }

        private void FindBoatDocks()
        {
            BoatDocks = FindObjectsOfType<DockController>();

            /*print(aux.Length);
            
            BoatDocks = new DockController[aux.Length/2];

            int j = 0;
            for(int i = 0; i < aux.Length; i++)
            {
                if(aux[i].enteringIsland) 
                {
                    BoatDocks[j] = aux[i];
                    j++;
                }
            }*/
        }

        private void FindIslandDocks()
        {
            IslandDocks = FindObjectsOfType<DockController>();
            
            /*IslandDocks = new DockController[aux.Length/2];

            int j = 0;
            for(int i = 0; i < aux.Length; i++)
            {
                if(!aux[i].enteringIsland) 
                {
                    IslandDocks[j] = aux[i];
                    j++;
                }
            }*/
        }

        private void FindExitCavernSpawnPoint()
        {
            GameObject aux = GameObject.Find("ExitFromCavernSpawnPoint");
            if(aux != null) exitFromCavernSpawnPoint = aux.transform;
        }

        private void FindEnterCavernSpawnPoint()
        {
            GameObject aux = GameObject.Find("EnterIntoCavernSpawnPoint");
            if(aux != null) enterIntoCavernSpawnPoint = aux.transform;
        }

        private void InitializeScene(Scene scene, LoadSceneMode mode)
        {
            deathText.enabled = false;        

            StartCoroutine(InitializeSceneCoroutine(scene));

            soundController = GameObject.FindWithTag("SoundController").GetComponent<GeneralSoundController>();
        }

        private IEnumerator InitializeSceneCoroutine(Scene scene)
        {
            if(lastScene != -1)
            {
                if(scene.buildIndex == mainMenuID)
                {
                    GameManager.Instance.CursorController.UnlockCursor();
                    yield return StartCoroutine(FadeIn(fadingTime));
                }
                else if(scene.buildIndex == boatSceneID)
                {
                    GameManager.Instance.CursorController.LockCursor();

                    FindBoatInitialPoint();
                    FindBoat();
                    FindBoatDocks();

                    if(lastScene == mainMenuID)
                    {
                        BoatTransform.position = boatInitialPoint.position;
                    }
                    else if(lastScene == islandSceneID)
                    {
                        Vector3 dockDestinationPoint = Vector3.zero;
                        for(int i = 0; i < BoatDocks.Length; i++)
                        {
                            if(BoatDocks[i].dockID == dockID)
                            {
                                dockDestinationPoint = BoatDocks[i].boatSpawnPoint.position;
                                break;
                            } 
                        }
                        BoatTransform.position = dockDestinationPoint;
                    }
                    
                    yield return StartCoroutine(FadeIn(fadingTime));
                }
                else if(scene.buildIndex == islandSceneID)
                {
                    GameManager.Instance.CursorController.LockCursor();
                    FindPlayer();
                    FindIslandDocks();
                    FindExitCavernSpawnPoint();

                    PlayerBehaviour.stopInput = true;
                    if(lastScene == boatSceneID)
                    {
                        Vector3 dockDestinationPoint = Vector3.zero;
                        for(int i = 0; i < IslandDocks.Length; i++)
                        {
                            if(IslandDocks[i].dockID == dockID)
                            {
                                dockDestinationPoint = IslandDocks[i].playerSpawnPoint.position;
                                break;
                            } 
                        }
                        PlayerBehaviour.transform.position = dockDestinationPoint;
                    }
                    else if(lastScene == cavernSceneID)
                    {
                        PlayerBehaviour.transform.position = exitFromCavernSpawnPoint.position;
                    }

                    yield return StartCoroutine(FadeIn(fadingTime));

                    PlayerBehaviour.stopInput = false;
                }
                else if(scene.buildIndex == cavernSceneID)
                {
                    GameManager.Instance.CursorController.LockCursor();
                    FindPlayer();
                    FindEnterCavernSpawnPoint();

                    if(lastScene == islandSceneID || lastScene == cavernSceneID)
                    {   
                        PlayerBehaviour.stopInput = true;

                        PlayerBehaviour.transform.position = enterIntoCavernSpawnPoint.position;

                        yield return StartCoroutine(FadeIn(fadingTime));

                        PlayerBehaviour.stopInput = false;
                    }
                }
            }
            
        }

        private IEnumerator ChangeToScene(int sceneID)
        {
            soundController.FadeOutTheme();

            lastScene = SceneManager.GetActiveScene().buildIndex;
            changedScene = true;
            yield return StartCoroutine(FadeOut(fadingTime));

            SceneManager.LoadScene(sceneID);
        }

        public void BackToMenu()
        {
            StartCoroutine(ChangeToScene(mainMenuID));
        }

        public void StartGame()
        {
            StartCoroutine(ChangeToScene(boatSceneID));
        }

        public void DockTheBoat(int dockID)
        {
            this.dockID = dockID;

            StartCoroutine(ChangeToScene(islandSceneID));
        }

        public void ToSail(int dockID)
        {
            this.dockID = dockID;

            StartCoroutine(ChangeToScene(boatSceneID));
        }

        public void EnterIntoTheCavern()
        {
            StartCoroutine(ChangeToScene(cavernSceneID));
        }

        public void ExitFromTheCavern()
        {
            StartCoroutine(ChangeToScene(islandSceneID));
        }

        public void ShowDeathText()
        {
            deathText.enabled = true;
            StartCoroutine(FadeInDeathText(1.5f));
        }

        private IEnumerator FadeInDeathText(float time)
        {
            Color c = deathText.color;
            c.a = 0f;
            deathText.color = c;

            float initialAlpha = 0f;
            float finalAlpha = 1f;

            RectTransform deathTextTransform = deathText.GetComponent<RectTransform>();

            Vector3 finalScale = deathTextTransform.localScale;
            Vector3 initialScale = finalScale * 0.5f;

            deathText.GetComponent<RectTransform>().localScale = initialScale;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                deathText.color = c;

                deathTextTransform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / time);

                yield return null;
            }

            c.a = finalAlpha;
            deathText.color = c;

            deathTextTransform.localScale = finalScale;
        }

        public void PlayerDead()
        {
            StartCoroutine(ChangeToScene(SceneManager.GetActiveScene().buildIndex));
        }

        /*public void ChangeToScene(string name)
        {
            StartCoroutine(TransitionToScene(fadingTime, name));
        }

        public void ChangeToScene(int id)
        {
            StartCoroutine(TransitionToScene(fadingTime, id));
        }

        private IEnumerator TransitionToScene(float time, string name)
        {
            yield return StartCourutine(FadeOut(time));
            SceneManager.LoadScene(name);
            yield return StartCoroutine(FadeIn(time));
        }

        private IEnumerator TransitionToScene(float time, int id)
        {
            yield return StartCourutine(FadeOut(time));
            SceneManager.LoadScene(id);
            yield return StartCoroutine(FadeIn(time));
        }*/

        private IEnumerator FadeOut(float time)
        {
            blackScreen.enabled = true;
            float initialAlpha = 0f;
            float finalAlpha = 1f;

            float elapsedTime = 0.0f;

            Color c = blackScreen.color;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                blackScreen.color = c;

                yield return null;
            }
            c.a = finalAlpha;
            blackScreen.color = c;
        }

        private IEnumerator FadeIn(float time)
        {
            float initialAlpha = 1f;
            float finalAlpha = 0f;

            float elapsedTime = 0.0f;

            Color c = blackScreen.color;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                blackScreen.color = c;

                yield return null;
            }

            c.a = finalAlpha;
            blackScreen.color = c;
            blackScreen.enabled = false;
        }

        public bool GetResolvedPuzles()
        {
            return resolvedPuzles;
        }
        
        public void SetResolvedPuzles(bool value)
        {
            resolvedPuzles = value;
        }
    }
}