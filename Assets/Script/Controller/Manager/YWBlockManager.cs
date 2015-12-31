using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

public class YWBlockManager : MonoBehaviour {

	protected int COLUMN = 9;
	protected int ROW = 9;

	protected float BLOCK_SIZE = 60.0f;
	protected float BLOCK_SWAP_DURATION = 0.5f;
	protected float MINIMUM_BLOCK_COUNT = 3;

	[Header("Block Position")]
	[SerializeField] protected Transform originTransform;

	[Header("Block Prefab")]
	[SerializeField] protected YWBlock[] blockPrefabs;

	protected YWBlock[,] blocks;

#region Callback Methods

	public void OnDestroy()
	{
	}

#endregion

#region Normal Methods

	public YWBlock[,] Blocks
	{
		get
		{
			return blocks;
		}
		set
		{
			blocks = value;
		}
	}

	public YWBlock GetRandomBlockPrefab ()
	{
		return blockPrefabs[Random.Range(0, blockPrefabs.Length)];
	}

	public Vector3 GetExactBlockPosition(int column, int row)
	{
		return new Vector3 (originTransform.localPosition.x + column * BLOCK_SIZE,
			originTransform.localPosition.y + row * BLOCK_SIZE,
			0.0f);
	}

	public void Initialize ()
	{
		GenerateInitialBlocks ();
	}

	public void Restart()
	{
		Clear ();
		Initialize ();
	}

	public void Clear ()
	{
		if (blocks == null) 
		{
			return;
		}

		for (int column = 0; column < COLUMN; column++) 
		{
			for (int row = 0; row < ROW; row++) 
			{
				Destroy (blocks [column, row].gameObject);
				blocks [column, row] = null;
			}
		}
	}

	public void Remove(int column, int row)
	{
		Destroy(blocks[column, row].gameObject);
		blocks[column, row] = null;
	}

	public IEnumerable<YWBlock> GetMatches(YWBlock block)
	{
		//* & & ! * * * * *
		//* * & ! & * * * *
		//* * * ! & & * * *

		List<YWBlock> horizontalMathces = new List<YWBlock> ();
		//From Origin To Left
		for (int column = block.Column; column >= 0; column--) 
		{
			YWBlock newBlock = blocks [column, block.Row];
			if (block.IsItSame (newBlock)) 
			{
				horizontalMathces.Add (newBlock);
			}
			else
			{
				break;
			}
		}
		//From Origin To Right
		for (int column = block.Column + 1; column < COLUMN; column++) 
		{
			YWBlock newBlock = blocks [column, block.Row];
			if (block.IsItSame (newBlock)) 
			{
				horizontalMathces.Add (newBlock);
			}
			else
			{
				break;
			}
		}
		if(horizontalMathces.Count < MINIMUM_BLOCK_COUNT)
		{
			horizontalMathces.Clear();
		}

		List<YWBlock> verticalMathces = new List<YWBlock> ();
		//From Origin To Bottom
		for (int row = block.Row; row >= 0; row--) 
		{
			YWBlock newBlock = blocks [block.Column, row];
			if (block.IsItSame (newBlock)) 
			{
				verticalMathces.Add (newBlock);
			}
			else
			{
				break;
			}
		}
		//From Origin To Top
		for (int row = block.Row + 1; row < ROW; row++) 
		{
			YWBlock newBlock = blocks [block.Column, row];
			if (block.IsItSame (newBlock)) 
			{
				verticalMathces.Add (newBlock);
			}
			else
			{
				break;
			}
		}
		if(verticalMathces.Count < MINIMUM_BLOCK_COUNT)
		{
			verticalMathces.Clear();
		}

		return horizontalMathces.Union(verticalMathces).Distinct();
	}

	public IEnumerable<YWBlock> GetMatches(IEnumerable<YWBlock> blocks)
	{
		List<YWBlock> mathces = new List<YWBlock> ();
		foreach(var block in blocks)
		{
			mathces.AddRange(GetMatches(block));
		}

		if(mathces.Count < MINIMUM_BLOCK_COUNT)
		{
			mathces.Clear();
		}

		return mathces.Distinct();
	}

	public IEnumerable<YWBlock> GetDescendingBlocksAfterMatch()
	{
		List<YWBlock> descendingBlocks = new List<YWBlock> ();
		for (int column = 0; column < COLUMN; column++) 
		{
			for (int row = 0; row < ROW; row++) 
			{
				if (blocks [column, row] == null) 
				{
					for (int newRow = row + 1; newRow < ROW; newRow++) 
					{
						if (blocks [column, newRow] != null) 
						{
							blocks [column, row] = blocks [column, newRow];
							blocks [column, newRow] = null;
							blocks [column, row].Row = row;

							descendingBlocks.Add (blocks [column, row]);

							break;
						}
					}
				}
			}
		}

		return descendingBlocks.Distinct ();
	}

	public IEnumerable<YWBlock> GetNewBlocksAfterMatch()
	{
		List<YWBlock> newBlocks = new List<YWBlock> ();
		for (int column = 0; column < COLUMN; column++) 
		{
			int blockCountPerRow = 0;
			for (int row = 0; row < ROW; row++) 
			{
				if (blocks [column, row] == null) 
				{
					blockCountPerRow++;

					blocks [column, row] = GenerateBlockWithColumnAndRow (column, row, GetRandomBlockPrefab ());
					blocks [column, row].transform.localPosition = new Vector3 
						(originTransform.localPosition.x + column * BLOCK_SIZE,
							originTransform.localPosition.y + (ROW + blockCountPerRow) * BLOCK_SIZE,
							0.0f);


					newBlocks.Add (blocks [column, row]);
				}
			}
		}

		return newBlocks.Distinct ();
	} 


	protected void GenerateInitialBlocks ()
	{
		blocks = new YWBlock[COLUMN, ROW];
		for (int column = 0; column < COLUMN; column++) 
		{
			for (int row = 0; row < ROW; row++) 
			{
				blocks [column, row] = GenerateBlockWithColumnAndRow (column, row, GetRandomBlockPrefab ());
			}
		}

		for (int column = 0; column < COLUMN; column++) 
		{
			for (int row = 0; row < ROW; row++) 
			{
				while (GetMatches (blocks [column, row]).Count () > 0) 
				{
					Destroy (blocks [column, row].gameObject);
					blocks [column, row] = GenerateBlockWithColumnAndRow (column, row, GetRandomBlockPrefab ());
				}
			}
		}
	}

	protected YWBlock GenerateBlockWithColumnAndRow(int column, int row, YWBlock blockPrefab)
	{
		YWBlock block = Instantiate (blockPrefab) as YWBlock;
		block.transform.SetParent (this.transform, true);
		block.transform.localPosition = new Vector3 
			(originTransform.localPosition.x + column * BLOCK_SIZE,
				originTransform.localPosition.y + row * BLOCK_SIZE,
				0.0f);
		block.transform.localScale = Vector3.one;

		block.Column = column;
		block.Row = row;
		block.Color = block.GetComponent<Image>().color;

		return block;
	}
			
#endregion
}
