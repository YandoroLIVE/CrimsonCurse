using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}