#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;
using UnityEngine.UI;

namespace Example2
{
    public class FlyingWord : MonoBehaviour
    {
        public Text text;
        public Vector3 position;
        public RectTransform rt;
        private new Camera camera;

        private void Start()
        {
            var canvas = GameManager.I.canvasRT.GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                camera = Camera.main;
            Destroy(gameObject, 3f);
        }

        // Update is called once per frame
        void Update()
        {
            var pos = Camera.main.WorldToScreenPoint(position);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.I.canvasRT, pos, camera, out Vector2 outVec))
                rt.anchoredPosition = outVec;
        }
    }
}
#endif