using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DefinitiveScript;
public class MapController : MonoBehaviour
{
    private Camera cam;
    public GameObject vcam;
    public Transform playerPos;
    private Vector3 desiredPos;
    public GameObject map;
    public GameObject playerIcon;
    private bool inMap;
    private float orthographicSize;
    private int priority = -1;

    private bool activatedMap;

    private PlayerBehaviour m_Player;
    public PlayerBehaviour Player {
        get {
            if(m_Player == null) m_Player = GameManager.Instance.LocalPlayer;
            return m_Player;
        }
    }

    public CinemachineVirtualCamera virtualCamera;

    public int activePriority = 20;
    public int deactivePriority = -20;

    void Start()
    {
        CloseMap();
    }

    public void OpenMap()
    {
        activatedMap = true;
        RenderSettings.fog = false;
        virtualCamera.m_Priority = activePriority;
    }

    public void CloseMap()
    {
        activatedMap = false;
        RenderSettings.fog = true;
        virtualCamera.m_Priority = deactivePriority;
    }
}
