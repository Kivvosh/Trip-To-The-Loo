using UnityEngine;
using UnityEngine.SceneManagement; // Wymagane do zmiany scen!

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Hospital"; // Wpisz tu nazwê swojej sceny z gr¹

    void Start()
    {
        // Upewniamy siê, ¿e w menu kursor jest widoczny i odblokowany
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}