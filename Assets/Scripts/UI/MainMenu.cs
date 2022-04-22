using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] AudioMixer audio_main;//mixer de volume
	
	//inicializa as variáveis do StaticVars
	void Awake()
	{
		//if impede as variáveis de resetarem quando o jogador volta pro menu
		if(!StaticVars.Initialized)
		{
			if(PlayerPrefs.HasKey("hScore"))
				StaticVars.HighScore = PlayerPrefs.GetInt("hScore");
			if(PlayerPrefs.HasKey("3d_hScore"))
				StaticVars.HighScore3D = PlayerPrefs.GetInt("3d_hScore");
			if(PlayerPrefs.HasKey("MainAudio"))
				StaticVars.MainVolume = PlayerPrefs.GetFloat("MainAudio");
			if(PlayerPrefs.HasKey("Difficulty"))
				StaticVars.GameDifficulty = PlayerPrefs.GetInt("Difficulty");
			
			StaticVars.Initialized = true;
		}
	}
	
	void Start()
	{
		float am_volume = StaticVars.MainVolume;
		
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(am_volume) * 20);
	}
	
	//muda a cena pro modo de jogo escolhido
    public void To2D()
	{
		SceneManager.LoadScene("Load2D");
	}
	public void To3D()
	{
		SceneManager.LoadScene("Load3D");
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
	
	public void ChangeVolume(float volume)
	{
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(volume) * 20);
		
		StaticVars.MainVolume = volume;
	}
}
