using UnityEngine;
using System.Collections;

namespace GDM
{
	namespace Util
	{
		public static class StringUtil 
		{
            /// <summary>
            /// 将秒转成"分：秒"格式的字符串,如参数为88，返回值为01:28.
            /// </summary>
            /// <targetParam name="secs"></targetParam>
            /// <returns></returns>
			public static string SecsToStringMS(uint secs)
			{
				return null;
			}

            /// <summary>
            /// 将秒转成"时：分：秒"格式的字符串,如参数为88，返回值为00:01:28.
            /// </summary>
            /// <targetParam name="secs"></targetParam>
            /// <returns></returns>
            public static string SecsToStringHMS(uint secs)
            {
                return null;
            }

            /// <summary>
            /// 将"(1.0, 2.0, 3.0)"格式的字符串转换为Vector3格式.
            /// </summary>
            /// <targetParam name="str"></targetParam>
            /// <returns></returns>
            public static Vector3 StringToVector3(string str)
            {
                return Vector3.one;
            }

            /// <summary>
            /// 将"(1.0, 2.0)"格式的字符串转换为Vector2格式.
            /// </summary>
            /// <targetParam name="str"></targetParam>
            /// <returns></returns>
            public static Vector2 StringToVector2(string str)
            {
                return Vector2.one;
            }

			//Add code here...
		}
	}
}