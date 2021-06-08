#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace Example2
{

    public class InputJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //摇杆根节点
        [SerializeField] RectTransform joystickRoot;
        //摇杆节点
        [SerializeField] RectTransform joystickNode;
        //摇杆方向节点
        [SerializeField] RectTransform joystickDirection;

        //摇杆半径
        [SerializeField] int joystickRadius = 200;
        //抬手摇杆复位速度
        [SerializeField, Range(0.01f, 1)] float revertPositionSpeed = 0.75f;
        //单击判定时间范围
        [SerializeField] float tapDuration = 0.1f;

        //摇杆事件回调
        public static Action OnJoystickMoveStart;
        public static Action<Vector2> OnJoystickMoving;
        public static Action OnJoystickMoveEnd;
        public static Action OnJoystickTap;

        //摇杆默认位置
        private Vector3 joystickDefaultPos;
        //按下时摇杆中心位置
        private Vector3 curJoystickOrigin;
        //按下时摇杆方向
        private Vector3 curJoystickDirection;

        internal static bool isInputing = false;        //是否按下摇杆
        private bool needToRevertRoot = false;  //根节点是否需要回位
        private bool needToRevertNode = false;  //遥感节点是否需要回位
        private bool isReadyToTap = false;      //是否判定单击

        //摇杆按下时间
        private float startInputTime;

        private void Awake()
        {
            joystickDefaultPos = joystickRoot.anchoredPosition;
        }

        private void Start()
        {
            joystickDirection.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnInputStart(Input.mousePosition);
            isInputing = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isInputing)
                OnInputIng(Input.mousePosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnInputEnd(Input.mousePosition);
            isInputing = false;
            if (needToRevertRoot)
            {
                RevertJoystickRootPos();
            }
            if (needToRevertNode)
            {
                RevertJoystickNodePos();
            }
        }
        private void OnInputStart(Vector3 point)
        {
            curJoystickOrigin = point;

            startInputTime = Time.unscaledTime;
            isReadyToTap = true;

            joystickNode.localPosition = Vector3.zero;

            joystickDirection.gameObject.SetActive(true);

            OnJoystickMoveStart?.Invoke();
        }
        float tempLength;
        private void OnInputIng(Vector3 point)
        {
            tempLength = (point - curJoystickOrigin).magnitude;

            if (tempLength < 0.01f)
            {
                curJoystickDirection = Vector3.zero;
                OnJoystickMoving?.Invoke(curJoystickDirection);

                if (isReadyToTap)
                {
                    if (Time.unscaledTime - startInputTime >= tapDuration)
                    {
                        isReadyToTap = false;
                    }
                }
            }
            else if (tempLength <= joystickRadius)
            {
                curJoystickDirection = (point - curJoystickOrigin).normalized * tempLength / joystickRadius;
                isReadyToTap = false;
            }
            else
            {
                curJoystickDirection = (point - curJoystickOrigin).normalized;
                isReadyToTap = false;
            }

            joystickNode.localPosition = curJoystickDirection * joystickRadius;
            if (curJoystickDirection == Vector3.zero)
                joystickDirection.up = Vector3.up;
            else
                joystickDirection.up = curJoystickDirection;

            OnJoystickMoving?.Invoke(curJoystickDirection);
        }
        private void OnInputEnd(Vector3 point)
        {
            curJoystickOrigin = joystickDefaultPos;

            needToRevertRoot = true;
            needToRevertNode = true;

            joystickDirection.gameObject.SetActive(false);

            if (isReadyToTap)
            {
                OnJoystickTap?.Invoke();
                isReadyToTap = false;
            }

            OnJoystickMoveEnd?.Invoke();
        }

        private void RevertJoystickRootPos()
        {
            if ((joystickRoot.position - joystickDefaultPos).sqrMagnitude > 0.1f)
            {
                joystickRoot.anchoredPosition = Vector3.Lerp(joystickRoot.anchoredPosition, joystickDefaultPos, revertPositionSpeed);
            }
            else
            {
                joystickRoot.anchoredPosition = joystickDefaultPos;
                needToRevertRoot = false;
            }
        }
        private void RevertJoystickNodePos()
        {
            if (joystickNode.localPosition.sqrMagnitude > 0.1f)
            {
                joystickNode.localPosition = Vector3.Lerp(joystickNode.localPosition, Vector3.zero, revertPositionSpeed);
            }
            else
            {
                joystickNode.localPosition = Vector3.zero;
                needToRevertNode = false;
            }
        }
    }
}
#endif