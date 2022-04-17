using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JumpboyControl : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D
	float half_sHeight;//metade da altura da tela

	[Header("Velocity and Misc")]
	Rigidbody rigid;
	Animator anim;
	[SerializeField] Transform transf_police;
	[SerializeField] Transform transf_target;//transform do alvo
	float rel_pos_police;//posição relativa X do policial em relação ao player
	//velocidade exponencial decrescente quando toma hit
	[SerializeField] float init_vel, end_vel, vel_hurt_time_mod;//vel_change;
	
	bool landed = true;//se o jogador está no chão
	
	int lives = 2;//vidas do jogador
	public bool hurt;//enquanto o jogador toma um hit
	[SerializeField] float hurt_time, max_hurt_time;//tempo atual e máximo na animação de dano
	
	[Header("Movement Actions")]
	[SerializeField] Vector3 BaseVelocity;//velocidade do jumpboy correndo
	[SerializeField] float land_ray_height = 0.2f, land_init_height = 0.1f;//raycast do pulo
	bool grounded;//se o player está no chão
	
	#region jump
	bool jump_holding, jumping;//se o player está carregando um pulo/pulando
	[SerializeField] float jump_charge, max_jump_charge;//carga do pulo atual e máxima
	
	[SerializeField] float jump_base_force, jump_charge_force;//força base e por carregar do pulo
	[SerializeField] float grav_mod = 1f;//modificador de gravidade
	#endregion
	
	#region slide
	bool sliding;//se o jogador está dando um slide
	
	[SerializeField]
	Collider head_col;//collider da cabeça, desabilitado durante o slide
	#endregion
	
	#region swipe
	[Header("Swipe")]
	[SerializeField] float minSwipeLength;//mínimo pra contar como swipe
	Vector2 touchStart;//começo do swipe
	
	int lane, previous_lane;//qual o player vai se mover para e anterior
	[SerializeField] float[] lane_pos;//posições da lane
	
	bool side_movement;//se o jogador está se movendo na lateral
	//timers do movimento
	[SerializeField] float side_timer, max_side_timer;
	#endregion
	
	#region score
	[SerializeField] Text scr_txt, h_scr_txt;//texto do score e high score
	[SerializeField] int high_score;//texto do high score
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
		
		//pega o high score
		if(!_3D)
		{
			if(PlayerPrefs.HasKey("hScore"))
				high_score = PlayerPrefs.GetInt("hScore");
		}
		else
		{
			if(PlayerPrefs.HasKey("3d_hScore"))
				high_score = PlayerPrefs.GetInt("3d_hScore");
		}
		
		h_scr_txt.text = "HIGH SCORE " + high_score;
    }
	
	//pega os inputs
	void Update()
	{
		#region inputs
		#if !UNITY_EDITOR
		if(Input.touchCount > 0)//se o jogador está tocando na tela
        {
			Touch touch = Input.GetTouch(0);//pega o input de toque

			//slide
			if(touch.position.y <= half_sHeight && _3D)
            {
				jump_holding = false;
				sliding = true;
				
				#region swipe
				if(!side_movement && lane == previous_lane)//se o jogador não está já se movendo
				{
					if(touch.phase == TouchPhase.Began)
					{
						//posição inicial do swipe
						touchStart = touch.position;
					}
					else if(touch.phase == TouchPhase.Ended)
					{
						//posição final do swipe
						Vector2 touchEnd = touch.position;
						//vetor do swipe
						Vector3 swipe = new Vector3(touchEnd.x - touchStart.x, touchEnd.y - touchStart.y);
						
						//se o tamanho do swipe não é pequeno demais
						if(swipe.magnitude >= minSwipeLength)
						{
							//pra esquerda
							if(swipe.x > 0)
							{
								if(lane > -1) lane--;
							}
							//pra direita
							else if(swipe.x < 0)
							{
								if(lane < 1) lane++;
							}
							
							side_movement = true;
						}
					}
				}
				#endregion
            }
			//pulo
			else
            {
				jump_holding = true;
				sliding = false;
            }
        }
        else
        {
			jump_holding = false;
			sliding = false;
		}
		#endif
		
		#if UNITY_EDITOR
		if(Input.GetMouseButton(0))//se o jogador está tocando na tela
        {
			Vector3 mousePos = Input.mousePosition;//pega a posição do mouse
			//slide
			if(mousePos.y <= half_sHeight && _3D)
            {
				jump_holding = false;
				sliding = true;
				
				#region swipe
				if(!side_movement && lane == previous_lane)//se o jogador não está já se movendo
				{
					if(Input.GetMouseButtonDown(0))
					{
						//posição inicial do swipe
						touchStart = Input.mousePosition;
					}
				}
				#endregion
            }
			//pulo
			else
            {
				jump_holding = true;
				sliding = false;
            }
        }
        else
        {
			jump_holding = false;
			sliding = false;
		}
		
		if(_3D && lane == previous_lane && Input.mousePosition.y <= half_sHeight && Input.GetMouseButtonUp(0))
		{
			//posição final do swipe
			Vector2 touchEnd = Input.mousePosition;
			//vetor do swipe
			Vector3 swipe = new Vector3(touchEnd.x - touchStart.x, touchEnd.y - touchStart.y);
			
			//se o tamanho do swipe não é pequeno demais
			if(swipe.magnitude >= minSwipeLength)
			{
				//pra esquerda
				if(swipe.x > 0)
				{
					if(lane > -1) lane--;
				}
				//pra direita
				else if(swipe.x < 0)
				{
					if(lane < 1) lane++;
				}
				
				side_movement = true;
			}
		}
		
		#endif
		
		#endregion
	}
	
	//física do jogo
	void FixedUpdate()
	{
		//aplica gravidade
		rigid.AddForce(Physics.gravity * grav_mod, ForceMode.Acceleration);
		
		//movimento do policial
		transf_police.position = new Vector3(transform.position.x + rel_pos_police, transf_police.position.y, transf_police.position.z);
		
		#region damage
		//se tomou dano
		if(hurt)
		{
			if(lives > 0)//continua vivo
			{
				if(hurt_time <= 0)
				{
					//deixa o jogador parado, talvez mude
					rigid.velocity = new Vector3(0, 0, 0);
					
					anim.SetTrigger("hurt");
				}
				else if(hurt_time >= max_hurt_time)
				{
					//volta ao normal
					hurt = false;
					rigid.velocity = BaseVelocity;
					
					anim.SetTrigger("hurt");
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
				//animação
				if(hurt_time <= 0)
				{
					anim.SetTrigger("morreu");
					
					//deixa o jogador parado
					rigid.velocity = new Vector3(0, 0, 0);
				}
				else if(hurt_time >= max_hurt_time)
				{
					//define o high score
					int score = (int)Mathf.Round(transform.position.x);
					
					if(score > high_score)
					{
						if(!_3D) PlayerPrefs.SetInt("hScore", score);
						else PlayerPrefs.SetInt("3d_hScore", score);
						
						PlayerPrefs.Save();
					}
					
					//recarrega a scene atual
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
				
				hurt_time += Time.deltaTime;
				
				//cancela o resto do update
				return;
			}
		}
		#endregion
		
		#region hold jump
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
		#endregion

		//checa se o jogador está no chão com raycast
		landed = Physics.Raycast(transform.position + (Vector3.up * land_init_height), -Vector3.up, land_ray_height);
		Debug.DrawRay(transform.position + (Vector3.up * land_init_height), -Vector3.up * land_ray_height, Color.red);
		print(landed);
		
		#region slide and jump
		//se o jogador está no chão
		if(landed)
		{
			//slide
			if(_3D && sliding)
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
        #endregion

        #region sideways movement
        //movimento pros lados
        if (_3D && side_movement)
		{
			//move o jogador pro lado Z
			float currentPos = Mathf.Lerp(lane_pos[previous_lane+1], lane_pos[lane+1], side_timer / max_side_timer);
				
			transform.position = new Vector3(transform.position.x, transform.position.y, currentPos);
			
			//timer pra determinar quando o movimento acaba
			if(side_timer >= max_side_timer)
			{
				side_timer = 0;
				previous_lane = lane;
				side_movement = false;
			}
			else side_timer += Time.deltaTime;
		}
		#endregion
		
		//score atual
		scr_txt.text = "SCORE " + Mathf.Round(transform.position.x);
		
		//determina a animação de pulo/andar
		anim.SetFloat("vel_Y", rigid.velocity.y);
	}
	
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
			else
			{
				//desativa o obstaculo
				other.enabled = false;
				
				//adiciona opção de transparência
				ToFadeMode(other.GetComponent<Renderer>().material);
				
				//deixa a cor do obstaculo transparente
				//indica que o obstaculo foi desativado
				Color color = other.GetComponent<Renderer>().material.color;
				color.a = 0.5f;
				other.GetComponent<Renderer>().material.color = color;
			}
			
			lives--;
			hurt_time = 0;
			hurt = true;
		}
	}
	
	//transforma o material em transparente
	static void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
    }
}