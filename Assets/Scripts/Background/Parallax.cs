using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	//posição que o objeto vai quando completa o loop
    [SerializeField] float teleport_pos_X;
	//onde o jogador vai se teleportar quando chegar
	[SerializeField] float teleport_at_X;
	
	//velocidade do paralaxe
	[SerializeField] float parallax_spd;
	
	void Start()
	{
		//muda teleport_at_X pro valor certo
		teleport_at_X -= GetComponent<SpriteRenderer>().bounds.size.x;
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
    }
}