using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	//muda a cena pro modo de jogo escolhido
    public void To2D()
	{
		SceneManager.LoadScene("Jumpboy2D");
	}
	public void To3D()
	{
		SceneManager.LoadScene("Jumpboy3D");
	}
}