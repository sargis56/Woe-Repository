using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject settingsMenu;

    public void ToggleTitleScreen()
    {
        titleScreen.SetActive(true);

        settingsMenu.SetActive(false);
    }

    public void ToggleSettingsScreen()
    {
        settingsMenu.SetActive(true);

        titleScreen.SetActive(false);
    }
}
