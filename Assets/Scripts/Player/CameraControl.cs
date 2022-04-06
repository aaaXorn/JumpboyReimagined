using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D

	[SerializeField] Transform target;//quem a camera segue
	[SerializeField] Vector3 offset;//diferência entre o vetor da camera e do player
	
	[SerializeField] float smoothSpd;//usado na velocidade da camera
	
	//movimento da camera
    void LateUpdate()//roda depois do update
    {
		//posição desejada
		Vector3 desiredPos = target.position + offset;
		Vector3 smoothedPos;
		
		if(_3D)
		{
			//velocidade da camera
			float camSpd = smoothSpd * Time.deltaTime;
			//determina a posição final da camera nesse frame
			smoothedPos = Vector3.Lerp(transform.position, desiredPos, camSpd);
			
			//rotaciona a camera
			//talvez seja trocado por uma rotação base inicial que não muda
			//transform.LookAt(target);//rotaciona a camera na direção do player
		}
		else
		{
			//muda a posição da camera
			smoothedPos = desiredPos;
		}
		
		//muda a posição da camera
		transform.position = new Vector3(smoothedPos.x, offset.y, offset.z);
    }
}
