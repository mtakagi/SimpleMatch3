using UnityEngine;
using System.Collections;

public class MainMenuLayer : LayerBase {
	
#region Monobehavior LifeCycle
	
	protected override IEnumerator LayerStart()
	{
		form = LayerForm.Page;
		
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

	public void OnPressedNextButton()
	{
		ExchangeLayer<GameLayer> ();
	}

#endregion
	
#region Normal Methods
	
	
#endregion
}
