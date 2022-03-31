using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	[SerializeField] float length;
	[SerializeField] float startPos;
	[SerializeField] GameObject cam;
	[SerializeField] float parallaxEffect;
	
	void Start()
	{
		cam = Camera.main.gameObject;
		startPos = transform.position.x;
		length = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	void Update()
	{
		float temp = (cam.transform.position.x * (1 - parallaxEffect));
		float distance = (cam.transform.position.x * parallaxEffect);
		
		transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
		
		if(temp > startPos + length)
		{
			startPos += length;
		}
		else if(temp < startPos - length)
		{
			startPos -= length;
		}
	}
	
	/*
	//posição que o objeto vai quando completa o loop
    [SerializeField] float teleport_pos_X;
	//onde o jogador vai se teleportar quando chegar, modifier de bugfix
	[SerializeField] float teleport_at_X;
	
	//velocidade do paralaxe
	[SerializeField] float parallax_spd;
	
	void Start()
	{
		float spriteSize = GetComponent<SpriteRenderer>().bounds.extents.x * transform.localScale.x;
		print(spriteSize);
		//muda teleport_at_X pro valor certo
		teleport_at_X -= spriteSize;
		
		teleport_pos_X = teleport_at_X + spriteSize*3;
	}
	
    void Update()
    {
		//vetor de movimento da paralaxe
        Vector3 move_to = new Vector3(parallax_spd * Time.deltaTime, 0, 0);
		//movimento
		transform.Translate(move_to);
		
		if(transform.localPosition.x <= teleport_at_X)
		{
			//diferença entre a posição ideal e a real
			float diff = teleport_at_X - transform.localPosition.x;
			
			transform.localPosition = new Vector3(teleport_pos_X + diff, 0, 0);
		}
    }*/
}