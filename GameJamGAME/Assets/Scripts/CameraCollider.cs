using UnityEngine;
using System.Collections;

public class CameraCollider : Singleton<CameraCollider> 
{


//-----------------------------------------------------------------CONSTANTS/FIELDS:
	
	CameraCollider camCollider;
	
//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	void Start () 
	{
		//camCollider = gameObject.GetComponent<CameraCollider>();
		//camCollider.transform.position = transform.position;
	}
	
	void Update()
	{
	}
}
