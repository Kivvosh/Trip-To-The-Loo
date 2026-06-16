using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance; // Singleton, by ³atwo go wywo³aæ
    public TextMeshProUGUI subtitleText;

    private Coroutine currentCoroutine;

    [Header("DŸwiêki Dialogów (Zmieñ Size na 3)")]
    [Tooltip("Wrzuæ tutaj swoje dŸwiêki. Skrypt wylosuje jeden z nich przy ka¿dym dialogu.")]
    public AudioClip[] subtitleSounds; // Dok³adnie tak samo jak w krokach!

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("SubtitleManager: Brak komponentu AudioSource na tym obiekcie!");
        }

        Instance = this;
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);
    }

    public void DisplaySubtitle(string text, float duration)
    {
        // Jeœli ju¿ coœ siê wyœwietla, zatrzymaj to
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowTextRoutine(text, duration));

        // Odpalamy losowy dŸwiêk (tak samo jak w krokach)
        PlayRandomSubtitleSound();
    }

    private void PlayRandomSubtitleSound()
    {
        // Sprawdzamy, czy AudioSource istnieje i czy w ogóle wrzuciliœmy jakieœ dŸwiêki do tabelki
        if (audioSource != null && subtitleSounds != null && subtitleSounds.Length > 0)
        {
            // Losujemy indeks z tabeli
            int randomIndex = Random.Range(0, subtitleSounds.Length);

            // Pobieramy wylosowany dŸwiêk
            AudioClip chosenSound = subtitleSounds[randomIndex];

            if (chosenSound != null)
            {
                // Odtwarzamy dŸwiêk przez PlayOneShot
                audioSource.PlayOneShot(chosenSound);
            }
        }
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