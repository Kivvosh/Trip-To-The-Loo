using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Ustawienia Bujania")]
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public float smoothAmount = 10f;

    [Header("DŸwiêki Kroków")]
    [Tooltip("Lista dŸwiêków kroków (skrypt bêdzie losowa³ jeden przy ka¿dym kroku, ¿eby nie brzmia³o to sztucznie).")]
    public AudioClip[] footstepSounds;

    private AudioSource audioSource;
    private bool isStepPressed = false;

    private float timer = 0f;
    private float midpoint;
    private Vector3 lastPosition;
    private float currentSpeed = 0f;
    private float targetX = 0f;
    private float targetY = 0f;

    void Start()
    {
        midpoint = transform.localPosition.y;
        lastPosition = transform.position;

        // Pobieramy Audio Source z tego samego obiektu
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z),
                                              new Vector3(lastPosition.x, 0, lastPosition.z));

        currentSpeed = distanceMoved / Time.deltaTime;
        lastPosition = currentPosition;

        Vector3 localPosition = transform.localPosition;

        if (currentSpeed > 0.2f)
        {
            timer += bobbingSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2)
            {
                timer -= (Mathf.PI * 2);
            }

            targetY = midpoint + (Mathf.Sin(timer) * bobbingAmount);
            targetX = Mathf.Cos(timer * 0.5f) * bobbingAmount;

            // --- NOWOŒÆ: Wykrywanie kroku ---
            // Gdy Sinus schodzi w dó³ (Mathf.Sin(timer) < -0.9f), oznacza to t¹pniêcie nogi o ziemiê
            if (Mathf.Sin(timer) < -0.9f)
            {
                if (!isStepPressed)
                {
                    PlayFootstepSound();
                    isStepPressed = true; // Blokada, ¿eby dŸwiêk nie odpali³ siê kilka razy w ci¹gu tej samej fali
                }
            }
            else
            {
                isStepPressed = false; // Reset blokady, gdy noga unosi siê do góry
            }
        }
        else
        {
            timer = 0f;
            targetY = midpoint;
            targetX = 0f;
            isStepPressed = false;
        }

        localPosition.y = Mathf.Lerp(localPosition.y, targetY, Time.deltaTime * smoothAmount);
        localPosition.x = Mathf.Lerp(localPosition.x, targetX, Time.deltaTime * smoothAmount);

        transform.localPosition = localPosition;
    }

    void PlayFootstepSound()
    {
        if (audioSource == null || footstepSounds.Length == 0) return;

        // Losujemy jeden dŸwiêk z tablicy, ¿eby kroki nie by³y identyczne
        int randomIndex = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[randomIndex];

        // Lekka modyfikacja tonacji (pitch) przy ka¿dym kroku dodaje realizmu!
        audioSource.pitch = Random.Range(0.85f, 1.1f);
        audioSource.Play();
    }
}