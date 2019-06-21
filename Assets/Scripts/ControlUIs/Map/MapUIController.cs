using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefinitiveScript;

public class MapUIController : MonoBehaviour
{
    public RectTransform mapMinPositionTrans;
    public RectTransform mapMaxPositionTrans;
    private Vector2 mapMinPosition;
    private Vector2 mapMaxPosition;

    public Transform worldMinPositionTrans;
    public Transform worldMaxPositionTrans;
    private Vector3 worldMinPosition;
    private Vector3 worldMaxPosition;

    public RectTransform playerIconRect;
    public Transform playerTransform;

    public Image[] puzleItemZoneImages;

    public RectTransform boatIconRect;
    public RectTransform[] boatIconPositions;

    public GameObject map;

    // Start is called before the first frame update
    void Start()
    {
        mapMinPosition = mapMinPositionTrans.position;
        mapMaxPosition = mapMaxPositionTrans.position;

        worldMinPosition = worldMinPositionTrans.position;
        worldMaxPosition = worldMaxPositionTrans.position;

        for(int i = 0; i < puzleItemZoneImages.Length; i++)
        {
            puzleItemZoneImages[i].enabled = false;
        }
    }

    public void OpenMap()
    {
        map.SetActive(true);
        GameManager.Instance.LocalPlayer.playerOff = true;
        RecalculatePlayerIconPosition();

        Time.timeScale = 0f;
    }

    public void CloseMap()
    {
        map.SetActive(false);
        GameManager.Instance.LocalPlayer.playerOff = false;

        Time.timeScale = 1f;
    }

    void RecalculatePlayerIconPosition()
    {
        float xPercent = Mathf.InverseLerp(worldMinPosition.x, worldMaxPosition.x, playerTransform.position.x);
        float xMapPosition = Mathf.Lerp(mapMinPosition.x, mapMaxPosition.x, xPercent);

        float zPercent = Mathf.InverseLerp(worldMinPosition.z, worldMaxPosition.z, playerTransform.position.z);
        float yMapPosition = Mathf.Lerp(mapMinPosition.y, mapMaxPosition.y, zPercent);

        Vector2 newPosition = new Vector2(xMapPosition, yMapPosition);

        playerIconRect.position = newPosition;
    }

    public void EnablePuzleItemZone(int i)
    {
        puzleItemZoneImages[i].enabled = true;
    }

    public void ChangeBoatPosition(int i)
    {
        boatIconRect.position = boatIconPositions[i].position;
    }
}
