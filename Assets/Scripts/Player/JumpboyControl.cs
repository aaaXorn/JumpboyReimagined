using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpboyControl : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D
	float half_sHeight;//metade da altura da tela

	[Header("Velocity and Misc")]
	Rigidbody rigid;
	Animator anim;
	[SerializeField] Transform transf_police, transf_target;//transform do alvo
	[SerializeField] float rel_pos_police;//posição relativa X do policial em relação ao player
	//velocidade exponencial pra baixo quando toma hit
	[SerializeField] float init_vel, end_vel, vel_hurt_time_mod;//vel_change;
	
	bool landed = true;//se o jogador está no chão
	
	[SerializeField] int lives = 2;//vidas do jogador
	public bool hurt;//enquanto o jogador toma um hit
	[SerializeField] float hurt_time, max_hurt_time;//tempo atual e máximo na animação de dano
	
	[Header("Movement Actions")]
	[SerializeField] Vector3 BaseVelocity;//velocidade do jumpboy correndo
	[SerializeField] float land_ray_height = 0.2f;//raycast do pulo
	bool grounded;//se o player está no chão
	
	#region jump
	bool jump_holding, jumping;//se o player está carregando um pulo/pulando
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
		//pega a largura da tela
		half_sHeight = Screen.height / 2;

        rigid = GetComponent<Rigidbody>();
		
		//movimento horizontal
		rigid.velocity = BaseVelocity;
		//setta a posição relativa do policial
		rel_pos_police = transf_police.position.x - transform.position.x;

		anim = GetComponent<Animator>();
    }
	
	//física do jogo
	void FixedUpdate()
	{
		#region inputs
		if(Input.touchCount > 0)//se o jogador está tocando na tela
        {
			Touch touch = Input.GetTouch(0);//pega o input de toque

			//pulo
			if(touch.position.y <= half_sHeight)//|| !_3D
            {
				jump_holding = true;
				sliding = false;
            }
			//slide
			else
            {
				jump_holding = false;
				sliding = true;
            }
        }
        else
        {
//se fora do editor da Unity (na hora de buildar)
#if !UNITY_EDITOR
			jump_holding = false;
			sliding = false;
#endif
		}
		#endregion

		//movimento do policial
		transf_police.position = new Vector3(transform.position.x + rel_pos_police, transf_police.position.y, transf_police.position.z);
		
		//se tomou dano
		if(hurt)
		{
			if(lives > 0)//continua vivo
			{
				if(hurt_time <= 0)
				{
					//deixa o jogador parado, talvez mude
					rigid.velocity = new Vector3(0, 0, 0);
				}
				else if(hurt_time >= max_hurt_time)
				{
					//volta ao normal
					hurt = false;
					rigid.velocity = BaseVelocity;
				}
				
				//socorro matematica
				float change = Mathf.SmoothStep(init_vel, end_vel, hurt_time / (max_hurt_time - vel_hurt_time_mod));
				//muda a posição do alvo da camera
				transf_target.Translate(Vector3.right * change);
				//muda a posição do policial
				rel_pos_police += change;

				hurt_time += Time.deltaTime;
				
				//cancela o resto do update
				return;
			}
			else//morre
			{
				//animação, timer etc
				
				//recarrega a scene atual
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

				//cancela o resto do update
				return;
			}
		}

		//pulo carregado
		if (jumping && jump_holding)
		{
			//se o timer passar do máximo
			if (jump_charge >= max_jump_charge)
			{
				jumping = false;
			}
			else
			{
				//timer
				jump_charge += Time.deltaTime;

				//força do pulo
				rigid.AddForce(new Vector3(0, jump_base_force * Time.deltaTime, 0), ForceMode.Impulse);
			}
		}
		else jumping = false;

		//checa se o jogador está no chão com raycast
		landed = Physics.Raycast(transform.position + (Vector3.up * 0.1f), -Vector3.up, land_ray_height);
		Debug.DrawRay(transform.position, -Vector3.up * land_ray_height, Color.red);

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
				if(jump_holding && !jumping)
				{
					//força do pulo
					rigid.AddForce(new Vector3(0, jump_base_force, 0), ForceMode.Impulse);

					jumping = true;
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
		if(!hurt && other.gameObject.CompareTag("Hazard"))
		{
			if (!_3D)
			{
				//desativa o obstaculo
				other.transform.position += new Vector3(0, 0, 10);

				//deixa a cor do obstaculo transparente
				//indica que o obstaculo foi desativado
				Color color = other.GetComponent<SpriteRenderer>().color;
				color.a = 0.5f;
				other.GetComponent<SpriteRenderer>().color = color;
			}
			
			lives--;
			hurt_time = 0;
			hurt = true;
		}
	}
}