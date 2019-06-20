using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public float playerHealth;
    public int playerMoney;
    public bool playerHasKey;

    public bool[] resolvedVisualPuzles;
    public bool[] resolvedPuzles;
    public bool opennedCavern;

    public GameData()
    {
        Reset();
    }

    public void Reset()
    {
        playerHealth = 100f;
        playerMoney = 0;
        playerHasKey = false;

        resolvedVisualPuzles = new bool[3];
        resolvedPuzles = new bool[3];
        opennedCavern = false;
    }
}
