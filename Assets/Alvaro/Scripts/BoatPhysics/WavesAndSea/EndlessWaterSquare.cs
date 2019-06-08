﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EndlessWaterSquare : MonoBehaviour
{
    //The object the water will follow
    public GameObject boatObj;
    //One water square
    public GameObject waterSqrObj;
    public GameObject bottomSqrObj;

    //Water square data
    private float squareWidth = 800f;
    private float innerSquareResolution = 5f;
    private float outerSuqareResolution = 25f;

    //The list with all water mesh squares == the entire ocean we can see
    List<WaterSquare> waterSquares = new List<WaterSquare>();
    List<WaterSquare> seaBottoms = new List<WaterSquare>();

    //Stuff needed for the thread
    //The timer that keeps track of seconds since start to update the water because we cant use Time.time in a thread
    float secondsSinceStart;
    //The position of the boat
    Vector3 boatPos;
    //The position of the ocean has to be updated in the thread because it follows the boat
    //Is not the same as pos of boat because it moves with the same resolution as the smallest water square resolution
    Vector3 oceanPos;
    //Has the thread finished updating the water so we can add the stuff from the thread to the main thread
    bool hasThreadUpdateWater;

    public float seaBottomDepth = 5f;

    // Start is called before the first frame update
    void Start()
    {
        CreateEndlessSea();

        secondsSinceStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaterNoThread();

        //Update these as often as possible because we don't know when the thread will run because of pooling
        //and we always need the lastest version

        //Update the time since start to get correct wave height which depends on time since start

        secondsSinceStart = Time.time;

        boatPos = boatObj.transform.position;
    }

    //Update the water with no thread to compare
    void UpdateWaterNoThread()
    {
        //Update the position of the boat
        boatPos = boatObj.transform.position;

        //Move the water to the boat
        MoveWaterToBoat();

        //Add the new position of the ocean to this transform
        transform.position = oceanPos;

        //Update the vertices
        for(int i = 0; i < waterSquares.Count; i++)
        {
            waterSquares[i].MoveSea(oceanPos, Time.time);
            seaBottoms[i].squareTransform.position = new Vector3(seaBottoms[i].squareTransform.position.x, oceanPos.y - seaBottomDepth, seaBottoms[i].squareTransform.position.z);
        }
    }

    //The loop that gives the updated vertices from the thread to the meshes
    //which we can't do in its own thread
    /*IEnumerator UpdateWater()
    {
        while(true)
        {
            //Has the thread finished updating the water?
            if(hasThreadUpdateWater)
            {
                //Move the water to the boat
                transform.position = oceanPos;

                //Add the updated vertices to the water meshes
                for(int i = 0; i < waterSquares.Count; i++)
                {
                    waterSquares[i].terrainMeshFilter.mesh.vertices = waterSquares[i].vertices;
                    seaBottoms[i].terrainMeshFilter.mesh.vertices = seaBottoms[i].vertices;

                    waterSquares[i].terrainMeshFilter.mesh.RecalculateNormals();
                    ///seaBottoms[i].terrainMeshFilter.mesh.RecalculateNormals();
                }

                //Stop looping until we have updated the water in the thread
                hasThreadUpdateWater = false;

                //Update the water in the thread
                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));
            }

            //Don't need to update the water every frame
            yield return new WaitForSeconds(Time.deltaTime * 3f);
        }
    }*/

    //Move the endless water to the boat's position in steps that's the same as the water's resolution
    void MoveWaterToBoat()
    {
        //Round to nearest resolution
        float x = innerSquareResolution * (int)Mathf.Round(boatPos.x / innerSquareResolution);
        float z = innerSquareResolution * (int)Mathf.Round(boatPos.z / innerSquareResolution);

        //Should we move the water?
        if(oceanPos.x != x || oceanPos.z != z)
        {
            //print("Moved sea");

            oceanPos = new Vector3(x, oceanPos.y, z);
        }
    }

    //Init the endless sea by creating all squares
    void CreateEndlessSea()
    {
        //The center piece
        AddWaterPlane(0f, 0f, 0f, squareWidth, innerSquareResolution, innerSquareResolution);

        //The 8 squares around the center square
        for(int x = -1; x <= 1; x += 1)
        {
            for(int z = -1; z <= 1; z += 1)
            {
                //Ignore the center pos
                if(x == 0 && z == 0) continue;

                //The y-Pos should be lower than the square with high resolution to avoid an ugly seam
                float yPos = -0.5f;
                AddWaterPlane(x * squareWidth, z * squareWidth, yPos, squareWidth, outerSuqareResolution, outerSuqareResolution);
            }
        }
    }

    //Add one water plane
    void AddWaterPlane(float xCoord, float zCoord, float yPos, float squareWidth, float waterSpacing, float bottomSpacing)
    {
        GameObject waterPlane = Instantiate(waterSqrObj, transform.position, transform.rotation) as GameObject;
        GameObject seaBottom = Instantiate(bottomSqrObj, transform.position, transform.rotation) as GameObject;

        waterPlane.SetActive(true);

        //Change its position
        Vector3 centerPos = transform.position;

        centerPos.x += xCoord;
        centerPos.y = yPos;
        centerPos.z += zCoord;

        waterPlane.transform.position = centerPos;

        centerPos.x = xCoord;
        centerPos.y -= seaBottomDepth;
        centerPos.z = zCoord;

        seaBottom.transform.localPosition = centerPos;

        //Parent it
        waterPlane.transform.parent = transform;
        seaBottom.transform.parent = transform;
        //Give it moving water properties and set its width and resolution to generate the water mesh
        WaterSquare newWaterSquare = new WaterSquare(waterPlane, squareWidth, waterSpacing);
        WaterSquare newSeaBottom = new WaterSquare(seaBottom, squareWidth, bottomSpacing);

        waterSquares.Add(newWaterSquare);
        seaBottoms.Add(newSeaBottom);
    }
}
