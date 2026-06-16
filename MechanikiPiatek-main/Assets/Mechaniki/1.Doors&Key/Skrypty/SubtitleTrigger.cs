using UnityEngine;
using System.Collections;

public class SubtitleTrigger : MonoBehaviour
{
    public string textToShow = "";
    public float duration = 3f;

    [Header("Druga linijka (opcjonalnie)")]
    public string text2 = "";
    public float duration2 = 3f;
    public float delayBetweenTexts = 1f; // Przerwa miêdzy znikniêciem 1. a pojawieniem siê 2.

    public bool destroyAfterUse = true;

    private bool isUsed = false; // Zabezpieczenie, ¿eby trigger nie odpali³ siê dwa razy na raz

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy czy to gracz i czy ten trigger nie zosta³ ju¿ aktywowany
        if (other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            StartCoroutine(PlaySubtitleSequence());
        }
    }

    private IEnumerator PlaySubtitleSequence()
    {
        // 1. Wyœwietlamy pierwsz¹ linijkê
        if (SubtitleManager.Instance != null && !string.IsNullOrEmpty(textToShow))
        {
            SubtitleManager.Instance.DisplaySubtitle(textToShow, duration);
        }

        // Jeœli zdefiniowaliœmy drug¹ linijkê, to realizujemy ca³¹ sekwencjê czekania
        if (!string.IsNullOrEmpty(text2))
        {
            // Czekamy tyle, ile trwa wyœwietlanie pierwszego tekstu + czas przerwy miêdzy nimi
            yield return new WaitForSeconds(duration + delayBetweenTexts);

            // 2. Wyœwietlamy drug¹ linijkê
            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.DisplaySubtitle(text2, duration2);
            }

            // Czekamy na zakoñczenie wyœwietlania drugiej linijki, zanim ewentualnie usuniemy trigger
            yield return new WaitForSeconds(duration2);
        }
        else
        {
            // Jeœli nie ma drugiej linijki, czekamy tylko na zakoñczenie pierwszej
            yield return new WaitForSeconds(duration);
        }

        // 3. Po zakoñczeniu ca³ego dialogu decydujemy, czy niszczymy obiekt triggera
        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}