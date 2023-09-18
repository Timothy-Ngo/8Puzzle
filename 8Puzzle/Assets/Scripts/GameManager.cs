//-------------------------------------------------------------------
// Name: Timothy Ngo
// School Email: timothyngo@nevada.unr.edu
//-------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string sceneName;

    public void GoToRaceScene()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("8Puzzle");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
