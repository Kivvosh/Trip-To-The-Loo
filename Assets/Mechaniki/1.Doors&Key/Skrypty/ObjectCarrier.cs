using UnityEngine;

public class ObjectCarrier : MonoBehaviour
{
    [Header("Ustawienia")]
    public float pickupRange = 5f;      // Jak daleko gracz siêga wzrokiem
    public float holdDistance = 2.5f;   // W jakiej odleg³oœci trzyma przedmiot przed sob¹
    public string pickupTag = "Pickup"; // Tag obiektów, które mo¿na podnosiæ
    public LayerMask placementMask;     // Warstwy, na których mo¿na k³aœæ przedmioty (np. Default, Ground)

    private GameObject heldObject;
    private Rigidbody heldRb;
    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // PPM
        {
            if (heldObject == null)
            {
                TryPickUp();
            }
            else
            {
                TryPlaceDown();
            }
        }

        if (heldObject != null)
        {
            CarryObject();
        }
    }

    void TryPickUp()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Raycast ze œrodka ekranu
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag(pickupTag))
            {
                heldObject = hit.collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody>();

                if (heldRb != null)
                {
                    heldRb.isKinematic = true; // Wy³¹czamy fizykê, ¿eby przedmiot nie "szala³"
                    heldRb.useGravity = false;
                }

                // Wy³¹czamy kolizje przedmiotu z graczem, ¿eby nas nie "wypchn¹³"
                Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponentInParent<Collider>(), true);
            }
        }
    }

    void CarryObject()
    {
        // Przedmiot p³ynnie pod¹¿a za wzrokiem
        Vector3 targetPosition = playerCam.transform.position + playerCam.transform.forward * holdDistance;
        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, targetPosition, Time.deltaTime * 10f);

        // Opcjonalnie: przedmiot zachowuje rotacjê œwiata (nie krêci siê razem z nami)
        // heldObject.transform.rotation = Quaternion.identity; 
    }

    void TryPlaceDown()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Szukamy punktu na powierzchni (pod³o¿e, stó³ itp.)
        if (Physics.Raycast(ray, out hit, pickupRange, placementMask))
        {
            PlaceObject(hit.point);
        }
        else
        {
            // Jeœli nie patrzymy na nic konkretnego, po prostu puœæ przedmiot przed sob¹
            PlaceObject(playerCam.transform.position + playerCam.transform.forward * holdDistance);
        }
    }

    void PlaceObject(Vector3 position)
    {
        heldObject.transform.position = position;

        if (heldRb != null)
        {
            heldRb.isKinematic = false;
            heldRb.useGravity = true;
        }

        // Przywracamy kolizje
        Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), GetComponentInParent<Collider>(), false);

        heldObject = null;
        heldRb = null;
    }
}