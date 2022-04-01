using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
	//tempo de spawn atual e maximo
	[SerializeField] float spawn_time, total_spawn_time;
	//spawn time modifier que varia com o obstaculo?
	
	//fazer o spawn time diminuir com o tempo?
	
	[Header("Obstaculos")]//obstaculos spawnados
	[SerializeField] GameObject placeholder;
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
			
			//cria um novo obstaculo
			Instantiate(placeholder, SpawnPos.position, transform.rotation);
			
			//reseta o timer
			spawn_time = total_spawn_time;
			
			//matematica pra lentamente diminuir o spawn_time
		}
    }
	
	void OnTriggerEnter(Collider other)
	{
		//destroi o obstaculo quando ele passa de um ponto fora da tela
		if(other.gameObject.CompareTag("Hazard"))
			Destroy(other.gameObject);
	}
}
