using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject settingsMenu;
    public GameObject lobbySelectionMenu;

    public void ToggleTitleScreen()
    {
        titleScreen.SetActive(true);

        settingsMenu.SetActive(false);
        lobbySelectionMenu.SetActive(false);
    }

    public void ToggleSettingsScreen()
    {
        settingsMenu.SetActive(true);

        titleScreen.SetActive(false);
        lobbySelectionMenu.SetActive(false);
    }

    public void ToggleLobbySelectionScreen()
    {
        lobbySelectionMenu.SetActive(true);
        
        settingsMenu.SetActive(false);
        titleScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
