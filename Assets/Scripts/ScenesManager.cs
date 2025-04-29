
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public void GameplayScene()
    {
        SceneManager.LoadScene("Jogo");
    }
    
    public void TutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }


}
