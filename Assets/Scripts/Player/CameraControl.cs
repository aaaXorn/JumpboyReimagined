using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[SerializeField] Transform target;//quem a camera segue
	[SerializeField] Vector3 offset;//diferência entre o vetor da camera e do player
	
	[SerializeField] float smoothSpd;//usado na velocidade da camera
	
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D
	
	//movimento da camera
    void LateUpdate()//roda depois do update
    {
		//posição desejada
		Vector3 desiredPos = target.position + offset;
		
		//velocidade da camera
		float camSpd = smoothSpd * Time.deltaTime;
		//determina a posição final da camera nesse frame
		Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, camSpd);
		
		//muda a posição da camera
		transform.position = smoothedPos;
		
		//se for a versão 3D
		//talvez seja trocado por uma rotação base inicial que não muda
		if(_3D) transform.LookAt(target);//rotaciona a camera na direção do player
    }
}