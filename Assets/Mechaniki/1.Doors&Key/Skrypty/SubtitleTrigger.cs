using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    public string textToShow = "";
    public float duration = 3f;
    public bool destroyAfterUse = true;

    [Header("Druga linijka (opcjonalnie)")]
    public string text2 = "";
    public float duration2 = 3f;
    public float delayBetweenTexts = 1f; // Przerwa miêdzy znikniêciem 1. a pojawieniem siê 2.


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SubtitleManager.Instance.DisplaySubtitle(textToShow, duration);

            if (destroyAfterUse) Destroy(gameObject);
        }
    }
}