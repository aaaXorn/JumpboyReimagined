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
	[SerializeField] int bg_obj;//numero de backgrounds
	[SerializeField] string[] bg_tag;//array de tags de background

	[Header("Obstaculos")]//obstaculos spawnados
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	//modifier do spawn_time que varia com o tempo
	[SerializeField] float spawn_time_mod, spawn_time_add;//add é o valor somado, mod é o atual

	//[SerializeField] GameObject[] hazards;//array de hazards
	[SerializeField] int hazards;//numero de hazards
	[SerializeField] string[] hz_tag;//array de tags de hazard
	[SerializeField] Transform SpawnPos, BG_SpawnPos;//posição que os obstaculos spawnam
	
	[SerializeField] JumpboyControl JC;//script do jumpboy
	
    void Start()
    {
		//cria o dicionario da pool
		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		//cria os objetos das pools
		foreach(Pool pool in pools)
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

		//tempo até o primeiro obstaculo aparecer
        spawn_time = total_spawn_time/2;
		
		//faz a função SpawnTimers() rodar a cada 0.5 sec
		InvokeRepeating("SpawnTimers", 0, 0.5f);
    }

	#region obj pool
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
	//instantiate:
	//SpawnFromPool(string tag, transform.position, Quaternion.identity);
	#endregion

	//InvokeRepeating
	void SpawnTimers()//Update()
    {
		if(!JC.hurt)
		{
			//timers
			spawn_time -= 0.5f;//Time.deltaTime;
			BG_spawn_time -= 0.5f;//Time.deltaTime;
			
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

			if(BG_spawn_time <= 0)
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
					//cor do material
					Color color = other.transform.GetChild(0).GetComponent<Renderer>().material.color;

					//se estiver transparente
					if (color.a < 1f)
					{
						//ativa o obstaculo
						other.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

						//indica que o obstaculo foi ativado
						//tira a transparência do objeto
						color.a = 1f;
						other.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
					}
				}

				other.gameObject.SetActive(false);
            }

			//Destroy(other.gameObject);
		}
	}
}
