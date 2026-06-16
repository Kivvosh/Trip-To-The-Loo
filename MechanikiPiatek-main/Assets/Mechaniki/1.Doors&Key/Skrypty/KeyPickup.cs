using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    public string keyId = "";
    public string prompt = "PPM – podnieœ klucz";

    private bool playerIn;
    private Inventory playerInv;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = true;
            // Szukamy Inventory na obiekcie, który wszed³ w trigger, 
            // a jeœli go tam nie ma, to u jego rodziców (np. na obiekcie Character)
            playerInv = other.GetComponentInParent<Inventory>();

            InteractionPromptUI.Instance?.Show(prompt);
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
        // Sprawdzamy czy gracz jest w œrodku i czy faktycznie znaleŸliœmy Inventory
        if (!playerIn || playerInv == null) return;

        // Zmienione na GetMouseButtonDown(1), czyli PPM
        if (Input.GetMouseButtonDown(1))
        {
            playerInv.AddKey(keyId);
            InteractionPromptUI.Instance?.Hide();
            Debug.Log($"Podniesiono klucz: {keyId}");
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        transform.Rotate(0f, 60f * Time.deltaTime, 0f, Space.World);
    }
}
