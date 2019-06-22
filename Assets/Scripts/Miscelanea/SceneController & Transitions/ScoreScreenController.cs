using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DefinitiveScript;

public class ScoreScreenController : MonoBehaviour
{
    public TextMeshProUGUI congratulationsText;

    public TextMeshProUGUI earnedMoneyInfoText;
    public TextMeshProUGUI elapsedTimeInfoText;

    public TextMeshProUGUI earnedMoneyText;
    public TextMeshProUGUI elapsedTimeText;

    public Image buttonImage;

    public AudioSource endingMusicSource;

    public float initialScale = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        Color transparent = new Color(1, 1, 1, 0);

        congratulationsText.color = transparent;
        earnedMoneyInfoText.color = transparent;
        elapsedTimeInfoText.color = transparent;
        buttonImage.color = transparent;

        congratulationsText.GetComponent<RectTransform>().localScale = congratulationsText.GetComponent<RectTransform>().localScale * initialScale;
        earnedMoneyInfoText.GetComponent<RectTransform>().localScale = earnedMoneyInfoText.GetComponent<RectTransform>().localScale * initialScale;
        elapsedTimeInfoText.GetComponent<RectTransform>().localScale = elapsedTimeInfoText.GetComponent<RectTransform>().localScale * initialScale;

        earnedMoneyText.text = "";
        elapsedTimeText.text = "";
    }

    public void Play(float waitingTime, int money, float elapsedTime)
    {
        endingMusicSource.Play();
        StartCoroutine(PlayCoroutine(waitingTime, money, elapsedTime));
    }

    private IEnumerator PlayCoroutine(float waitingTime, int money, float elapsedTime)
    {
        yield return new WaitForSeconds(waitingTime);

        yield return StartCoroutine(FadeInAndScaleText(0.5f, congratulationsText));

        yield return StartCoroutine(FadeInAndScaleText(0.5f, earnedMoneyInfoText));

        int auxMoney = 0;

        earnedMoneyText.text = "0";
        while(auxMoney < money)
        {
            auxMoney++;
            earnedMoneyText.text = auxMoney.ToString();
            yield return new WaitForSeconds(0.01f);
        }

        yield return StartCoroutine(FadeInAndScaleText(0.5f, elapsedTimeInfoText));

        int elapsedTimeInt = Mathf.RoundToInt(elapsedTime);
        int auxTime = 0;
        int seconds = 0;
        int minutes = 0;
        int hours = 0; 

        elapsedTimeText.text = "00:00:00";
        while(auxTime < elapsedTime)
        {
            auxTime++;
            seconds++;

            if(seconds == 60)
            {
                seconds = 0;
                minutes++;

                if(minutes == 60)
                {
                    minutes = 0;
                    hours++;
                }
            }

            string text = (hours > 9 ? hours.ToString() : "0" + hours) + ":";
            text += (minutes > 9 ? minutes.ToString() : "0" + minutes) + ":";
            text += seconds > 9 ? seconds.ToString() : "0" + seconds;

            elapsedTimeText.text = text;

            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(FadeInButton(1.0f, buttonImage));
    }

    private IEnumerator FadeInAndScaleText(float time, TextMeshProUGUI text)
    {
        Color c = text.color;

        float initialAlpha = 0f;
        float finalAlpha = 1f;

        RectTransform textTransform = text.GetComponent<RectTransform>();

        Vector3 initialScale = textTransform.localScale;
        Vector3 finalScale = new Vector3(1, 1, 1);

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
            text.color = c;

            textTransform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / time);

            yield return null;
        }

        c.a = finalAlpha;
        text.color = c;

        textTransform.localScale = finalScale;
    }

    private IEnumerator FadeInButton(float time, Image button)
    {
        Color c = button.color;

        float initialAlpha = 0f;
        float finalAlpha = 1f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
            button.color = c;

            yield return null;
        }

        c.a = finalAlpha;
        button.color = c;
    }

    private IEnumerator FadeOutText(float time, TextMeshProUGUI text)
    {
        Color c = text.color;

        float initialAlpha = 1f;
        float finalAlpha = 0f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
            text.color = c;

            yield return null;
        }

        c.a = finalAlpha;
        text.color = c;
    }

    private IEnumerator FadeOutButton(float time, Image button)
    {
        Color c = button.color;

        float initialAlpha = 1f;
        float finalAlpha = 0f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
            button.color = c;

            yield return null;
        }

        c.a = finalAlpha;
        button.color = c;
    }

    private IEnumerator FadeOutClip(float time, AudioSource audio)
    {
        float initialVolume = audio.volume;
        float finalVolume = 0f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            audio.volume = Mathf.Lerp(initialVolume, finalVolume, elapsedTime / time);

            yield return null;
        }
        audio.volume = finalVolume;
    } 

    public void BackToMenu()
    {
        StartCoroutine(BackToMenuCoroutine(1.0f));
    }

    private IEnumerator BackToMenuCoroutine(float time)
    {
        StartCoroutine(FadeOutText(time, congratulationsText));
        StartCoroutine(FadeOutText(time, earnedMoneyInfoText));
        StartCoroutine(FadeOutText(time, elapsedTimeInfoText));
        StartCoroutine(FadeOutText(time, earnedMoneyText));
        StartCoroutine(FadeOutText(time, elapsedTimeText));
        StartCoroutine(FadeOutButton(time, buttonImage));

        StartCoroutine(FadeOutClip(time, endingMusicSource));

        yield return new WaitForSeconds(time);

        GameManager.Instance.SceneController.FinishGame();
    }


}
