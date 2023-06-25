using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Managers
{
    public class InputManager : MonoBehaviour
    {
        private static Vector2 _firstMousePosition;
        private static Vector2 _lastMousePosition;

        public static Vector2 DeltaMousePosition { get; set; }

        private void Update()
        {
            if (GetMouseButtonDown())
            {
                _firstMousePosition = GetMousePosition();
                _lastMousePosition = _firstMousePosition;
                DeltaMousePosition = Vector2.zero;
            }
            else if (GetMouseButton() || GetMouseButtonUp())
            {
                var mousePosition = GetMousePosition();
                DeltaMousePosition = mousePosition - _lastMousePosition;
                _lastMousePosition = mousePosition;
            }
            else
            {
                DeltaMousePosition = Vector2.zero;
            }
        }

        public static bool IsPointerOverGameObject()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return GetMouseButton() && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            return GetMouseButton() && !EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public static bool GetMouseButton()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;
            
            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase != TouchPhase.Ended && touchPhase != TouchPhase.Canceled;
#else
            return Input.GetMouseButton(0);
#endif
        }

        public static bool GetMouseButtonDown()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public static bool GetMouseButtonUp()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;

            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }

        public static Vector2 GetMousePosition()
        {
            var mousePosition = Vector3.zero;

            if (GetMouseButton() || GetMouseButtonDown() || GetMouseButtonUp())
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                mousePosition = Input.GetTouch(0).position;
#else
                mousePosition = Input.mousePosition;
#endif
            }

            return mousePosition;
        }
    }
}