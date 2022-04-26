using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class CameraControl : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo está em 2D ou 3D

	[SerializeField] Transform target;//quem a camera segue
	[SerializeField] Vector3 offset;//diferência entre o vetor da camera e do player
	
	//[SerializeField] float smoothSpd;//usado na velocidade da camera

	[SerializeField] Transform transf_manager;//chão e managers de hazard
	
	[SerializeField] float lensD_intensity, lensD_centerY;//intensidade e centro Y da distorção de lente

	[SerializeField] Text text_fps;
	float fps_counter_time;//timer do contador de FPS pra não ficar no modo eplepsia

	void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = -1;

		if(_3D)
		{
			//pega o post process volume
			PostProcessVolume vol = GetComponent<PostProcessVolume>();
			//variável de referência da distorção de lente
			LensDistortion lensD;
			
			vol.profile.TryGetSettings(out lensD);
			lensD.intensity.value = (StaticVars.ConcaveCam ? lensD_intensity : 0);
			lensD.centerY.value = (StaticVars.ConcaveCam ? lensD_centerY : 0);
		}

		//mostra o FPS
		int current_frame = (int)(1f / Time.unscaledDeltaTime);
		text_fps.text = "FPS: " + current_frame;
	}
	
	//movimento da camera
    void LateUpdate()//roda depois do update
    {
		//posição desejada
		Vector3 desiredPos = target.position + offset;
		//tirei o smoothing que tava meio travado, aqui o código antigo se precisar pra algo
		/*Vector3 smoothedPos;
		
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
		transf_manager.position = new Vector3(target.position.x, transf_manager.position.y, transf_manager.position.z);
		*/
		
		//muda a posição da camera
		transform.position = new Vector3(desiredPos.x, offset.y, offset.z);
		transf_manager.position = new Vector3(target.position.x, transf_manager.position.y, transf_manager.position.z);

		fps_counter_time += Time.unscaledDeltaTime;
		if (fps_counter_time > 1)
		{
			//mostra o FPS
			int current_frame = (int)(1f / Time.unscaledDeltaTime);
			text_fps.text = "FPS: " + current_frame;

			fps_counter_time = 0;

			print("a");
		}
	}
}