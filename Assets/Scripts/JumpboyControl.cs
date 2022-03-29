using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpboyControl : MonoBehaviour
{
	bool landed;//se o jogador está no chão
	
	bool jump_holding;//se o player está carregando um pulo
	float jump_charge, max_jump_charge;//carga do pulo atual e máxima
	
	Rigidbody rigid;
	
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	//update pra física
	void FixedUpdate()
	{
		
	}
	
	#region buttons
	//botão de pulo apertado
	public void JumpButtonPress()
	{
		jump_holding = true;
		jump_charge = 0;
	}
	
	//botão de pulo soltado
	public void JumpButtonRelease()
	{
		jump_holding = false;
	}
	#endregion
}
