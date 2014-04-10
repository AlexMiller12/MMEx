using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager> {

	
//---------------------------------------------------------------------------FIELDS:
	private List<Block> blocks = new List<Block>();

//-------------------------------------------------------------MONOBEHAVIOR METHDOS:
	
	
//--------------------------------------------------------------------------METHODS:
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
