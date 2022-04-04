using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpboyControl : MonoBehaviour
{
	[Header("Velocity and Misc")]
	Rigidbody rigid;
	Animator anim;
	[SerializeField] Rigidbody rigid_police;//rigidbody do policial que segue o player
	[SerializeField] Transform transf_police, transf_target;//transform do alvo
	//velocidade exponencial pra baixo quando toma hit
	[SerializeField] float init_vel, end_vel, vel_hurt_time_mod;//vel_change;
	
	bool landed = true;//se o jogador está no chão
	
	int lives = 2;//vidas do jogador
	public bool hurt;//enquanto o jogador toma um hit
	[SerializeField] float hurt_time, max_hurt_time;//tempo atual e máximo na animação de dano
	
	[Header("Movement Actions")]
	[SerializeField] Vector3 BaseVelocity;//velocidade do jumpboy correndo
	[SerializeField] float land_ray_height = 0.2f;//raycast do pulo
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
	#endregion
	
    //inicializa variáveis
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
		
		//movimento horizontal
		rigid.velocity = BaseVelocity;
		rigid_police.velocity = BaseVelocity;
		
		anim = GetComponent<Animator>();
    }
	
	//física do jogo
	void FixedUpdate()
	{
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
		
		//se tomou dano
		if(hurt)
		{
			if(lives > 0)//continua vivo
			{
				if(hurt_time <= 0)
				{
					//deixa o jogador parado, talvez mude
					rigid.velocity = new Vector3(0, 0, 0);
					rigid_police.velocity = new Vector3(0, 0, 0);
				}
				else if(hurt_time >= max_hurt_time)
				{
					//volta ao normal
					hurt = false;
					rigid.velocity = BaseVelocity;
					rigid_police.velocity = BaseVelocity;
				}
				
				//socorro matematica
				float change = Mathf.SmoothStep(init_vel, end_vel, hurt_time / (max_hurt_time - vel_hurt_time_mod));
				//muda a posição do alvo da camera
				transf_target.Translate(Vector3.right * change);
				//muda a posição do policial
				transf_police.Translate(Vector3.right * change);
				
				hurt_time += Time.deltaTime;
				
				//cancela o resto do update
				//player ainda pode dar buffer de um high jump
				return;
			}
			else//morre
			{
				//animação, timer etc
				
				//recarrega a scene atual
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
		
		//checa se o jogador está no chão com raycast
		landed = Physics.Raycast(transform.position + (Vector3.up * 0.1f), -Vector3.up, land_ray_height);
		Debug.DrawRay(transform.position, -Vector3.up * land_ray_height, Color.red);
		print(landed);
		
		//se o jogador está no chão
		if(landed)
		{
			//slide
			if(sliding)
			{
				//tira a hurtbox da cabeça
				if(head_col.enabled) head_col.enabled = false;

				//reseta o timer do pulo
				jump_charge = 0;
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
		
		//determina a animação de pulo/andar
		anim.SetFloat("vel_Y", rigid.velocity.y);
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
	
	void OnTriggerEnter(Collider other)
	{
		//se for um obstaculo
		if(other.gameObject.CompareTag("Hazard"))
		{
			//desativa o obstaculo
			other.transform.position += new Vector3(0, 0, 5);
			
			//deixa a cor do obstaculo transparente
			//indica que o obstaculo foi desativado
			Color color = other.GetComponent<SpriteRenderer>().color;
			color.a = 0.5f;
			other.GetComponent<SpriteRenderer>().color = color;
			
			lives--;
			hurt_time = 0;
			hurt = true;
		}
	}
}