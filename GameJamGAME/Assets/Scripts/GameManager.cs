using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {
	
	
//---------------------------------------------------------------------------FIELDS:
	
	private const float MINIMUM_ORB_TIME = 3000.0f;
	
	private float playerEnteredOrb;
	private bool playerInOrb;
	
	private int currentScene;
	private Orb orb;
	private Player player;
	private List<Block> blocks = new List<Block>();
	
//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	void Start()
	{
		DontDestroyOnLoad(transform.gameObject);
		currentScene = 0;
	}
	
	void Update()
	{
		if (currentScene > 0)
		{
			checkToSwitch();
		}
	}
	
//--------------------------------------------------------------------------METHODS:
	
	private void checkToSwitch()
	{
		if (! playerInOrb)
		{
			if(Vector3.Distance(player.transform.position, orb.transform.position) < 
			   orb.radius)
			{
				//TODO play orb sound
				playerInOrb = true;
				playerEnteredOrb = Time.time;
			}
		}
		else
		{
			if(MINIMUM_ORB_TIME < Time.time - playerEnteredOrb)
			{
				//TODO stop playing orb sound
				loadLevel(currentScene + 1);
			}
		}
	}
	
	private void initLevel()
	{
		orb = Orb.Instance;
		player = Player.Instance;
	}
	
	public void loadLevel(int level)
	{
		Application.LoadLevel(level);
		initLevel();
		
	}
	
	public void addBlock(Block block)
	{
		blocks.Add(block);
	}
	
	public void resetBlocks()
	{
		foreach(Block block in blocks)
		{
			block.reset();
		}
	}
	
}
