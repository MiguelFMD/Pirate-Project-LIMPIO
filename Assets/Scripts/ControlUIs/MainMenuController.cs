using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;

    public Image titleImage;
    public RectTransform pressText;

    void Start()
    {
        titleImage.color = new Color(1, 1, 1, 0);
        pressText.gameObject.SetActive(false);

        StartCoroutine(Intro());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("MouseLeftClick"))
        {
            OpenMainMenu();
        }
    }

    private IEnumerator Intro()
    {
        yield return StartCoroutine(FadeInTitle(2.0f));

        pressText.gameObject.SetActive(true);
    }

    private IEnumerator FadeInTitle(float time)
    {
        Color c = titleImage.color;

        float initialAlpha = 0f;
        float finalAlpha = 1f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
            titleImage.color = c;

            yield return null;
        }
        c.a = finalAlpha;
        titleImage.color = c;
    }

    private void OpenMainMenu()
    {
        gameObject.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
