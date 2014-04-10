using UnityEngine;
using System.Collections;

public class GUIPlayer : Singleton<GUIPlayer> 
{

//-----------------------------------------------------------------CONSTANTS/FIELDS:
	
	private const float GUN_RANGE = 1000;
	private const float COLLIDER_RADIUS = 1f;
	private const float MAX_CHARGE = 2.71f;
	public TextMesh startBut, contBut, exitBut;
	public Texture2D crosshairTexture;
	private float crosshairWidth = 50, crosshairHeight = 50; //TODO dynamically set based on resolution
	private float charge = 0;
	private bool charging = false;
	
//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	
	void Start() 
	{
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
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			charging = true;
		}
		if (Input.GetMouseButtonUp(0))
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
	}
	
//--------------------------------------------------------------------------METHODS:

	public void shootRay() 
	{
 		Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
		Ray ray = Camera.main.ViewportPointToRay(screenCenter);
		Vector3 dest = ray.direction * GUN_RANGE + ray.origin;
		RaycastHit hit;
		Debug.DrawLine(ray.origin, dest);
		if (Physics.Raycast(ray.origin, dest, out hit))
		{
			if(hit.transform == exitBut.transform)
			{
				exitBut.fontStyle = FontStyle.Bold;
				Application.Quit();
			}
			else if(hit.transform == startBut.transform)
			{
				startBut.fontStyle = FontStyle.Bold;
				//Application.LoadLevel("DevScene"); //TODO load a scene
			}
			
		}
		
	}
}
