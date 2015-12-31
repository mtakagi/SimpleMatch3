using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Manager;

public class PopupBase : LayerBase 
{
	[SerializeField] protected Text titleText;
	[SerializeField] protected Text messageText;
	[SerializeField] protected Button leftArrowButton;
	[SerializeField] protected Button rightArrowButton;
	[SerializeField] protected Button closeButton;
	[SerializeField] protected Button confirmButton;
	[SerializeField] protected Button cancelButton;
	
#region Monobehavior LifeCycle
	
	protected override IEnumerator LayerStart()
	{
		form = LayerForm.Popup;

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

#region Button Callback Methods
	
	public virtual void OnPressedCloseButton()
	{
		StartCoroutine(DestroyLayer());
	}
	
#endregion

#region Normal Method

	public void ExchangePopup<Type>() where Type : PopupBase 
	{
		PopupBase anotherLayer = (PopupBase)SceneManager.Instance.CreateStateByType<Type>();
		if(anotherLayer != null)
		{
			if(this == SceneManager.Instance.RootLayer)
			{
				SceneManager.Instance.RootLayer = anotherLayer;
			}

			anotherLayer.transform.parent = transform.parent;

			Canvas anotherCanvas = anotherLayer.GetComponentInChildren<Canvas>();
			anotherCanvas.sortingOrder = 9999 + SceneManager.Instance.PopupList.Count;

			SceneManager.Instance.PopupList.Add(anotherLayer);

			StartCoroutine(DestroyLayer());
		}
	}

	public override IEnumerator DestroyLayer()
	{
		status = LayerStatus.End;

		yield return StartCoroutine(LayerEnd());
		
		SceneManager.Instance.RemovePopup(this);

		yield return null;
	}
	
#endregion
}
