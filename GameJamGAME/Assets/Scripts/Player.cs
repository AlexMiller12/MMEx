using UnityEngine;
using System.Collections;

public class Player : Singleton<Player> 
{

//-----------------------------------------------------------------CONSTANTS/FIELDS:
	private CameraCollider camCollider;
	private const float GUN_RANGE = 500;
	private const float COLLIDER_RADIUS = 1f;
	private const float MAX_CHARGE = 2.71f;
	public Texture2D crosshairTexture;
	private float crosshairWidth = 50, crosshairHeight = 50; //TODO dynamically set based on resolution
	private float charge = 0;
	private bool charging = false;
	private LevelManager levelManager;

//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	
	void Start() 
	{
		camCollider = CameraCollider.Instance;
		levelManager = LevelManager.Instance;
	}
	
	void OnGUI()
	{
		float currentWidth = (crosshairWidth * (charge + 1));
		float currentHeight = (crosshairHeight * (charge + 1));
		float top = (Screen.height - currentHeight) / 2;
		float left = (Screen.width - currentWidth) / 2;
		Rect position = new Rect(left, top, currentWidth, currentHeight);
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
				Debug.Log ("Player --- TODO kill");
				strikingBlock.stopExtruding();
			}
		}
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z)) 
		{
			charging = true;
		}
		if (Input.GetKeyUp(KeyCode.Z))
		{
			shootRay();
			charging = false;
			charge = 0;
		}
		if(charging)
		{
			charge += Time.deltaTime;
			if(charge > MAX_CHARGE)
			{
				charge = MAX_CHARGE;
			}
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			levelManager.resetBlocks();
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
	
	public void push(Vector3 direction, float forcePower)
	{
		transform.rigidbody.AddForce(direction * forcePower);
	}
}
