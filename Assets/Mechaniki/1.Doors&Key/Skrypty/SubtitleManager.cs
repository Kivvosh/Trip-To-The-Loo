using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance; // Singleton, by ³atwo go wywo³aæ
    public TextMeshProUGUI subtitleText;

    private Coroutine currentCoroutine;

    void Awake()
    {
        Instance = this;
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);
    }

    public void DisplaySubtitle(string text, float duration)
    {
        // Jeœli ju¿ coœ siê wyœwietla, zatrzymaj to
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowTextRoutine(text, duration));
    }

    private IEnumerator ShowTextRoutine(string text, float duration)
    {
        subtitleText.text = text;
        subtitleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        subtitleText.gameObject.SetActive(false);
        currentCoroutine = null;
    }
}