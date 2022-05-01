using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpaqueOnStart : MonoBehaviour
{
    void Start()
    {
		Renderer render = transform.GetChild(0).GetComponent<Renderer>();
		
        ToOpaqueMode(render.material);
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
