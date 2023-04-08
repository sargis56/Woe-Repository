using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject settingsMenu;

    //public CameraController cameraController;
    public PlayerController playerController;
    public void ToggleSettingsMenu()
    {
        menu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ToggleMenu()
    {
        menu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ReturnToGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.isInMenu = false;
        //cameraController.cameraMovementToggle = true;
        this.gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToGame();
        }
    }
}
