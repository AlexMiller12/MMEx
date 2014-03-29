using UnityEngine;
using System.Collections;

public class Player : Singleton<Player> 
{

//-----------------------------------------------------------------CONSTANTS/FIELDS:
	private CameraCollider camCollider;
	private const float GUN_RANGE = 50;
	private const float COLLIDER_RADIUS = 1f;
	public Texture2D crosshairTexture;
	private int crosshairWidth = 100, crosshairHeight = 100; //TODO dynamically set based on resolution
	private int charge;

//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	
	void Start() 
	{
		charge = 1;
		camCollider = CameraCollider.Instance;
	}
	
	void OnGUI()
	{
		float top = (Screen.height - crosshairHeight) / 2;
		float left = (Screen.width - crosshairWidth) / 2;
		Rect position = new Rect(left, top, crosshairWidth, crosshairHeight);
		GUI.DrawTexture(position, crosshairTexture);
	}
	
	bool isColliderStriking(Collision collision, Block strikingBlock)
	{
		Vector3 dirOfCollider = collision.transform.position - transform.position;
		return Vector3.Dot(dirOfCollider, strikingBlock.ExtrusionDirection) < 0.0f;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Player --- Bumped into something!");
		Block strikingBlock = collision.collider.gameObject.GetComponent<Block>();
		if (strikingBlock != null && isColliderStriking(collision, strikingBlock))
		{
			Vector3 direction = strikingBlock.ExtrusionDirection;
			RaycastHit hit;
			float distance = COLLIDER_RADIUS;
			Vector3 origin = transform.position + direction * COLLIDER_RADIUS;
			if (Physics.Raycast(transform.position, direction, distance)) 
			{
				strikingBlock.stopExtruding();
			}
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)) 
		{
			shootRay();
		}
		if(Input.GetKeyDown(KeyCode.F)) 
		{
			charge++;
		}
		
		//transform.position = camCollider.transform.position;
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
	
	public void push(Vector3 direction, float forcePower)
	{
		transform.rigidbody.AddForce(direction * forcePower);
	}
}
