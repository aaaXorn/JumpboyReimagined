using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpboyControl : MonoBehaviour
{
	Rigidbody rigid;
	
	bool landed = true;//se o jogador está no chão
	
	int lives = 2;//vidas do jogador
	bool hurt;//enquanto o jogador toma um hit
	
	[SerializeField] Vector3 BaseVelocity;//velocidade do jumpboy correndo
	float ground_dist, land_ray_height = 0.1f;//raycast do pulo
	bool grounded;//se o player está no chão
	
	#region jump
	bool jump_holding;//se o player está carregando um pulo
	[SerializeField] float jump_charge, max_jump_charge;//carga do pulo atual e máxima
	
	[SerializeField] float jump_base_force, jump_charge_force;//força base e por carregar do pulo
	#endregion
	
	#region slide
	bool sliding;//se o jogador está dando um slide
	
	[SerializeField]
	Collider head_col;//collider da cabeça, desabilitado durante o slide
	[SerializeField] Collider body_col;//collider do corpo
	#endregion
	
    //inicializa variáveis
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
		
		//determina a distância até o chão
		ground_dist = body_col.bounds.extents.y + land_ray_height;
		
		//movimento horizontal
		rigid.velocity = BaseVelocity;
    }
	
	//física do jogo
	void FixedUpdate()
	{
		//movimento horizontal
		
		
		//carregando o pulo
		//fora do if(landed) pra servir como buffer
		if(jump_holding)
		{
			//se o timer passar do máximo
			if(jump_charge >= max_jump_charge)
			{
				jump_charge = max_jump_charge;
			}
			else
			{
				//timer
				jump_charge += Time.deltaTime;
			}
		}
		
		//checa se o jogador está no chão com raycast
		landed = Physics.Raycast(transform.position, -Vector3.up, ground_dist);
		Debug.DrawRay(transform.position, -Vector3.up * ground_dist, Color.red);
		print(landed);
		
		//se o jogador está no chão
		if(landed)
		{
			//slide
			if(sliding)
			{
				//tira a hurtbox da cabeça
				if(head_col.enabled) head_col.enabled = false;
				
				
			}
			//idle e pulo
			else
			{
				//retorna a hurtbox da cabeça
				if(!head_col.enabled) head_col.enabled = true;
				
				//pulo
				if(jump_charge > 0 && !jump_holding)
				{
					//força do pulo
					float forceY = jump_base_force +
								  (jump_charge_force * jump_charge / max_jump_charge);
					rigid.AddForce(new Vector3(0, forceY, 0), ForceMode.Impulse);
					
					//reseta o charge do pulo
					jump_charge = 0;
				}
			}
		}
	}
	
	#region buttons
	//botão de pulo apertado
	public void JumpButtonPress()
	{
		jump_holding = true;
	}
	//botão de pulo soltado
	public void JumpButtonRelease()
	{
		//sobrepõe o slide
		sliding = false;
		
		jump_holding = false;
	}
	
	//botão de slide apertado
	public void SlideButtonPress()
	{
		sliding = true;
	}
	//botão de slide soltado
	public void SlideButtonRelease()
	{
		sliding = false;
	}
	#endregion
}
