using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnTime : MonoBehaviour
{
	[SerializeField] bool _3D;//se o jogo ta no 3D
	
	[SerializeField] float start_time;//tempo até disabilitar
	
    void OnEnable()
	{
		Invoke("Disable", start_time);
	}
	
	void Disable()
	{
		if (!_3D)
		{
			if (gameObject.CompareTag("Hazard"))
			{
				//cor do sprite
				Color color = GetComponent<SpriteRenderer>().color;

				//se estiver transparente
				if (color.a < 1f)
				{
					//ativa o obstaculo
					transform.position += new Vector3(0, 0, -10);

					//indica que o obstaculo foi ativado
					//tira a transparência do objeto
					color.a = 1f;
					GetComponent<SpriteRenderer>().color = color;
				}
			}

			gameObject.SetActive(false);
		}
		else
		{
			if (gameObject.CompareTag("Hazard"))
			{
				Renderer render = transform.GetChild(0).GetComponent<Renderer>();

				if (render != null)
				{
					//cor do material
					Color color = render.material.color;

					//se estiver transparente
					if (color.a < 1f)
					{
						//ativa o obstaculo
						transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

						//tira opção de transparência
						ToOpaqueMode(render.material);

						//indica que o obstaculo foi ativado
						//tira a transparência do objeto
						color.a = 1f;
						render.material.color = color;
					}
				}
			}
			
			gameObject.SetActive(false);
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
