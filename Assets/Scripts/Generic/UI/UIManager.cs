using UnityEngine;
using System.Collections.Generic;
using GDM.Global;

namespace GDM
{ 
    namespace UI
    {
        public class UIManager : Singleton<UIManager>
        {
			protected Dictionary<string, GameObject> dicUis = new Dictionary<string, GameObject>();
			protected Stack<string> stackUis = new Stack<string>();


			public bool Show(string name, object param)
			{
				if(dicUis.ContainsKey(name)){
					GameObject go = dicUis[name];
					if(go != null){
						go.SetActive(true);
						UIBase ui = go.GetComponent<UIBase>();
						if(ui != null){
							ui.OnVisible(param);
						}
					}
					return true;
				}
				return false;
			}


			public bool Hide(string name, object param)
			{
				if(dicUis.ContainsKey(name)){
					GameObject go = dicUis[name];
					if(go != null){
						go.SetActive(false);
						UIBase ui = go.GetComponent<UIBase>();
						if(ui != null){
							ui.OnInvisible(param);
						}
					}
				}
				return false;
			}


			public void Next()
			{

			}


			public void Previous()
			{

			}

        }
    } 
} 
