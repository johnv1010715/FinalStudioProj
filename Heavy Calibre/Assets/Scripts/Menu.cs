using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject optionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Back()
    {
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1f;
    }

    public void ViewOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quiting the game");
        Application.Quit();
    }
}
