using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reenable : MonoBehaviour
{
	Collider col;
	
	void Awake()
	{
		col = GetComponent<BoxCollider>();
	}
	
    void OnEnable()
	{
		col.enabled = true;
	}
}
