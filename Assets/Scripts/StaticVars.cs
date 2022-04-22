using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//variáveis mantidas entre cenas
public static class StaticVars
{
	//se já foram inicializadas
	public static bool Initialized {get; set;}
	
    public static float MainVolume {get; set;}
	public static int GameDifficulty {get; set;}
	public static int HighScore {get; set;}
	public static int HighScore3D {get; set;}
}