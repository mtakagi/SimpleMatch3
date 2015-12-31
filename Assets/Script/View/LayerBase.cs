using UnityEngine;
using System.Collections;

using Manager;

public abstract class LayerBase : MonoBehaviour {

	public enum LayerStatus
	{
		Start,
		Update,
		WaitingInput,
		Tutorial,
		End
	}
	public enum LayerForm
	{
		Page,
		Popup
	}
	
	protected bool isParent;
	protected LayerStatus status;
	protected LayerForm form;
	protected LayerBase childLayer;
	protected Canvas canvas;

	public bool IsParent
	{
		get
		{
			return isParent;
		}
		set
		{
			isParent = value;
		}
	}
	public LayerForm Form
	{
		get
		{
			return form;
		}

	}
	public LayerBase ChildLayer
	{
		get
		{
			return childLayer;
		}
		
	}

	// Use this for initialization
	IEnumerator Start () {
		status = LayerStatus.Start;
		canvas = GetComponentInChildren<Canvas>();

		yield return StartCoroutine(LayerStart());

		status = LayerStatus.Update;
	}
	
	// Update is called once per frame
	void Update () {
		if(status == LayerStatus.Update)
		{
			LayerUpdate();
			if(isParent && childLayer == null)
			{
				isParent = false;
				StartCoroutine(LayerRestart());
			}
		}
	}

#region CallBack Methods

	protected abstract IEnumerator LayerStart();

	protected abstract IEnumerator LayerRestart();

	protected abstract IEnumerator LayerEnd();

	protected virtual void LayerFocused() {}

	protected virtual void LayerUnfocused() {}

	protected virtual void LayerUpdate() {}

#endregion

#region Normal Methods

	public void CreateChildLayer<Type>() where Type : LayerBase 
	{
		isParent = true;
		childLayer = SceneManager.Instance.CreateStateByType<Type>();
		if(childLayer != null)
		{
			if(SceneManager.Instance.RootLayer == null)
			{
				SceneManager.Instance.RootLayer = childLayer;
			}
			else
			{
				childLayer.transform.parent = transform;
			}

			Canvas childCanvs = childLayer.GetComponentInChildren<Canvas>();
			childCanvs.sortingOrder = canvas.sortingOrder + 1;

			LayerUnfocused();
		}
	}

	public void CreatePopupLayer<Type>() where Type : LayerBase 
	{
		isParent = true;
		childLayer = SceneManager.Instance.CreateStateByType<Type>();
		if(childLayer != null)
		{
			SceneManager.Instance.PopupList.Add((PopupBase)childLayer);
			
			Canvas childCanvs = childLayer.GetComponentInChildren<Canvas>();
			childCanvs.sortingOrder = 9999 + SceneManager.Instance.PopupList.Count;
			
			LayerUnfocused();
		}
	}

	public void ExchangeLayer<Type>() where Type : LayerBase 
	{
		SceneManager.Instance.CloseAllPopup();
		
		LayerBase anotherLayer = SceneManager.Instance.CreateStateByType<Type>();
		if(anotherLayer != null)
		{
			if(this == SceneManager.Instance.RootLayer)
			{
				SceneManager.Instance.RootLayer = anotherLayer;
			}

			anotherLayer.transform.parent = transform.parent;

			Canvas anotherCanvas = anotherLayer.GetComponentInChildren<Canvas>();
			anotherCanvas.sortingOrder = canvas.sortingOrder;

			Destroy(gameObject);
		}
	}

	public virtual IEnumerator DestroyLayer()
	{
		status = LayerStatus.End;

		ClearLayerInfoFromParent();

		yield return StartCoroutine(LayerEnd());
		
		Destroy(gameObject);

		yield return null;
	}
	
	public void ClearLayerInfoFromParent()
	{
		if(this == SceneManager.Instance.RootLayer)
		{
			SceneManager.Instance.RootLayer = null;
		}
		if(transform.parent != null)
		{
			LayerBase parentLayer = transform.parent.GetComponent<LayerBase>();
			if(parentLayer != null)
			{
				parentLayer.childLayer = null;
			}
		}
	}

#endregion
}
