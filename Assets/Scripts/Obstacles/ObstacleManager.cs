using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	
	//fazer o spawn time diminuir com o tempo?
	
	[Header("Obstaculos")]//obstaculos spawnados
	[SerializeField] GameObject placeholder;
	[SerializeField] Transform SpawnPos;//posição que os obstaculos spawnam
	
    void Start()
    {
		//tempo até o primeiro obstaculo aparecer
        spawn_time = total_spawn_time/2;
    }
	
    void Update()
    {
        spawn_time -= Time.deltaTime;
		
		if(spawn_time == 0)
		{
			//cria um novo obstaculo
			
			//reseta o timer
			spawn_time = total_spawn_time;
		}
    }
	
	void OnTriggerEnter(Collider other)
	{
		//destroi o obstaculo quando ele passa de um ponto fora da tela
		if(other.gameObject.CompareTag("Obstacle"))
			Destroy(other.gameObject);
	}
}
