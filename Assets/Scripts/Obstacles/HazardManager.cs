using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D

	#region pools
	[System.Serializable]
    public class Pool//informações da pool
    {
		public string tag;
		public GameObject prefab;
		public int size;
	}
	public List<Pool> pools;//lista da pool
	public Dictionary<string, Queue<GameObject>> poolDictionary;
    #endregion

    [Header("Backgrounds")]//objetos spawnados no background
	//tempo de spawn atual e maximo pro BG
	[SerializeField] float BG_spawn_time, total_BG_spawn_time;

	//[SerializeField] GameObject[] bg_obj;//array de background
	int bg_obj;//numero de backgrounds (ou ruas no 3D)
	[SerializeField] string[] bg_tag;//array de tags de background
	int predio_obj;//numero de predios
	[SerializeField] string[] predio_tag;//array de tags de predio
	[SerializeField] int lanes_onStart;//quantas lanes de rua/prédio tem quando o jogo inicia
	Quaternion predio2_rot;
	
	[Header("Obstaculos")]//obstaculos spawnados
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	//modifier do spawn_time que varia com o tempo
	[SerializeField] float spawn_time_mod, spawn_time_add;//add é o valor somado, mod é o atual

	//[SerializeField] GameObject[] hazards;//array de hazards
	int hazards;//numero de hazards
	[SerializeField] string[] hz_tag;//array de tags de hazard
	[SerializeField] Transform SpawnPos, BG_SpawnPos;//posição que os obstaculos spawnam
	int bg_loop;//numero de vezes que a rua spawnou

	[SerializeField] JumpboyControl JC;//script do jumpboy
	
    void Awake()
    {
		QualitySettings.vSyncCount = 0;
		
		//cria o dicionario da pool
		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		//cria os objetos das pools
		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();

			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate(pool.prefab);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}

			poolDictionary.Add(pool.tag, objectPool);
		}

		bg_obj = bg_tag.Length;
		hazards = hz_tag.Length;
		
		//spawna as ruas/prédios iniciais
		if(_3D)
		{
			predio_obj = predio_tag.Length;
			
			//rotação do predio 2, pra ele ficar do outro lado
			predio2_rot = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
			
			for(int i = 0; i < lanes_onStart; i++)
			{
				Vector3 posit = new Vector3(15 * (i-1), -0.58f, 0);
				
				//ruas
				SpawnFromPool(bg_tag[0], posit, transform.rotation).SetActive(true);
				
				//predio 1
				//aleatoriza o predio
				int no = Random.Range(0, predio_obj);
				string tag = predio_tag[no];
				//cria o obj de predio
				SpawnFromPool(tag, posit, transform.rotation).SetActive(true);
				
				//predio 2
				//aleatoriza o outro predio
				no = Random.Range(0, predio_obj);
				tag = predio_tag[no];
				//cria o outro obj de predio
				SpawnFromPool(tag, posit, predio2_rot).SetActive(true);
			}
		}
    }
	
	void Start()
	{
		if (_3D)
		{
			//cria as ruas e obstaculos
			InvokeRepeating("Streets", 0, 0.1f);
		}
		else
		{
			//tempo até o primeiro obstaculo aparecer
			spawn_time = total_spawn_time / 2;

			//faz a função SpawnTimers() rodar a cada 0.5 sec
			InvokeRepeating("SpawnTimers", 0, 0.5f);
		}
	}

	#region obj pool
	//instantiate:
	//SpawnFromPool(string tag, transform.position, Quaternion.identity);
	//spawna um objeto
	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
		if (!poolDictionary.ContainsKey(tag))
        {
			Debug.LogWarning("Pool tag error.");
			return null;
        }
		
		//tira o objeto da queue para mudar a posição
		GameObject objectToSpawn = poolDictionary[tag].Dequeue();
		//muda a posição
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		//atualiza o objeto na queue
		poolDictionary[tag].Enqueue(objectToSpawn);

		return objectToSpawn;
    }
	#endregion

	//timer hazards
	void SpawnTimers()
    {
		if(!JC.hurt)
		{
			//timers
			spawn_time -= 0.5f;//Time.deltaTime;
			
			if(spawn_time <= 0)
			{
				//aleatoriza o próximo obstaculo
				int no = Random.Range(0, hazards);
				string tag = hz_tag[no];

				//cria um novo obstaculo
				SpawnFromPool(tag, SpawnPos.position, transform.rotation).SetActive(true);
				//Instantiate(hazards[no], SpawnPos.position, transform.rotation);
				
				//reseta o timer
				spawn_time = total_spawn_time - spawn_time_mod;
				
				//? substituir por um aumento da velocidade dos obstaculos
				//? adicionar os dois
				//matematica pra lentamente diminuir o spawn_time
				spawn_time_mod += (spawn_time_mod < total_spawn_time / 2) ? spawn_time_add : 0;
			}

			BG_spawn_time -= 0.5f;//Time.deltaTime;

			if (BG_spawn_time <= 0)
			{
				//aleatoriza um obj de background
				int no = Random.Range(0, bg_obj);
				string tag = bg_tag[no];

				//cria o obj de background
				SpawnFromPool(tag, BG_SpawnPos.position, transform.rotation).SetActive(true);
				//Instantiate(bg_obj[no], BG_SpawnPos.position, transform.rotation);

				//reseta o timer
				BG_spawn_time = total_BG_spawn_time;
			}
		}
    }
	
	//pos ruas
	void Streets()
    {
		if(JC.transform.position.x > 15 * bg_loop)
        {
			//define a posição
			float diff = JC.transform.position.x - (15 * bg_loop);
			
			Vector3 pos_bg = new Vector3(JC.transform.position.x + 135 - diff, BG_SpawnPos.position.y, BG_SpawnPos.position.z);
			Vector3 pos_hz = new Vector3(pos_bg.x + 7.5f, pos_bg.y, pos_bg.z);

			//aleatoriza um obj de rua
			int no = Random.Range(0, bg_obj);
			string tag = bg_tag[no];
			//cria o obj de rua
			SpawnFromPool(tag, pos_bg, transform.rotation).SetActive(true);
			
			//aleatoriza o predio
			no = Random.Range(0, predio_obj);
			tag = predio_tag[no];
			//cria o obj de predio
			SpawnFromPool(tag, pos_bg, transform.rotation).SetActive(true);
			//aleatoriza o outro predio
			no = Random.Range(0, predio_obj);
			tag = predio_tag[no];
			//cria o outro obj de predio
			SpawnFromPool(tag, pos_bg, predio2_rot).SetActive(true);
			
			bg_loop++;

			if (bg_loop < 20)
			{
				if (bg_loop % 3 == 0)
				{
					//aleatoriza o próximo obstaculo
					no = Random.Range(0, hazards);
					tag = hz_tag[no];
					//cria um novo obstaculo
					SpawnFromPool(tag, pos_hz, transform.rotation).SetActive(true);
				}
			}
			else if (bg_loop < 40)
			{
				if (bg_loop % 2 == 0)
				{
					//aleatoriza o próximo obstaculo
					no = Random.Range(0, hazards);
					tag = hz_tag[no];
					//cria um novo obstaculo
					SpawnFromPool(tag, pos_hz, transform.rotation).SetActive(true);
				}
			}
			else
            {
				//aleatoriza o próximo obstaculo
				no = Random.Range(0, hazards);
				tag = hz_tag[no];
				//cria um novo obstaculo
				SpawnFromPool(tag, pos_hz, transform.rotation).SetActive(true);
			}
        }
    }

	void OnTriggerEnter(Collider other)
	{
		//destroi o obstaculo/objeto do BG quando ele passa de um ponto fora da tela
		if(other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Background"))
        {
			if (!_3D)
			{
				if (other.gameObject.CompareTag("Hazard"))
				{
					//cor do sprite
					Color color = other.GetComponent<SpriteRenderer>().color;

					//se estiver transparente
					if (color.a < 1f)
					{
						//ativa o obstaculo
						other.transform.position += new Vector3(0, 0, -10);

						//indica que o obstaculo foi ativado
						//tira a transparência do objeto
						color.a = 1f;
						other.GetComponent<SpriteRenderer>().color = color;
					}
				}

				other.gameObject.SetActive(false);
			}
			else
            {
				if (other.gameObject.CompareTag("Hazard"))
				{
					Renderer render = other.transform.GetChild(0).GetComponent<Renderer>();

					if (render != null)
					{
						//cor do material
						Color color = render.material.color;

						//se estiver transparente
						if (color.a < 1f)
						{
							//ativa o obstaculo
							other.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

							//tira opção de transparência
							ToOpaqueMode(render.material);

							//indica que o obstaculo foi ativado
							//tira a transparência do objeto
							color.a = 1f;
							render.material.color = color;
						}
					}
				}
				
				other.gameObject.SetActive(false);
            }

			//Destroy(other.gameObject);
		}
	}
	
	//transforma o material em um material opaco
	static void ToOpaqueMode(Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }
}
