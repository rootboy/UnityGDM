using UnityEngine;
using System.Collections;

namespace GDM
{
	namespace Tools
	{
		[ExecuteInEditMode]
		[RequireComponent(typeof(Camera))]
		public class VertMinusCameraFOV  : MonoBehaviour
		{
			public float manualFOV = 60;
			public int manualWidth = 640;
			public int manualHeight = 1136;

			public bool runOnlyOnce = false;


			void Start()
			{

			}

			void Update()
			{

			}





		}
	}
}



/*void Start () {
	
	float aspectRatio = (float) manualWidth / (float) manualHeight;
	float vFOVInRads = 60 * Mathf.Deg2Rad;
	hFOVInRads = 2f * Mathf.Atan( Mathf.Tan(vFOVInRads/2f) * aspectRatio);
}

void Update () 
{
	
	//		if(root != null){
	//
	//			Vector2 screen =  NGUITools.screenSize;
	//			float aspect = (float)screen.x / screen.y;
	//			float initialAspect = (float)manualWidth / manualHeight;
	//
	//			float scale = System.Convert.ToSingle(root.manualHeight / manualWidth);
	//
	//			if(initialAspect != aspect)
	//			{
	//				camera.fieldOfView = 60 * scale;
	//			}
	//		}
	
	if(manualWidth != Screen.width || manualHeight != Screen.height)
	{
		float aspectRatio = (float) Screen.width / (float) Screen.height;
		float vFOVInRads = 2f * Mathf.Atan( Mathf.Tan(hFOVInRads/2f) / aspectRatio );
		camera.fieldOfView = vFOVInRads * Mathf.Rad2Deg;
	}
}*/