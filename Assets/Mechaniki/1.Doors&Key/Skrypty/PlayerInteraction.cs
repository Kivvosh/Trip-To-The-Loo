using UnityEngine;
using TMPro; // Wymagane dla TextMeshPro

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ustawienia Interakcji")]
    public float interactionDistance = 5f;

    [Header("Referencje")]
    public TextMeshProUGUI tooltipTextUI;

    private Camera mainCamera;
    private Inventory playerInventory;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) mainCamera = Object.FindFirstObjectByType<Camera>();

        // Szukamy ekwipunku na graczu
        playerInventory = GetComponentInParent<Inventory>();
        if (playerInventory == null) playerInventory = GetComponent<Inventory>();

        // Domyœlnie chowamy napis na starcie
        if (tooltipTextUI != null) tooltipTextUI.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        if (mainCamera == null) return;

        // Tworzymy promieñ ze œrodka ekranu
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Opcjonalnie rysujemy liniê pomocnicz¹ w oknie Scene
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                // 1. Pobieramy odpowiedni tekst (naprawia b³¹d z linijki 45 i 50)
                if (tooltipTextUI != null)
                {
                    tooltipTextUI.text = interactable.GetCurrentTooltip(playerInventory);
                    tooltipTextUI.gameObject.SetActive(true);
                }

                // 2. Przekazujemy ekwipunek do interakcji po klikniêciu PPM (naprawia b³¹d z linijki 56)
                if (Input.GetMouseButtonDown(1))
                {
                    interactable.Interact(playerInventory);
                }

                return; // Wychodzimy, ¿eby nie schowaæ napisu
            }
        }

        // Jeœli odwrócimy wzrok, chowamy podpowiedŸ
        if (tooltipTextUI != null && tooltipTextUI.gameObject.activeSelf)
        {
            tooltipTextUI.gameObject.SetActive(false);
        }
    }
}