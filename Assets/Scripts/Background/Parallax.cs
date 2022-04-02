using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	//tamanho do sprite
	[SerializeField] float length;
	//posição anterior da última vez que o background loopou
	[SerializeField] float startPos;
	//referência da camera
	[SerializeField] GameObject cam;
	//força da paralaxe
	[SerializeField] float parallaxEffect;
	
	void Start()//variáveis
	{
		cam = Camera.main.gameObject;
		startPos = transform.position.x;
		length = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	void FixedUpdate()
	{
		//usado para determinar quando o loop acontece
		float temp = (cam.transform.position.x * (1 - parallaxEffect));
		//distância que o background com paralaxe vai percorrer
		float distance = (cam.transform.position.x * parallaxEffect);
		
		transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
		
		//loopa o background
		if(temp > startPos + length)
		{
			startPos += length;
		}
		else if(temp < startPos - length)
		{
			startPos -= length;
		}
	}
}