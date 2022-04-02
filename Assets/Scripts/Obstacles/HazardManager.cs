using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	//spawn time modifier que varia com o obstaculo?
	
	//fazer o spawn time diminuir com o tempo?
	
	[Header("Backgrounds")]//objetos spawnados no background
	[SerializeField] GameObject[] bg_obj;
	
	[Header("Obstaculos")]//obstaculos spawnados
	[SerializeField] GameObject[] hazards;
	[SerializeField] Transform SpawnPos;//posição que os obstaculos spawnam
	
    void Start()
    {
		//tempo até o primeiro obstaculo aparecer
        //spawn_time = total_spawn_time/2;
    }
	
    void Update()
    {
        spawn_time -= Time.deltaTime;
		
		if(spawn_time <= 0)
		{
			//aleatoriza o próximo obstaculo
			int no = Random.Range(0, hazards.Length);
			
			//cria um novo obstaculo
			Instantiate(hazards[no], SpawnPos.position, transform.rotation);
			
			//reseta o timer
			spawn_time = total_spawn_time;
			
			//matematica pra lentamente diminuir o spawn_time
		}
    }
	
	void OnTriggerEnter(Collider other)
	{
		print("trigger");
		//destroi o obstaculo quando ele passa de um ponto fora da tela
		if(other.gameObject.CompareTag("Hazard"))
			Destroy(other.gameObject);
		//destroy o objeto de BG e cria um novo
		else if(other.gameObject.CompareTag("Background"))
		{
			Destroy(other.gameObject);
			
			//aleatoriza um obj de background
			int no = Random.Range(0, bg_obj.Length);
			
			//cria o obj de background
			Instantiate(bg_obj[no], SpawnPos.position, transform.rotation);
		}
	}
}
