using UnityEngine;

public class HeavyObjectPush : MonoBehaviour
{
    [Header("Ustawienia Przesuwania")]
    public Vector3 pushDirection = Vector3.forward; // Kierunek przesuniêcia (w osiach lokalnych lub œwiata)
    public float distancePerClick = 0.05f;         // Jak daleko przesunie siê po 1 klikniêciu
    public float maxPushDistance = 2.0f;           // Maksymalna odleg³oœæ przesuniêcia
    public float lerpSpeed = 5f;                   // P³ynnoœæ ruchu
    public float interactionRange = 3f;            // Zasiêg interakcji
    public AudioSource scrapeSound;                //dodaj na górze skryptu:


    [Header("UI")]
    public string prompt = "Spamuj PPM, aby przesun¹æ";

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentMovedDistance = 0f;
    private Camera playerCam;
    private bool isPlayerNear;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        playerCam = Camera.main;
    }

    void Update()
    {
        // 1. Sprawdzanie dystansu do gracza
        CheckPlayerProximity();

        // 2. Obs³uga klikania
        if (isPlayerNear && Input.GetMouseButtonDown(1)) // PPM
        {
            TryPush();
        }

        // 3. P³ynny ruch w stronê celu
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
    }

    void CheckPlayerProximity()
    {
        float dist = Vector3.Distance(transform.position, playerCam.transform.position);

        // Jeœli gracz podszed³ blisko
        if (dist <= interactionRange)
        {
            if (!isPlayerNear) // Moment wejœcia w zasiêg
            {
                isPlayerNear = true;
                InteractionPromptUI.Instance?.Show(prompt);
                SubtitleManager.Instance.DisplaySubtitle("It's so heavy, but I need to keep pushing!", 3f);
            }
        }
        else
        {
            if (isPlayerNear) // Moment wyjœcia z zasiêgu
            {
                isPlayerNear = false;
                InteractionPromptUI.Instance?.Hide();
            }
        }
    }

    void TryPush()
    {
        // Sprawdzamy Raycastem czy gracz patrzy bezpoœrednio na ³ó¿ko
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Wewn¹trz funkcji TryPush():
        if (scrapeSound != null && !scrapeSound.isPlaying)
        {
            scrapeSound.Play();
        }

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.transform == transform)
            {
                if (currentMovedDistance < maxPushDistance)
                {
                    // Dodajemy dystans do celu
                    targetPosition += pushDirection.normalized * distancePerClick;
                    currentMovedDistance += distancePerClick;

                    // Opcjonalnie: lekkie dr¿enie kamery przy ka¿dym klikniêciu dla efektu si³y
                    Debug.Log("Popychanie...");
                }
                else
                {
                    InteractionPromptUI.Instance?.Show("Przesuniête do oporu");
                }
            }
        }
    }
}