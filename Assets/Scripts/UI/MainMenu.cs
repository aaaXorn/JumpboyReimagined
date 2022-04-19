using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] AudioMixer audio_main;//mixer de volume
	
	void Start()
	{
		float am_volume = PlayerPrefs.GetFloat("MainAudio");
		
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(am_volume) * 20);
	}
	
	//muda a cena pro modo de jogo escolhido
    public void To2D()
	{
		PlayerPrefs.Save();
		SceneManager.LoadScene("Load2D");
	}
	public void To3D()
	{
		PlayerPrefs.Save();
		SceneManager.LoadScene("Load3D");
	}
	
	public void Exit()
	{
		PlayerPrefs.Save();
		Application.Quit();
	}
	
	public void ChangeVolume(float volume)
	{
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(volume) * 20);
		
		PlayerPrefs.SetFloat("MainAudio", volume);
	}
}
