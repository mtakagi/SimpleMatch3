using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Common;

namespace Manager
{
	public class SceneManager : SingletonMonoBehaviour<SceneManager>
	{
		public enum LayerTransition
		{
			SingleNormal = 0,
			MultiNormal,
			ShowSingleResult,
			ShowMultiResult,
			ShowMessage,
			TryReplay
		}

		private List<PopupBase> popupList = new List<PopupBase>();
		
		private GameObject loadingGameobject;

		private LayerBase rootLayer;
		private LayerTransition trasition;

		public List<PopupBase> PopupList
		{
			get
			{
				return popupList;
			}
			set
			{
				popupList = value;
			}
		}
		public LayerBase RootLayer
		{
			get
			{
				return rootLayer;
			}
			set
			{
				rootLayer = value;
			}
		}
		public LayerTransition Trasition
		{
			get
			{
				return trasition;
			}
			set
			{
				trasition = value;
			}
		}

#region Normal Methods

		public void ShowLoading()
		{
			if(loadingGameobject == null)
			{
				GameObject loadingPrefab = Resources.Load(CommonData.LOADING_PREFAB_PATH) as GameObject;
				loadingGameobject = Instantiate(loadingPrefab) as GameObject;
				loadingGameobject.transform.localPosition = Vector3.one;
				loadingGameobject.transform.localScale = Vector3.one;
			}
		}
		
		public void HideLoading()
		{
			if(loadingGameobject)
			{
				Destroy(loadingGameobject);
				loadingGameobject = null;
			}
		}

		public LayerBase GetStatePrefabByType<Type>() where Type : LayerBase 
		{
			LayerBase[] result = Resources.FindObjectsOfTypeAll(typeof(Type)) as LayerBase[];
			if(result != null && result.Length > 0)
			{
				return result[0];
			}

			Debug.LogError("not found state "+ typeof(Type));

			return null;
		}
		
		public LayerBase CreateStateByType<Type>() where Type : LayerBase 
		{
			LayerBase layer = GetStatePrefabByType<Type>();
			if(layer != null) 
			{
				LayerBase result = Instantiate(layer) as LayerBase;
				result.name = layer.name;

				return result;
			}

			return null;
		}
		
		public void RemoveFrontestPopup()
		{
			Destroy(popupList[popupList.Count - 1].gameObject);
			popupList.RemoveAt(popupList.Count - 1);
		}
		
		public void RemovePopup(PopupBase popup)
		{
			popupList.Remove(popup);
			Destroy(popup.gameObject);
		}

		public void CloseAllPopup()
		{
			while(popupList.Count > 0)
			{
				RemoveFrontestPopup();
			}
		}

#endregion
	}
}
