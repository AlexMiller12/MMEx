using UnityEngine;
using System.Collections;

public class Player : Singleton<Player> 
{

//-----------------------------------------------------------------CONSTANTS/FIELDS:
	
	private const float GUN_RANGE = 50;
	public Texture2D crosshairTexture;
	private int crosshairWidth = 100, crosshairHeight = 100; //TODO dynamically set based on resolution
	private int charge;

//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	
	void Start() 
	{
		charge = 1;
		
	}
	
	void OnGUI()
	{
		float top = (Screen.height - crosshairHeight) / 2;
		float left = (Screen.width - crosshairWidth) / 2;
		Rect position = new Rect(left, top, crosshairWidth, crosshairHeight);
		GUI.DrawTexture(position, crosshairTexture);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Player --- Bumped into something!");
		Block strikingBlock = collision.collider.gameObject.GetComponent<Block>();
		if (strikingBlock != null)
		{
			
		//TODO freeze blocks if appropriate
			//isColliding = true;?
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)) 
		{
			shootRay();
		}
	}
	
//--------------------------------------------------------------------------METHODS:
	public void shootRay() 
	{
 		Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
		Ray ray = Camera.main.ViewportPointToRay(screenCenter);
		Vector3 dest = ray.direction * GUN_RANGE + ray.origin;
		RaycastHit hit;
		if (Physics.Raycast(ray.origin, dest, out hit))
		{
			Block block = hit.collider.GetComponent<Block>();
			//TODO check for null
			if (block != null)
			{
				block.handleShot(hit, charge);
			}
		}
		
	}
	
	public Vector3 getPosition() 
	{
		
		return transform.position;
	}
	
	public void push(Vector3 pushDist)
	{
		transform.Translate(pushDist);
	}
}
