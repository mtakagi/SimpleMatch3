using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

public class YWPuzzleManager : MonoBehaviour 
{
	public enum GameState
	{
		UserInput = 0,
		SelectedFirstBlock,
		Animating,
		Win,
		Lose
	}

	protected int SCORE_PER_BLOCK = 100;

	public System.Action<int> OnChangedScore;

	protected float BLOCK_SWAP_DURATION = 0.3f;
	protected float BLOCK_SHOW_REMOVE_DURATION = 1.0f;
	protected float BLOCK_DROP_DURATION = 0.2f;
	protected float MINIMUM_BLOCK_COUNT = 3;

	[SerializeField] protected Camera gameCamera;
	[SerializeField] protected YWBlockManager blockManager;

	protected YWBlock selectedFirstBlock;
	protected GameState state;

	protected int score;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (state) 
		{
		case GameState.UserInput:
			if (Input.GetMouseButtonDown (0)) 
			{
				var blockWorldPos = gameCamera.ScreenToWorldPoint (Input.mousePosition);
				var hit = Physics2D.Raycast(blockWorldPos, Vector2.zero);
				if (hit.collider != null)
				{
					var block = hit.collider.gameObject.GetComponent<YWBlock> ();
					if (block != null) 
					{
						selectedFirstBlock = block;
						state = GameState.SelectedFirstBlock;
					}
				}
			}

			break;
		case GameState.SelectedFirstBlock:

			if (Input.GetMouseButtonUp (0)) 
			{
				var blockWorldPos = gameCamera.ScreenToWorldPoint (Input.mousePosition);
				var hit = Physics2D.Raycast(blockWorldPos, Vector2.zero);
				if (hit.collider != null) {
					var block = hit.collider.gameObject.GetComponent<YWBlock> ();
					if (block != null && selectedFirstBlock != null) 
					{
						StartCoroutine (CheckMatchesAndDestroy (selectedFirstBlock, block));

						selectedFirstBlock = null;
					}
				} 
				else 
				{
					state = GameState.UserInput;
				}
			}
			break;
		default:
			break;
		}
	}


	public IEnumerator Swap(YWBlock firstBlock, YWBlock secondBlock)
	{
		Vector3 tempPosition = firstBlock.transform.position;
		firstBlock.transform.DOMove (secondBlock.transform.position, BLOCK_SWAP_DURATION);
		secondBlock.transform.DOMove (tempPosition, BLOCK_SWAP_DURATION);

		blockManager.Blocks[firstBlock.Column, firstBlock.Row] = secondBlock;
		blockManager.Blocks[secondBlock.Column, secondBlock.Row] = firstBlock;

		firstBlock.Swap (secondBlock);

		yield return new WaitForSeconds (BLOCK_SWAP_DURATION);
	}

	public void Drop(IEnumerable<YWBlock> blocks)
	{
		foreach (var block in blocks) 
		{
			block.transform.DOLocalMove (blockManager.GetExactBlockPosition(block.Column, block.Row), BLOCK_SWAP_DURATION);
		}
	}

	public IEnumerator CheckMatchesAndDestroy(YWBlock firstBlock, YWBlock secondBlock)
	{
		if (!IsItMovable (firstBlock, secondBlock)) 
		{
			state = GameState.UserInput;

			yield break;
		}

		state = GameState.Animating;

		yield return StartCoroutine(Swap(firstBlock, secondBlock));

		var firstMatches = blockManager.GetMatches(firstBlock);
		var secondMatches = blockManager.GetMatches(secondBlock);
		var totalMatches = firstMatches.Union(secondMatches).Distinct();
		if(totalMatches.Count() < MINIMUM_BLOCK_COUNT)
		{
			yield return StartCoroutine(Swap(firstBlock, secondBlock));
		}

		while(totalMatches.Count() >= MINIMUM_BLOCK_COUNT)
		{
			foreach(var block in totalMatches)
			{
				block.mark.SetActive (true);
			}
			yield return new WaitForSeconds (BLOCK_SHOW_REMOVE_DURATION);

			foreach(var block in totalMatches)
			{
				blockManager.Remove(block.Column, block.Row);
			}

			score += totalMatches.Count () * SCORE_PER_BLOCK;
			if (OnChangedScore != null) 
			{
				OnChangedScore (score);
			}

			var droppedBlocks = blockManager.GetDescendingBlocksAfterMatch ();
			var newBlocks = blockManager.GetNewBlocksAfterMatch ();

			//drop animation
			Drop(droppedBlocks);
			Drop(newBlocks);
			yield return new WaitForSeconds (BLOCK_DROP_DURATION);

			totalMatches = blockManager.GetMatches(droppedBlocks).
				Union(blockManager.GetMatches(newBlocks)).Distinct();
		}

		state = GameState.UserInput;
	}

	public bool IsItMovable(YWBlock firstBlock, YWBlock secondBlock)
	{
		return (Mathf.Abs (firstBlock.Row - secondBlock.Row) <= 1 && Mathf.Abs (firstBlock.Column - secondBlock.Column) <= 1) &&
			!(Mathf.Abs (firstBlock.Row - secondBlock.Row) == 1 && Mathf.Abs (firstBlock.Column - secondBlock.Column) == 1) &&
			!((firstBlock.Row == secondBlock.Row) && (firstBlock.Column == secondBlock.Column));
	}

	public GameState GetState()
	{
		return state;
	}

	public Camera GetMainCamera()
	{
		var mainRoot = GameObject.Find ("MainRoot");
		var cameraObject = mainRoot.transform.Find ("Camera");
		if (cameraObject != null) 
		{
			return cameraObject.GetComponent<Camera> ();
		}

		return null;
	}
}
