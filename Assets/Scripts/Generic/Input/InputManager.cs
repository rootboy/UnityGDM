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
            OnPress,
            OnHover,
            OnKey,
            OnScroll,
            OnDeviceRotation,
            OnDrag,
            OnPinch,
            OnSwipe,
            OnGyroscope
        }

        /// <summary>
        /// InputManager is a helper class to process various of event sources.
        /// </summary>
		public sealed class InputManager : Singleton<InputManager>
		{
            private DeviceOrientation mOrientation;
            private Action<DeviceOrientation> onDeviceOrientationChange;

            public bool useKeyboard = true;
            public bool useMouse = true;
            public bool useTouch = true;
            public bool useDeviceOrientation = true;
            public bool useGyroscope = true;

            void Awake()
            {
                mOrientation = Input.deviceOrientation;
            }

            void Update()
            {
                if (useMouse) ProcessMouse();
                if (useKeyboard) ProcessKeyboard();
                if (useTouch) ProcessTouch();
                if (useDeviceOrientation) ProcessDeviceOrientation();
                if (useGyroscope) ProcessGyroscope();
            }

            public void RegisterInputEvent(InputEvent inputEvent)
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

            private void Notify(GameObject go, string functionName, object obj = null)
            {
 
            }

            private void ProcessDeviceOrientation()
            {
                if (Input.deviceOrientation != mOrientation)
                {
                    mOrientation = Input.deviceOrientation;
                    if (onDeviceOrientationChange != null) 
                        onDeviceOrientationChange(mOrientation);
                }
            }

            private void ProcessMouse()
            {

            }

            private void ProcessKeyboard()
            {

            }

            private void ProcessTouch()
            {


            }

            private void ProcessGyroscope()
            {


            }
		}
	}
}