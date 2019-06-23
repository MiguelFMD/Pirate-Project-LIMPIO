using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefinitiveScript;

public class BoatEngine : MonoBehaviour
{
    private InputController m_InputController;
    public InputController InputController
    {
        get {
            if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
            return m_InputController;
        }
    }

    //Drags
    public Transform waterJetTransform;
    public Transform sailObjectTransform;
    public BoatUIController BoatUIController;

    //How fast should the engine accelerate?
    public float powerFactor;

    //What's the boat's maximum engine power?
    public float maxPower;

    public float steerVelocity = 2f;
    public float waterJetMaxAngle = 15f;
    public float sailMaxAngle = 60f;

    //The boat's current engine power is public for debugging
    public float currentJetPower;

    private float thrustFromWaterJet = 0f;

    private Rigidbody boatRB;

    private float WaterJetRotation_Y = 0f;
    private float SailRotation_Y = 0f;

    BoatController boatController;
    BoatSoundController boatSoundController;

    private Vector3 originalPosition;
    private Vector3 originalRotation;

    private float conversionFactor;

    private Coroutine restartSailCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        boatRB = GetComponent<Rigidbody>();

        boatController = GetComponent<BoatController>();
        boatSoundController = GetComponent<BoatSoundController>();

        originalPosition = transform.position;
        originalRotation = transform.localEulerAngles;

        conversionFactor = BoatUIController.rudderMaxAngle / waterJetMaxAngle;
    }

    // Update is called once per frame
    void Update()
    {
        UserInput();
    }

    void FixedUpdate()
    {
        UpdateWaterJet();
    }

    void UserInput()
    {
        //Forward / reverse
        if(InputController.Vertical > 0f)
        {
            if(boatController.CurrentSpeed < 50f && currentJetPower < maxPower)
            {
                currentJetPower += 1f * powerFactor;
            }
        }
        else
        {
            if(currentJetPower > 0f)
            {  
                currentJetPower -= 1f * powerFactor;
            }
        }

        boatSoundController.ChangeSailingSoundVolume(currentJetPower / maxPower);

        //Steer left
        if(InputController.Horizontal < 0f)
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y + steerVelocity * Time.deltaTime;

            if(WaterJetRotation_Y > 180f + waterJetMaxAngle)
            {
                WaterJetRotation_Y = 180f + waterJetMaxAngle;
            }

            if(restartSailCoroutine != null)
            {
                StopCoroutine(restartSailCoroutine);
                restartSailCoroutine = null;
                sailObjectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }

            SailRotation_Y = (WaterJetRotation_Y - 180f) * sailMaxAngle / waterJetMaxAngle;

            waterJetTransform.localEulerAngles = new Vector3(0f, WaterJetRotation_Y, 0f);
            sailObjectTransform.localEulerAngles = new Vector3(0f, SailRotation_Y, 0f);
            
            BoatUIController.UpdateRudder(steerVelocity * Time.deltaTime * conversionFactor);
        }
        //Steer right
        else if(InputController.Horizontal > 0f)
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y - steerVelocity * Time.deltaTime;

            if(WaterJetRotation_Y < 180f - waterJetMaxAngle)
            {
                WaterJetRotation_Y = 180f - waterJetMaxAngle;
            }

            if(restartSailCoroutine != null)
            {
                StopCoroutine(restartSailCoroutine);
                restartSailCoroutine = null;
                sailObjectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }

            SailRotation_Y = (WaterJetRotation_Y - 180f) * sailMaxAngle / waterJetMaxAngle;

            waterJetTransform.localEulerAngles = new Vector3(0f, WaterJetRotation_Y, 0f);
            sailObjectTransform.localEulerAngles = new Vector3(0f, SailRotation_Y, 0f);

            BoatUIController.UpdateRudder(-steerVelocity * Time.deltaTime * conversionFactor);
        }
        else if(InputController.CenterRudder)
        {
            WaterJetRotation_Y = 180f;
            
            restartSailCoroutine = StartCoroutine(RestartSailCoroutine(0.3f));
            SailRotation_Y = 0;

            waterJetTransform.localEulerAngles = new Vector3(0f, WaterJetRotation_Y, 0f);
            BoatUIController.RestartRudder();
        }

        if(Input.GetKey(KeyCode.R)) //DevHack
        {
            RestartBoat();
            BoatUIController.RestartUI();
        }

        if(InputController.Horizontal != 0f) boatSoundController.PlayTurningSailSound();
        if(InputController.Horizontal == 0f) boatSoundController.StopTurningSailSound();

        BoatUIController.UpdateCompass(transform.TransformDirection(Vector3.forward));
    }

    void RestartBoat()
    {
        transform.position = originalPosition;
        transform.localEulerAngles = originalRotation;

        WaterJetRotation_Y = 180f;
        SailRotation_Y = 0;

        waterJetTransform.localEulerAngles = new Vector3(0f, WaterJetRotation_Y, 0f);
        sailObjectTransform.localEulerAngles = new Vector3(0f, SailRotation_Y, 0f);

        currentJetPower = 0f;
    }

    IEnumerator RestartSailCoroutine(float time)
    {
        boatSoundController.PlayTurningSailSound();

        float initialRotation = SailRotation_Y;
        float finalRotation = 0f;
        float newRotation = 0f;

        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            newRotation = Mathf.Lerp(initialRotation, finalRotation, elapsedTime / time);
            sailObjectTransform.localEulerAngles = new Vector3(0f, newRotation, 0f);

            yield return null;
        }
        sailObjectTransform.localEulerAngles = new Vector3(0f, finalRotation, 0f);

        boatSoundController.StopTurningSailSound();
    }

    void UpdateWaterJet()
    {
        //Debug.Log(boatController.CurrentSpeed);

        Vector3 forceToAdd = -waterJetTransform.forward * currentJetPower;

        Debug.DrawRay(waterJetTransform.position, waterJetTransform.forward * 3f, Color.magenta);

        //Only add the force if the is below sea level
        float waveYPos= WaterController.current.GetWaveYPos(waterJetTransform.position, Time.time);

        if(waterJetTransform.position.y < waveYPos)
        {
            boatRB.AddForceAtPosition(forceToAdd, waterJetTransform.position);
        }
        else
        {
            boatRB.AddForceAtPosition(Vector3.zero, waterJetTransform.position);
        }
    }
}
