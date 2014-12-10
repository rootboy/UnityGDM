using UnityEngine;
using System.IO;
using System.Collections;


namespace GDM
{
	namespace Tools
	{
		/// <summary>
		/// Collect log.
		/// </summary>
		public class Log : MonoBehaviour 
		{
			string output = "";
			string stack = "";
			LogType logType = LogType.Log;

			StreamWriter sw;
			bool init;


			void Awake()
			{
				sw = new StreamWriter(Application.streamingAssetsPath + "/log.txt", true);
				Application.RegisterLogCallback(HandleLog);
			}

			void OnDestroy()
			{
				Application.RegisterLogCallback(null);
			}
			
			void HandleLog(string logString, string stackTrace, LogType type) 
			{
				output = logString;
				stack = stackTrace;
				logType = type;

				if(sw != null){
					sw.WriteLine(System.DateTime.UtcNow.ToString());
					sw.WriteLine(string.Format("output: {0}", output));
					sw.WriteLine(string.Format("stack: {0}", stackTrace));
					sw.WriteLine(string.Format("logType: {0}", type.ToString()));

					sw.WriteLine(string.Format("deviceModel: {0}", SystemInfo.deviceModel));
					sw.WriteLine(string.Format("deviceName: {0}", SystemInfo.deviceName));
					sw.WriteLine(string.Format("graphicsDeviceVendor: {0}", SystemInfo.graphicsDeviceVendor));
					sw.WriteLine(string.Format("graphicsDeviceName: {0}", SystemInfo.graphicsDeviceName));
					sw.WriteLine(string.Format("systemMemorySize: {0}", SystemInfo.systemMemorySize));
					sw.WriteLine(string.Format("graphicsMemorySize: {0}", SystemInfo.graphicsMemorySize));
					sw.WriteLine();
				}
				sw.Close();
			}
		}
	}
}