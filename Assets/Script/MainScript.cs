using UnityEngine;
using System.Collections;

using Manager;

public class MainScript : LayerBase 
{
	[Header("Layer for Reference")]
	[SerializeField] private LayerBase[] layers;

#region Monobehavior LifeCycle
	
	protected override IEnumerator LayerStart()
	{
#if UNITY_EDITOR
		QualitySettings.vSyncCount = 0;
#endif
		Application.targetFrameRate = 30;

		CreateChildLayer<MainMenuLayer>();
		
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

#region Normal Methods

#endregion
}
