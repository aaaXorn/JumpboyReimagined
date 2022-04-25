using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] AudioMixer audio_main;//mixer de volume
	
	[SerializeField] Slider audioSlider, diffSlider;//sliders
	//[SerializeField] Text audioText, diffText;//texto dos sliders
	[SerializeField] Toggle concaveToggle;//toggle da camera concava
	
	[SerializeField] GameObject Obj_H2P;//objeto da tela de how to play
	
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
				else StaticVars.MainVolume = 1;
			if(PlayerPrefs.HasKey("Difficulty"))
				StaticVars.GameDifficulty = PlayerPrefs.GetInt("Difficulty");
			if(PlayerPrefs.HasKey("ConcaveCamera"))
				StaticVars.ConcaveCam = (PlayerPrefs.GetInt("ConcaveCamera") > 0);
			
			StaticVars.Initialized = true;
		}
		
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(StaticVars.MainVolume) * 20);
		audioSlider.value = StaticVars.MainVolume;
		//define a dificuldade
		diffSlider.value = StaticVars.GameDifficulty;
		//define a camera concava
		concaveToggle.isOn = StaticVars.ConcaveCam;
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
		StaticVars.NextScene = "Jumpboy2D";

		SceneManager.LoadScene("Load");
	}
	public void To3D()
	{
		StaticVars.NextScene = "Jumpboy3D";

		SceneManager.LoadScene("Load");
	}
	
	public void Exit()
	{
		//salva as info do player prefs
		PlayerPrefs.SetFloat("MainAudio", StaticVars.MainVolume);
		PlayerPrefs.SetInt("Difficulty", StaticVars.GameDifficulty);
		PlayerPrefs.SetInt("hScore", StaticVars.HighScore);
		PlayerPrefs.SetInt("3d_hScore", StaticVars.HighScore3D);
		PlayerPrefs.SetInt("ConcaveCamera", (StaticVars.ConcaveCam == true ? 1 : 0));
		
		PlayerPrefs.Save();
		Application.Quit();
	}
	
	public void ChangeVolume(float volume)
	{
		//define o audio
		audio_main.SetFloat("Master", Mathf.Log10(volume) * 20);
		
		StaticVars.MainVolume = volume;
		
		//audioText.text = ""+Mathf.Round(volume * 100);
	}
	
	public void ChangeDifficulty(float diff)
	{
		//define a dificuldade do jogo
		StaticVars.GameDifficulty = (int)diff;
		
		//diffText.text = ""+diff;
	}
	
	public void ChangeConcaveCam(bool conc)
	{
		//define o tipo de camera
		StaticVars.ConcaveCam = conc;
	}
	
	public void HowToPlay()
	{
		Obj_H2P.SetActive(!Obj_H2P.activeSelf);
	}
}
