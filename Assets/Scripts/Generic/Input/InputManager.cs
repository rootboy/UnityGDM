// Unity Game Development Model
// Author: Rootboy
// Date: 2014-12-30 22:12:39


using UnityEngine;
using GDM.Global;

namespace GDM
{
	namespace INPUT
	{
        public enum InputEvent
        {
            OnHover,
            OnPress,
            OnSelect,
            OnClick,
            OnDoubleClick,
            OnDragStart,
            OnDrag,
            OnDragOver,
            OnDragOut,
            OnDragEnd,
            OnTooltip,
            OnScroll,
            OnKey,
            OnSwipe,
            OnTwist,
            OnPinch,
            OnGyroscope,
            OnDeviceRotation,
        }

        /// <summary>
        /// InputManager is a helper class to process various of input event sources.
        /// * OnHover (isOver) is sent when the mouse hovers over a collider or moves away.
        /// * OnPress (isDown) is sent when a mouse button gets pressed on the collider.
        /// * OnSelect (selected) is sent when a mouse button is first pressed on an object. Repeated presses won't result in an OnSelect(true).
        /// * OnClick () is sent when a mouse is pressed and released on the same object.
        /// * OnDoubleClick () is sent when the click happens twice within a fourth of a second.
        /// * OnDragStart () is sent to a game object under the touch just before the OnDrag() notifications begin.
        /// * OnDrag (delta) is sent to an object that's being dragged.
        /// * OnDragOver (draggedObject) is sent to a game object when another object is dragged over its area.
        /// * OnDragOut (draggedObject) is sent to a game object when another object is dragged out of its area.
        /// * OnDragEnd () is sent to a dragged object when the drag event finishes.
        /// * OnTooltip (show) is sent when the mouse hovers over a collider for some time without moving.
        /// * OnScroll (float delta) is sent out when the mouse scroll wheel is moved.
        /// * OnKey (KeyCode key) is sent when keyboard or controller input is used.
        /// * OnSwipe
        /// * OnTwist
        /// * OnPinch
        /// * OnGyroscope
        /// * OnDeviceRotation
        /// </summary>
		public sealed class InputManager : Singleton<InputManager>
		{
            public class MouseOrTouch
            {
                public Vector2 pos;				
                public Vector2 lastPos;
            }

            private DeviceOrientation mOrientation;
   
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
                    //TODO: Notify
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
                for(int i=0; i<Input.touchCount;i++)
                {
                    Touch touch = Input.GetTouch(i);



                }

            }

            private void ProcessGyroscope()
            {


            }
		}
	}
}