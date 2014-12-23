using UnityEngine;
using System.Collections;
using GDM.Asset;
using System.Runtime.InteropServices;

namespace GDM
{
	namespace Global
	{
		public class Initializer  : MonoBehaviour
		{
		
			[DllImport("Test")]
			private static extern  float FooPluginFunction();
		
			void Start()
			{
				Debug.Log(FooPluginFunction());
			}
		}
	}
}