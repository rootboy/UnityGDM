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


			public bool Show(string name)
			{
				if(dicUis.ContainsKey(name))
				{
					dicUis[name].SetActive(true);
					return true;
				}
				return false;
			}

			public bool Hide(string name)
			{
				if(dicUis.ContainsKey(name))
				{
					dicUis[name].SetActive(false);
					return true;
				}
				return false;
			}


        }
    } 
} 
