using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    void Update()
    {
        // START GAME
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            StartGame();
        }

        // QUIT GAME
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            QuitGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
        SceneManager.LoadScene("ArrivalCutscene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}