using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLayer : LayerBase {

	[SerializeField] private Text scoreText;

	[SerializeField] private YWPuzzleManager puzzleManager;
	[SerializeField] private YWBlockManager blockManager;
	
#region Monobehavior LifeCycle
	
	protected override IEnumerator LayerStart()
	{
		form = LayerForm.Page;

		puzzleManager.OnChangedScore = OnChangedScore;
		blockManager.Initialize ();
		
		yield return null;
	}

	protected override IEnumerator LayerRestart()
	{
		yield return null;
	}
	
	protected override IEnumerator LayerEnd()
	{
		yield return null;
	}
	
	protected override void LayerUpdate () 
	{
	}
	
#endregion

#region Button Methods

	public void OnPressedBackButton()
	{
		ExchangeLayer<MainMenuLayer> ();
	}

#endregion

#region Button Methods

	public void OnChangedScore(int score)
	{
		scoreText.text = score.ToString ();
	}

#endregion
	
#region Normal Methods
	
	
#endregion
}
