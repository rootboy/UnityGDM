using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GDM.Util;


namespace GDM
{
	namespace Asset
	{
		/// <summary>
		/// DLC System.
		/// </summary>

		public sealed class DLC : MonoBehaviour 
		{
			private const string serverIP = "192.168.1.1";
			private const string MainfestServerPath = "mainfext.txt";
			private const string MainfestLocalPath = "mainfest.txt";

			private Dictionary<string, string> dicLocal = new Dictionary<string, string>();
			private Dictionary<string, string> dicServel = new Dictionary<string, string>();


			IEnumerator Start()
			{
				yield return StartCoroutine(DownloadMainfest());
				CompareMainfest();
			}


			bool CompareMainfest()
			{
				return false;
			}


			private string GetMD5FromFile(string filename)
			{
				FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] hash = md5.ComputeHash(fs);

				StringBuilder sb = new StringBuilder();
				for(int i=0; i<hash.Length;i++)
				{
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}


			#region Coroutines

			IEnumerator DownloadMainfest()
			{
				WWW www = new WWW(serverIP);
				yield return www;
				if(string.IsNullOrEmpty(www.error)){

				}
				www.Dispose();
			}

			IEnumerator DownloadAsset()
			{
				yield break;
			}

			#endregion
		}
	}
}