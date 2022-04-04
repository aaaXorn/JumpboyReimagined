using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
	[Header("Backgrounds")]//objetos spawnados no background
	//tempo de spawn atual e maximo pro BG
	[SerializeField] float BG_spawn_time, total_BG_spawn_time;

	[SerializeField] GameObject[] bg_obj;//array de background
	
	[Header("Obstaculos")]//obstaculos spawnados
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	//modifier do spawn_time que varia com o tempo
	[SerializeField] float spawn_time_mod, spawn_time_add;//add é o valor somado, mod é o atual

	[SerializeField] GameObject[] hazards;//array de hazards
	[SerializeField] Transform SpawnPos, BG_SpawnPos;//posição que os obstaculos spawnam
	
	[SerializeField] JumpboyControl JC;//script do jumpboy
	
    void Start()
    {
		//tempo até o primeiro obstaculo aparecer
        spawn_time = total_spawn_time/2;
    }
	
    void Update()
    {
		if(!JC.hurt)
		{
			//timers
			spawn_time -= Time.deltaTime;
			BG_spawn_time -= Time.deltaTime;
			
			if(spawn_time <= 0)
			{
				//aleatoriza o próximo obstaculo
				int no = Random.Range(0, hazards.Length);
				print(no);
				//cria um novo obstaculo
				Instantiate(hazards[no], BG_SpawnPos.position, transform.rotation);
				
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
				int no = Random.Range(0, bg_obj.Length);

				//cria o obj de background
				Instantiate(bg_obj[no], SpawnPos.position, transform.rotation);

				//reseta o timer
				BG_spawn_time = total_BG_spawn_time;
			}
		}
    }
	
	void OnTriggerEnter(Collider other)
	{
		//destroi o obstaculo/objeto do BG quando ele passa de um ponto fora da tela
		if(other.gameObject.CompareTag("Hazard") || other.gameObject.CompareTag("Background"))
			Destroy(other.gameObject);
	}
}
