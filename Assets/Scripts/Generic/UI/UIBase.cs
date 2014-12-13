using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GDM
{
	namespace UI
	{
		public abstract class UIBase : MonoBehaviour 
		{
			protected Dictionary<string, BoxCollider> dicBtns = new Dictionary<string, BoxCollider>();
			protected Dictionary<string, UILabel> dicLabels = new Dictionary<string, UILabel>();
			protected Dictionary<string, UISprite> dicSprite = new Dictionary<string, UISprite>();

			protected string buttonFilter = "Button_";
			protected string labelFilter = "Label_";
			protected string spriteFilter = "Sprite_";

			protected virtual void Awake()
			{
				GetAllReference();
			}

			public virtual void OnVisible(object param)
			{
				
			}
			
			public virtual void OnInvisible(object param) 
			{
				
			}

			protected void GetAllReference()
			{
				GetReference<BoxCollider>(buttonFilter, dicBtns);
				GetReference<UILabel>(labelFilter, dicLabels);
				GetReference<UISprite>(spriteFilter, dicSprite);
			}

			protected void GetReference<T>(string filter, IDictionary dic) where T : Component
			{
				T[] array = GetComponentsInChildren<T>(true);
				foreach(T item in array){
					if(item.name.Contains(filter)){
						if(dic.Contains(item.name)){
							Debug.LogError("");
							continue;
						}
						dic.Add(item.name, item);
					}
				}
			}

		}
	}
}