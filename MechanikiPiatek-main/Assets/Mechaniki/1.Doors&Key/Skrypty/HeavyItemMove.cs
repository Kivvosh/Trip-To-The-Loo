using UnityEngine;

public class HeavyObjectPush : MonoBehaviour
{
    [Header("Warunek Blokady")]
    [Tooltip("Wpisz tutaj nazwê przedmiotu, który odblokuje mo¿liwoœæ pchania (np. Karta3). Jeœli zostawisz puste, ³ó¿ko mo¿na pchaæ od razu.")]
    public string requiredKeyId = "Karta3";
    public string dialogLocked = "Jest za ciê¿kie, nie dam rady sama tego przesun¹æ...";

    [Header("Ustawienia Przesuwania")]
    public Vector3 pushDirection = Vector3.forward;
    public float distancePerClick = 0.05f;
    public float maxPushDistance = 2.0f;
    public float lerpSpeed = 5f;
    public float interactionRange = 3f;

    [Header("DŸwiêk Przesuwania")]
    public AudioClip scrapeClip;

    [Header("UI")]
    public string prompt = "Spamuj PPM, aby przesun¹æ";

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentMovedDistance = 0f;
    private Camera playerCam;
    private bool isPlayerNear;

    private AudioSource audioSource;
    private Inventory playerInventory; // Referencja do ekwipunku gracza

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        playerCam = Camera.main;

        // Szukamy ekwipunku na graczu w scenie
        playerInventory = Object.FindFirstObjectByType<Inventory>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (audioSource != null && scrapeClip != null)
        {
            audioSource.clip = scrapeClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        CheckPlayerProximity();

        if (isPlayerNear && Input.GetMouseButtonDown(1)) // PPM
        {
            TryPush();
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

        HandleSoundFade();
    }

    void CheckPlayerProximity()
    {
        if (playerCam == null) return;
        float dist = Vector3.Distance(transform.position, playerCam.transform.position);

        if (dist <= interactionRange)
        {
            if (!isPlayerNear)
            {
                isPlayerNear = true;

                // Pokazujemy prompt tylko, jeœli gracz MA kartê LUB jeœli nie ma ¿adnych wymagañ
                if (HasRequiredItem())
                {
                    InteractionPromptUI.Instance?.Show(prompt);
                    SubtitleManager.Instance.DisplaySubtitle("It's so heavy, but I need to keep pushing!", 3f);
                }
                else
                {
                    // Jeœli nie ma karty, pokazujemy podpowiedŸ zamiast instrukcji spamowania
                    InteractionPromptUI.Instance?.Show("[PPM] Zbadaj");
                }
            }
        }
        else
        {
            if (isPlayerNear)
            {
                isPlayerNear = false;
                InteractionPromptUI.Instance?.Hide();
            }
        }
    }

    void TryPush()
    {
        if (playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.transform == transform)
            {
                // --- NOWOŒÆ: Sprawdzamy czy gracz ma wymagany przedmiot ---
                if (!HasRequiredItem())
                {
                    // Gracz nie ma karty - blokujemy ruch i wyœwietlamy dialog o braku si³y
                    if (SubtitleManager.Instance != null)
                    {
                        SubtitleManager.Instance.DisplaySubtitle(dialogLocked, 3f);
                    }
                    return; // Przerywamy funkcjê, ³ó¿ko siê nie ruszy!
                }
                // ---------------------------------------------------------

                if (currentMovedDistance < maxPushDistance)
                {
                    if (audioSource != null && !audioSource.isPlaying && scrapeClip != null)
                    {
                        audioSource.volume = 0.5f;
                        audioSource.Play();
                    }

                    targetPosition += pushDirection.normalized * distancePerClick;
                    currentMovedDistance += distancePerClick;

                    Debug.Log("Popychanie...");
                }
                else
                {
                    InteractionPromptUI.Instance?.Show("Przesuniête do oporu");
                }
            }
        }
    }

    // Pomocnicza funkcja sprawdzaj¹ca czy klucz jest w ekwipunku
    private bool HasRequiredItem()
    {
        if (string.IsNullOrEmpty(requiredKeyId)) return true; // Brak wymagañ
        return playerInventory != null && playerInventory.HasKey(requiredKeyId);
    }

    void HandleSoundFade()
    {
        if (audioSource == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > 0.01f && currentMovedDistance < maxPushDistance && HasRequiredItem())
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0.5f, Time.deltaTime * 10f);
        }
        else
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 5f);

            if (audioSource.volume < 0.05f && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}