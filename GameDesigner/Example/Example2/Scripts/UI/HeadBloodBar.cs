#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;
using UnityEngine.UI;

namespace Example2
{
    public class HeadBloodBar : MonoBehaviour
    {
        public RectTransform rt;
        public Vector3 offset = new Vector3(0, 1f, 0);
        public Image image;
        public Text text;
        public Transform target;
        private CanvasGroup canvasGroup;
        private new Camera camera;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            var canvas = GameManager.I.canvasRT.GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            var pos = Camera.main.WorldToScreenPoint(target.position + offset);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.I.canvasRT, pos, camera, out Vector2 outVec))
                rt.anchoredPosition = outVec;
            var dis = (target.position - Camera.main.transform.position).magnitude;
            canvasGroup.alpha = (dis < 20f) ? 1f : 0f;
        }
    }
}
#endif