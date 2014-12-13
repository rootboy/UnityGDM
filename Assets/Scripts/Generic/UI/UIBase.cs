using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GDM
{
	namespace UI
	{
		public abstract class UIBase : MonoBehaviour 
		{
			protected Dictionary<string, GameObject> dicBtns = new Dictionary<string, GameObject>();
			protected Dictionary<string, UILabel> dicLabels = new Dictionary<string, UILabel>();
			protected Dictionary<string, UISprite> dicSprite = new Dictionary<string, UISprite>();

			protected string buttonFilter = "Button_";
			protected string labelFilter = "Label_";
			protected string spriteFilter = "Sprite_";

			protected virtual void Awake(){
				FindReference();
			}


			protected virtual void Start(){

			}


			void FindReference(){

			}



			protected virtual void OnClick(GameObject go){}
			protected virtual void OnVisible(object param){}
			protected virtual void OnInvisible(object param) {}


		}
	}
}