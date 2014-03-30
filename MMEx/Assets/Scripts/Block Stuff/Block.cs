using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour 
{

//-----------------------------------------------------------------CONSTANTS/FIELDS:	
	
	private const float EXTRUSION_SCALER = 2.0f;
	private const float EPSILON = 0.01f;
	private const float BASE_SPEED = 1.12f;
	private const float IMPACT_FORCE = 30f;
	
	public SurfaceType xPos, xNeg, yPos, yNeg, zPos, zNeg;
	bool isOscillating, isExtruding; //TODO with anim?
	bool isResetting = false;
	
	private float currentSpeed;
	private float traveled, journeyLength;	
	private float extrusionStartTime;
	private Vector3 extrusionStartPos, extrusionDirection;
	private Vector3 pushDist; // Distance we will push the player on impact
	private Vector3 destPos, destScale, totalScaleAmt, origScale;
	
	private Vector3 initialPosition, initialScale;
	private Player player;
	private LevelManager levelManager;
	
//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	void FixedUpdate()
	{
		if(isResetting)
		{
			
		}
		else if (isExtruding)
		{
			stepExtrusion();
		}
		
	}
	
	void OnCollisionEnter(Collision collision)
	{
		handleCollision(collision);
		Debug.Log("Block --- Bumped into something!");
	}	
	
	void Start() 
	{
		isOscillating = false; //TODO temp?
		isExtruding = false;
		transform.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		player = Player.Instance;
		levelManager = LevelManager.Instance;
		levelManager.addBlock(this);
		initialPosition = transform.position;
		initialScale = transform.localScale;
	}
	
//--------------------------------------------------------------------------METHODS:
	
	private static Vector3 calcScale(FaceType faceType)
	{
		switch(faceType)
		{
		case FaceType.XPOS:
		case FaceType.XNEG:
			return new Vector3(1, 0, 0);

		case FaceType.YPOS:
		case FaceType.YNEG:
			return new Vector3(0, 1, 0);
			
		case FaceType.ZPOS:
		case FaceType.ZNEG:
			return new Vector3(0, 0, 1);

		default:
			Debug.Log("Surface --- Unknown FaceType!");
			return Vector3.zero;
		}
	}
	
	private static Vector3 calcTranslation(FaceType faceType)
	{
		switch(faceType)
		{
		case FaceType.XPOS:
			return new Vector3(1, 0, 0);
		case FaceType.XNEG:
			return new Vector3(-1, 0, 0);
		case FaceType.YPOS:
			return new Vector3(0, 1, 0);
		case FaceType.YNEG:
			return new Vector3(0, -1, 0);
		case FaceType.ZPOS:
			return new Vector3(0, 0, 1);
		case FaceType.ZNEG:
			return new Vector3(0, 0, -1);
		default:
			Debug.Log("Surface --- Unknown FaceType!");
			return Vector3.zero;
		}
		
	}
	
	private FaceType getFaceType(RaycastHit hit)
	{
		if(hit.normal.x > 0)    return FaceType.XPOS;
		else if(hit.normal.x < 0)    return FaceType.XNEG;
		else if(hit.normal.y > 0)    return FaceType.YPOS;
		else if(hit.normal.y < 0)    return FaceType.YNEG;
		else if(hit.normal.z > 0)    return FaceType.ZPOS;
		else   return FaceType.ZNEG;
	}
	
	private SurfaceType getSurfaceType(RaycastHit hit)
	{
		if(hit.normal.x > 0)    return xPos;
		else if(hit.normal.x < 0)    return xNeg;
		else if(hit.normal.y > 0)    return yPos;
		else if(hit.normal.y < 0)    return yNeg;
		else if(hit.normal.z > 0)    return zPos;
		else   return zNeg;
	}
	private SurfaceType getSurfaceCollision(Vector3 normal)
	{
		if(normal.x > 0)    return xPos;
		else if(normal.x < 0)    return xNeg;
		else if(normal.y > 0)    return yPos;
		else if(normal.y < 0)    return yNeg;
		else if(normal.z > 0)    return zPos;
		else   return zNeg;
	}
	
	/*
	 * Returns true if strikes other surface and should stop
	 */
	private void handleCollision(Collision collision) 
	{
		if (collision.collider.gameObject.Equals(player.gameObject))
		{
			Vector3 collisionNormal = collision.contacts[0].normal;
			SurfaceType surfaceHit = getSurfaceCollision(collisionNormal);
			if(surfaceHit == SurfaceType.ELASTIC) {
				handleElasticCollision();
			}
			else {
				player.push(extrusionDirection, currentSpeed * IMPACT_FORCE);
			}
			return;
		}
		foreach(ContactPoint contact in collision.contacts) 
		{
			Vector3 dirNorm = extrusionDirection + contact.normal;
			if (Mathf.Abs(dirNorm.x) < EPSILON &&
				Mathf.Abs(dirNorm.y) < EPSILON &&
				Mathf.Abs(dirNorm.z) < EPSILON)
			{
				Debug.Log("Block --- I'm freezing!");
				stopExtruding();
				return;
			}
		}
	}
	
	public void handleElasticCollision()
	{
	}
	
	public void handleShot(RaycastHit hit, float shotCharge)
	{
		if(!isExtruding) 
		{			
			SurfaceType surfaceType = this.getSurfaceType(hit);
			FaceType faceType = this.getFaceType(hit);
			switch(surfaceType)
			{
			case SurfaceType.DEAD:
				break;
			case SurfaceType.PERM:
				this.permImpact(faceType, shotCharge);
				break;
			case SurfaceType.TEMP:
				//TODO
				break;
			case SurfaceType.REFLECTIVE:
				//TODO
				break;
			default:
				Debug.Log("Block --- Unknown surface type!");
				break;
			}
		}
	}
	
	public void reset()
	{
		stopExtruding();
		//transform.localScale = initialScale;
		//transform.position = initialPosition;
		startReset(initialPosition, initialScale);
	}
	
	
	private void startExtrusion(Vector3 desiredTranslation, Vector3 desiredScale)
	{
		extrusionDirection = Vector3.Normalize(desiredTranslation);
		if( ! sweep())
		{
			destPos = desiredTranslation + transform.position;
			journeyLength = Vector3.Distance(transform.position, destPos);
			isExtruding = true;
			extrusionStartTime = Time.time;
			extrusionStartPos = transform.position;
			totalScaleAmt = desiredScale;
			origScale = transform.localScale;
		}
	}
	
	private void stepExtrusion()
	{
		
		float distCovered = currentSpeed * (Time.time - extrusionStartTime);
		float fracJourney = distCovered / journeyLength;
		if (journeyLength < EPSILON)
		{
			fracJourney = 0;
		}
		Vector3 lastPosition = transform.position;
		transform.position = Vector3.Lerp(extrusionStartPos, destPos, fracJourney);
		// Double position difference to account for dist gained by scaling
		pushDist = 2.0f * (transform.position - lastPosition);
		transform.localScale = origScale + totalScaleAmt * fracJourney;
		
		if (fracJourney >= 1.0f - EPSILON)   
		{
			isExtruding = false;
		}
		
	}
	private void startReset(Vector3 desiredPosition, Vector3 desiredScale)
	{
		StartCoroutine(TweenReset(transform.position, transform.localScale, .3f));
	}
	
	public void stopExtruding()
	{
		isExtruding = false;
	}
	
	
	private bool sweep()
	{
		RaycastHit hit;
		float sweepDistance = 0.27f;
		Vector3 origScale = transform.localScale;
		transform.localScale *= 0.98f;
		bool hitSomething = transform.rigidbody.SweepTest(extrusionDirection, 
														  out hit, 
														  sweepDistance);
		transform.localScale = origScale; 
		if (hit.collider != null && hit.collider.gameObject.Equals(player.gameObject))
		{
			hitSomething = false;
		}
		return hitSomething;		
	}
	
	private void permImpact(FaceType faceType, float shotCharge)
	{
		currentSpeed = shotCharge * BASE_SPEED;
		float extrusionScaler = shotCharge * EXTRUSION_SCALER;
		Vector3 translation = calcTranslation(faceType) * 0.5f * extrusionScaler;
		Vector3 scale = calcScale(faceType) * extrusionScaler;
		startExtrusion(translation, scale);
		//transform.localScale += scale;
		//transform.Translate(translation);
	}
	
	private void reflectiveImpact(RaycastHit hit, FaceType faceType, int shotCharge)
	{
		
	}
	
	private void tempImpact(FaceType faceType, int shotCharge)
	{
		
	}
	
	public Vector3 ExtrusionDirection
	{
		get
		{
			return extrusionDirection;
		}
	}
	
	IEnumerator TweenReset(Vector3 startPos, Vector3 startScale, float totalTime) {
		for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
		{
			float newTime = elapsedTime/totalTime;
			newTime = Mathf.Sin(newTime * Mathf.PI * 0.5f);
			
			transform.position = Vector3.Lerp(startPos, initialPosition, newTime);
			transform.localScale = Vector3.Lerp(startScale, initialScale, newTime); 
			yield return 0;
		}
		transform.position = initialPosition;
		transform.localScale = initialScale;
		isResetting = false;
	}
}
