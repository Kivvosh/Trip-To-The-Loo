using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorLock : MonoBehaviour
{
    [Header("Ustawienia Drzwi")]
    public bool requiresKey = false;
    public string requiredKeyId = "";
    public Transform doorHinge;         // Pivot/zawias drzwi
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Teksty UI")]
    public string promptOpen = "PPM – Otwórz drzwi";
    public string promptLocked = "PPM – Użyj klucza";
    public string promptNoKey = "Zablokowane (wymagany klucz)";

    private bool isOpen;
    private bool playerIn;
    private bool isMoving;
    private Inventory playerInv;
    private Quaternion closedRot;
    private Collider doorCollider;

    private void Awake()
    {
        // Jeśli nie przypisano hinge, używamy obiektu, na którym jest skrypt
        if (!doorHinge) doorHinge = transform;

        closedRot = doorHinge.rotation;

        // Szukamy kolidera blokującego przejście (nie triggera)
        doorCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = true;
            playerInv = other.GetComponent<Inventory>();
            UpdatePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = false;
            playerInv = null;
            InteractionPromptUI.Instance?.Hide();
        }
    }

    private void Update()
    {
        // Jeśli gracza nie ma w zasięgu, drzwi są już otwarte lub w trakcie ruchu - nic nie rób
        if (!playerIn || isOpen || isMoving) return;

        if (!playerIn || isOpen || isMoving) return;

        // Dodaj tę linię, żeby mieć pewność, że dane są aktualne:
        if (playerInv == null) playerInv = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Inventory>();

            // Obsługa Prawego Przycisku Myszy (PPM)
            if (Input.GetMouseButtonDown(1))
        {
            bool hasKey = playerInv != null && playerInv.HasKey(requiredKeyId);
            bool canOpen = !requiresKey || hasKey;

            if (canOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else
            {
                // Jeśli wymagany klucz, a gracz go nie ma - mrugnij czerwonym komunikatem
                StartCoroutine(FlashNoKey());
            }
        }
    }

    private IEnumerator OpenDoor()
    {
        isMoving = true;
        isOpen = true;
        InteractionPromptUI.Instance?.Hide();

        Quaternion targetRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorHinge.rotation = Quaternion.Slerp(closedRot, targetRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // Wyłączamy kolider blokujący dopiero po całkowitym otwarciu (lub w trakcie)
        if (doorCollider) doorCollider.enabled = false;
        isMoving = false;
    }

    private IEnumerator FlashNoKey()
    {
        isMoving = true; // Blokuje ponowne kliknięcie w trakcie wyświetlania błędu
        InteractionPromptUI.Instance?.Show(promptNoKey);
        yield return new WaitForSeconds(1.0f);
        isMoving = false;
        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (isOpen || !playerIn)
        {
            InteractionPromptUI.Instance?.Hide();
            return;
        }

        if (!requiresKey)
        {
            // Drzwi nie wymagają klucza
            InteractionPromptUI.Instance?.Show(promptOpen);
        }
        else
        {
            // Drzwi wymagają klucza - sprawdź czy gracz go ma
            bool has = playerInv != null && playerInv.HasKey(requiredKeyId);
            InteractionPromptUI.Instance?.Show(has ? promptLocked : promptNoKey);
        }
    }
}