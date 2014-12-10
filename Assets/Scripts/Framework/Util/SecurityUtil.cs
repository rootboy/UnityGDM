using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace GDM
{
	namespace Util
	{
		public sealed class SecurityUtil 
		{

			/// <summary>
			/// Get the MD5 from the fileName.
			/// </summary>
			public static string GetMD5FromFile(string fileName)
			{
				FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] hash = md5.ComputeHash(fs);
				
				StringBuilder sb = new StringBuilder();
				for(int i=0; i<hash.Length;i++)
				{
					sb.Append(hash[i].ToString("x2"));
				}
				return sb.ToString();
			}



		}
	}
}