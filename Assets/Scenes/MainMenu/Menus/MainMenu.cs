using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject credits;

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OpenCredits()
    {
        credits.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void LoadBoss()
    {
        SceneManager.LoadScene(14);
    }
}
