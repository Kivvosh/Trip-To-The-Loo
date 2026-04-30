using UnityEngine;

public class GameSetup : MonoBehaviour
{
    void Start()
    {
        // To wykonuje siê tylko raz, zaraz po za³adowaniu sceny gry
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Kursor zosta³ zablokowany przez GameSetup.");
    }
}