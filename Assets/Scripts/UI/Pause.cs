using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] JumpboyControl JC;//script do jump b�ia
    float unpaused_time;//tempo fora do pause

    [SerializeField] GameObject Obj_PauseMenu;//objeto do menu de pause

    void Start()
    {
        unpaused_time = Time.timeScale;//pega o tempo base
    }

    public void PauseMenu()
    {
        if (!JC.paused)
        {
            Time.timeScale = 0;

            JC.paused = true;
            Obj_PauseMenu.SetActive(true);
        }
        else if(JC.lives > 0)
        {
            Time.timeScale = unpaused_time;

            JC.paused = false;
            Obj_PauseMenu.SetActive(false);
        }
    }

    public void MainMenu()
    {
        Time.timeScale = unpaused_time;

        PlayerPrefs.Save();
        SceneManager.LoadScene("LoadMenu");
    }

    public void Exit()
    {
        Time.timeScale = unpaused_time;

        PlayerPrefs.Save();
        Application.Quit();
    }
}
