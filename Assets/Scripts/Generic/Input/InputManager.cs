using UnityEngine;
using System;
using System.Collections;
using GDM.Global;

namespace GDM
{
	namespace INPUT
	{
        public enum InputEvent
        {
            OnClick,
            OnDoubleClick,
            OnDeviceRotation,
            OnPinch,
            OnDrag,
            OnSwipe,
        }

		public class InputManager : Singleton<InputManager>
		{
            private DeviceOrientation mOrientation;
            private Action<DeviceOrientation> onDeviceOrientationChange;
            

            void Awake()
            {
                mOrientation = Input.deviceOrientation;
            }

            void Update()
            {
                //Device Orientation
                if (Input.deviceOrientation != mOrientation) 
                {
                    mOrientation = Input.deviceOrientation;
                    if (onDeviceOrientationChange != null) onDeviceOrientationChange(mOrientation);
                }
            }

            public void RegisterInputEvent(InputEvent inputEvent)
            {
                switch(inputEvent)
                {
                    case InputEvent.OnClick:
                        break;
                    case InputEvent.OnDoubleClick:
                        break;
                    case InputEvent.OnDeviceRotation:
                        break;
                    case InputEvent.OnDrag:
                        break;
                    case InputEvent.OnPinch:
                        break;
                    case InputEvent.OnSwipe:
                        break;
                    default:
                        break;
                }
            }


            public void UnRegisterInputEvent(InputEvent inputEvent)
            {
                switch (inputEvent)
                {
                    case InputEvent.OnClick:
                        break;
                    case InputEvent.OnDoubleClick:
                        break;
                    case InputEvent.OnDeviceRotation:
                        break;
                    case InputEvent.OnDrag:
                        break;
                    case InputEvent.OnPinch:
                        break;
                    case InputEvent.OnSwipe:
                        break;
                    default:
                        break;
                }
            }
		}
	}
}