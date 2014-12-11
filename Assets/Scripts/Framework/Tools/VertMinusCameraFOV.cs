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

			float hFOVInRads;


			void Start(){
				float aspectRatio = (float) manualWidth / (float) manualHeight;
				float vFOVInRads = manualFOV * Mathf.Deg2Rad;
				hFOVInRads = 2f * Mathf.Atan( Mathf.Tan(vFOVInRads/2f) * aspectRatio);
			}


			void Update(){
				if(manualHeight != Screen.height  || manualWidth != Screen.width){
					float aspectRatio = (float) Screen.width / (float) Screen.height;
					float vFOVInRads = 2f * Mathf.Atan( Mathf.Tan(hFOVInRads/2f) / aspectRatio );
					camera.fieldOfView = vFOVInRads * Mathf.Rad2Deg;
				}
			}
		}
	}
}