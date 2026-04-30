using UnityEngine;
using UnityEngine.UI; // Wymagane do obs³ugi Image
using System.Collections;

public class GameEnder : MonoBehaviour
{
    [Header("Ustawienia Napisu")]
    public string endMessage = "Finally.";
    public float textDuration = 3f;

    [Header("Ustawienia Fade (Œciemniania)")]
    public Image fadeImage;       // Przeci¹gnij tutaj swój FadeImage
    public float fadeSpeed = 0.5f; // Prêdkoœæ œciemniania (im mniej, tym wolniej)

    [Header("Ustawienia Interakcji")]
    public float interactionRange = 3f;
    public string prompt = "PPM – Zakoñcz to";

    private bool playerIn;
    private bool isEnding = false;
    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main;
        // Upewniamy siê, ¿e na starcie obrazek jest przezroczysty
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEnding)
        {
            playerIn = true;
            InteractionPromptUI.Instance?.Show(prompt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = false;
            InteractionPromptUI.Instance?.Hide();
        }
    }

    void Update()
    {
        if (playerIn && !isEnding && Input.GetMouseButtonDown(1))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.transform == transform)
            {
                StartCoroutine(EndGameRoutine());
            }
        }
    }

    IEnumerator EndGameRoutine()
    {
        isEnding = true;
        InteractionPromptUI.Instance?.Hide();

        // 1. Wyœwietlamy napis
        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.DisplaySubtitle(endMessage, textDuration);
        }

        // 2. Czekamy chwilê, zanim zacznie siê œciemniaæ (np. po sekundzie napisu)
        yield return new WaitForSeconds(1f);

        // 3. Pêtla œciemniania (Fade to Black)
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        // 4. Krótka pauza na pe³nym czarnym ekranie
        yield return new WaitForSeconds(1.5f);

        // 5. Wyjœcie z gry
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}