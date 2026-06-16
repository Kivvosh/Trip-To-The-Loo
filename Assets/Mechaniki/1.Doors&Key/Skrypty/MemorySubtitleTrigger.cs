using UnityEngine;
using System.Collections;

public class MemorySubtitleTrigger : MonoBehaviour
{
    [Header("Warunek aktywacji")]
    [Tooltip("Wpisz dok³adnie nazwê obiektu drzwi z hierarchii + '_otwarte'")]
    public string requiredMemoryId = "OpenableKibel_otwarte";

    [Header("Dialog postaci")]
    public string subtitleText = "";
    public float duration = 3.5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Jeœli to gracz i trigger nie odpali³ siê jeszcze wczeœniej
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // Szukamy komponentu ekwipunku na graczu
            Inventory inv = other.GetComponentInParent<Inventory>();
            if (inv == null) inv = other.GetComponent<Inventory>();

            if (inv != null)
            {
                // SPRAWDZAMY: Czy gracz wchodzi³ wczeœniej w interakcjê z tamtymi drzwiami?
                if (inv.HasKey(requiredMemoryId))
                {
                    hasTriggered = true;
                    StartCoroutine(ShowSubtitle());
                }
            }
        }
    }

    IEnumerator ShowSubtitle()
    {
        // Wywo³ujemy Subtitle Manager
        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.DisplaySubtitle(subtitleText, duration);
        }

        yield return new WaitForSeconds(duration);

        // Po wypowiedzeniu kwestii niszczymy ten trigger na ziemi, ¿eby nie odpala³ siê w nieskoñczonoœæ
        Destroy(gameObject);
    }
}
