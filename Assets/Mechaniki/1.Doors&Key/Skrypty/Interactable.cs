using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    [Header("Warunek Ekwipunku (Opcjonalnie)")]
    [Tooltip("Jeœli wpiszesz tu ID przedmiotu (np. Karta1), skrypt sprawdzi czy gracz go ma. Jeœli zostawisz puste, od razu zadzia³a Wariant B.")]
    public string requiredKeyId = "";

    [Header("Wariant A: BRAK PRZEDMIOTU (Blokada)")]
    [Tooltip("Tekst wyœwietlany nad celownikiem, gdy gracz nie ma wymaganego przedmiotu.")]
    public string tooltipLocked = "[PPM] Zbadaj";
    [Tooltip("Dialog wypowiadany przez postaæ po klikniêciu PPM bez przedmiotu.")]
    public string dialogLocked = "To przejœcie jest zablokowane.";

    [Header("Wariant B: SUKCES (Interakcja dostêpna)")]
    [Tooltip("Tekst wyœwietlany nad celownikiem, gdy interakcja jest mo¿liwa.")]
    public string tooltipAvailable = "[PPM] U¿yj";
    [Tooltip("Dialog wypowiadany przez postaæ po klikniêciu PPM (gdy ma przedmiot lub go nie wymaga).")]
    public string dialogAvailable = "Uda³o siê.";

    [Header("Konfiguracja Akcji")]
    [Tooltip("Czy po udanej interakcji (Wariant B) ten obiekt ma znikn¹æ ze sceny?")]
    public bool destroyOnSuccess = false;
    [Tooltip("Czas wyœwietlania jednej linijki tekstu w sekundach.")]
    public float dialogDuration = 3f;

    [Tooltip("OpóŸnienie przed pojawieniem siê tekstu po klikniêciu (w sekundach).")]
    public float delayBeforeSpeech = 0.5f; // Pó³ sekundy opóŸnienia domyœlnie

    private bool isTalking = false;

    // Funkcja dla skryptu wzroku (PlayerInteraction) do pobrania aktualnego tekstu
    public string GetCurrentTooltip(Inventory inv)
    {
        if (HasRequiredItem(inv))
        {
            return tooltipAvailable;
        }
        return tooltipLocked;
    }

    // Funkcja dla skryptu wzroku (PlayerInteraction) wywo³ywana po klikniêciu PPM
    // Funkcja dla skryptu wzroku (PlayerInteraction) wywo³ywana po klikniêciu PPM
    public void Interact(Inventory inv)
    {
        if (isTalking) return; // Ochrona przed ponownym klikniêciem w trakcie kwestii

        if (HasRequiredItem(inv))
        {
            // --- TUTAJ DODAJEMY BRAKUJ¥C¥ LOGIKÊ ZAPISYWANIA SUKCESU ---
            if (inv != null)
            {
                // Jeœli przedmiot nie wymaga klucza, ale sam ma nazwê (np. karta na ziemi), dodaje swoj¹ nazwê.
                // Jeœli to drzwi, które w³aœnie otwieramy, zapisujemy w pamiêci "NazwaObiektu_otwarte"
                string kluczSukcesu = string.IsNullOrEmpty(requiredKeyId) ? gameObject.name + "_otwarte" : gameObject.name;
                inv.AddKey(kluczSukcesu);

                // Ten napis pojawi siê w konsoli Unity, gdy otworzysz kabinê:
                Debug.Log("<color=green>SUKCES INTERAKCJI!</color> Dodano do ekwipunku: " + kluczSukcesu);
            }
            // ----------------------------------------------------------

            // Odpalamy dialog sukcesu i przekazujemy informacjê o ewentualnym usuniêciu obiektu
            StartCoroutine(PlayDialogSequence(dialogAvailable, destroyOnSuccess));
        }
        else
        {
            // Odpalamy dialog blokady (obiekt na pewno zostaje)
            StartCoroutine(PlayDialogSequence(dialogLocked, false));
        }
    }

    // Pomocnicza funkcja sprawdzaj¹ca czy gracz spe³nia warunki
    private bool HasRequiredItem(Inventory inv)
    {
        // Jeœli pole jest puste, warunek jest zawsze spe³niony
        if (string.IsNullOrEmpty(requiredKeyId)) return true;

        // Jeœli pole ma wpis, pytamy ekwipunek czy posiada klucz
        return inv != null && inv.HasKey(requiredKeyId);
    }

    IEnumerator PlayDialogSequence(string textToDisplay, bool shouldDestroy)
    {
        // Jeœli pole dialogu jest puste w Inspektorze, pomijamy gadanie
        if (string.IsNullOrEmpty(textToDisplay))
        {
            if (shouldDestroy) Destroy(gameObject);
            yield break;
        }

        isTalking = true;

        // Wywo³ujemy Twój Subtitle Manager
        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.DisplaySubtitle(textToDisplay, dialogDuration);
        }

        yield return new WaitForSeconds(dialogDuration);

        // Jeœli interakcja siê uda³a i zaznaczyliœmy usuwanie - niszczymy obiekt
        if (shouldDestroy)
        {
            Destroy(gameObject);
        }

        isTalking = false;
    }
}