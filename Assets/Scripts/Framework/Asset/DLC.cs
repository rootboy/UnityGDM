using UnityEngine;
using System.Collections;

namespace GDM
{
	namespace Asset
	{
		/// <summary>
		/// DLC System.(Downloadable Content)
		/// 1、Download mainfest file comparing between client and server
		/// 2、if the version not equal, start downloading, else, jump step 3
		/// 3、Enter Game
		/// </summary>

		public sealed class DLC : MonoBehaviour 
		{
			public DLC()
			{
				GameObject go = new GameObject("_DLC");
				//DLC dlc = go.AddComponent<DLC>();
				//if(dlc == null) Debug.LogError("DLC Error!");
			}
		}
	}
}