using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] JumpboyControl JC;//script do jump bóia
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

	public void Restart()
	{
		Time.timeScale = unpaused_time;
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    public void MainMenu()
    {
        Time.timeScale = unpaused_time;
		
        SceneManager.LoadScene("LoadMenu");
    }

    public void Exit()
    {
        //salva as info do player prefs
		PlayerPrefs.SetFloat("MainAudio", StaticVars.MainVolume);
		PlayerPrefs.SetInt("Difficulty", StaticVars.GameDifficulty);
		PlayerPrefs.SetInt("hScore", StaticVars.HighScore);
		PlayerPrefs.SetInt("3d_hScore", StaticVars.HighScore3D);
		
		PlayerPrefs.Save();
		Application.Quit();
    }
}
