﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefinitiveScript;
public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadScene()
    {
        GameManager.Instance.SceneController.StartGame();
    }
}
