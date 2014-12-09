using UnityEngine;
using System.Collections;

namespace GDM
{
    namespace Util
    {
        public class FpsHelper : MonoBehaviour
        {

            private float mLastTime = 0f;
            private int mFrames = 0;

            private string mText;

            void Start()
            {
                mLastTime = Time.realtimeSinceStartup;
                mFrames = 0;
            }

            void OnGUI()
            {
                GUI.Label(new Rect(0.0f, 0.0f, 200f, 200f), mText);
            }

            void Update()
            {
                ++mFrames;

                float curTime = Time.realtimeSinceStartup;

                if (curTime > mLastTime + 1f)
                {
                    float fps = (float)(mFrames / (curTime - mLastTime));
                    float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
                    mText = ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS ";
                    
                    //reset
                    mFrames = 0;
                    mLastTime = curTime;
                }
            }
        }


    }
}
