using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class PuzleController : MonoBehaviour
{
    public VisualPuzle[] visualPuzles;
    public Puzle[] puzles;

    private int resolvedPuzles;
    public int nPuzleObjective = 3;

    public GameObject rockInEntry;
    public BoxCollider entryTrigger;

    private SceneController m_SceneController;
    public SceneController SceneController
    {
        get {
            if(m_SceneController == null) m_SceneController = GameManager.Instance.SceneController;
            return m_SceneController;
        }
    }

    void Start() {
        
        resolvedPuzles = 0;

        for(int i = 0; i < visualPuzles.Length; i++)
        {
            visualPuzles[i].PuzleController = puzles[i].PuzleController = this;
            visualPuzles[i].puzleID = puzles[i].puzleID = i;

            if(SceneController.GetResolvedVisualPuzle(i))
            {
                visualPuzles[i].SetEndedPuzle(true);
                visualPuzles[i].InstantlyOpenPuzle();
            }

            if(SceneController.GetResolvedPuzle(i))
            {
                puzles[i].SetEndedPuzle(true);
                resolvedPuzles++;
            }
        }

        if(SceneController.GetOpennedCavern())
        {
            OpenCavern();
        }
    }

    void Update() {
        if(Input.GetKey(KeyCode.H) && Input.GetKeyDown(KeyCode.K))
        {
            for(int i = 0; i < 3; i++)
            {
                VisualPuzleResolved(i);
                PuzleResolved(i);
            }
        }
    }

    public void VisualPuzleResolved(int i)
    {
        SceneController.ResolvedVisualPuzle(i);
    }

    public void PuzleResolved(int i)
    {
        SceneController.ResolvedPuzle(i);

        resolvedPuzles++;
        if(resolvedPuzles == nPuzleObjective)
        {
            OpenCavern();
            SceneController.OpennedCavern();
        }
    }

    private void OpenCavern()
    {
        print("Open cavern");
        rockInEntry.SetActive(false);
        entryTrigger.enabled = true;
    }
}
