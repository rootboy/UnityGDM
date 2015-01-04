using UnityEngine;
using System.Collections;

namespace GDM
{
    namespace Util
    {
        public static class TransformUtil
        {
            /// <summary>
            /// Add a child to parent, keep the value of transform equally.
            /// </summary>
            /// <param name="child"></param>
            /// <param name="parent"></param>
            public static void AddChild(Transform child, Transform parent)
            {
                Vector3 pos = child.localPosition;
                Vector3 rot = child.localEulerAngles;
                Vector3 sca = child.localScale;

                child.parent = parent;
                child.localPosition = pos;
                child.localEulerAngles = rot;
                child.localScale = sca;
            }


            /// <summary>
            /// Add a child to parent, keep the value of transform equally.
            /// </summary>
            /// <param name="child"></param>
            /// <param name="parent"></param>
            public static void AddChild(GameObject child, GameObject parent)
            {
                AddChild(child.transform, parent.transform);
            }
        }
    }
}
