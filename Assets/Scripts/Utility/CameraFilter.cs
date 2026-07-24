using UnityEngine;

namespace YEONJI.Kindergarten
{
    [RequireComponent(typeof(Camera))]
    public class CameraFilter : MonoBehaviour
    {
        public float padding = 0f;

        Camera cam;
        private Rect lastSafeArea;

        private void Awake()
        {
            enabled = false;
        }

        public void Init()
        {
            cam = GetComponent<Camera>();
            Apply();
            enabled = true;
        }

        void Update()
        {
            if (Screen.safeArea != lastSafeArea)
                Apply();
        }

        void Apply()
        {
            lastSafeArea = Screen.safeArea;
            Rect safe = Screen.safeArea;
            cam.rect = new Rect(
                safe.x / Screen.width,
                safe.y / Screen.height,
                safe.width / Screen.width,
                safe.height / Screen.height
            );

            // 2) GridMap에서 실제 크기·중심을 읽어옴
            float mapWidth = InGameCore.GRID.WorldWidth + padding;
            float mapHeight = InGameCore.GRID.WorldHeight + padding;

            // 3) Fill 방식으로 꽉 채우기 (Safe Area 비율 기준)
            float safeAspect = safe.width / safe.height;
            float sizeByWidth = mapWidth / safeAspect / 2f;
            float sizeByHeight = mapHeight / 2f;
            cam.orthographicSize = Mathf.Min(sizeByWidth, sizeByHeight);

            // 4) 맵 중심 바라보기
            Vector3 c = InGameCore.GRID.WorldCenter;
            transform.position = new Vector3(c.x, c.y, -10f);
        }
    }
}