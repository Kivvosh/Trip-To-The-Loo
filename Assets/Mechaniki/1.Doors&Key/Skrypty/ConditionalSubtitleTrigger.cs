using UnityEngine;

public class ConditionalSubtitleTrigger : MonoBehaviour
{
    [Header("Ustawienia Napisu")]
    public string textNoKey = "Drzwi s¹ zamkniête... Czy pielêgniarka schowa³a klucz?";
    public float duration = 3f;

    [Header("Warunek")]
    public string requiredKeyId = "Karta3";
    public bool destroyAfterShowing = true;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Sprawdzamy czy cokolwiek wesz³o w trigger
        Debug.Log("Coœ wesz³o w trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("To gracz! Szukam ekwipunku...");

            Inventory inv = other.GetComponentInParent<Inventory>();

            if (inv != null)
            {
                Debug.Log("Ekwipunek znaleziony. Sprawdzam klucz: " + requiredKeyId);

                if (inv.HasKey(requiredKeyId))
                {
                    Debug.Log("Gracz MA KLUCZ. Niszczê trigger bez napisu.");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Gracz NIE MA klucza. Próbujê wyœwietliæ napis...");

                    if (SubtitleManager.Instance != null)
                    {
                        SubtitleManager.Instance.DisplaySubtitle(textNoKey, duration);
                        if (destroyAfterShowing) Destroy(gameObject);
                    }
                    else
                    {
                        Debug.LogError("B£¥D: SubtitleManager.Instance jest pustY! Nie ma go na scenie?");
                    }
                }
            }
            else
            {
                Debug.LogWarning("UWAGA: Nie znaleziono skryptu Inventory na graczu ani jego rodzicach!");
            }
        }
    }
}