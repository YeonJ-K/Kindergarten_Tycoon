using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
// ────────────────────────────────────────────────────────────
// 화면 전환 컨트롤러
//   상황실(맵 전체) ↔ 메인 룸(MainRoom 구역만) 을 버튼으로 오간다.
//   카메라를 목표 구역에 맞춰 부드럽게 줌/이동한다.
// ────────────────────────────────────────────────────────────

    public enum ViewMode
    {
        Overview,
        MainRoom
    } // 상황실 / 메인룸

    public class CamViewManager : BaseManager
    {
        private Camera cam;

        [Header("전환 애니메이션")] private float transitionTime = 0.6f; // 줌 이동 시간(초)
        private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private float padding; // 구역 주변 여백(유닛)

        [Header("화면별 UI (켜고 끌 오브젝트)")] [SerializeField]
        private GameObject overviewUI; // 상황실 UI (스트레스 수치 등)

        [SerializeField] private GameObject mainRoomUI; // 메인 룸 UI (상태창 등)

        public ViewMode currentMode { get; private set; }

        Coroutine transition;
        
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            SetMainCamera();
            ApplyImmediate(ViewMode.Overview);
            base.Init();
        }

        public void SetMainCamera() => cam = Camera.main;
        public void GoMainRoom() => SwitchTo(ViewMode.MainRoom);
        public void GoOverview() => SwitchTo(ViewMode.Overview);
        public void SwitchTo(ViewMode mode)
        {
            currentMode = mode;
            UpdateUI(mode);

            // 목표 카메라 상태 계산
            GetTarget(mode, out Vector3 targetPos, out float targetSize);

            if (transition != null) StopCoroutine(transition);
            transition = StartCoroutine(TransitionTo(targetPos, targetSize));
        }

        // 모드별 목표 위치·줌 계산
        void GetTarget(ViewMode mode, out Vector3 pos, out float orthoSize)
        {
            Vector3 center;
            Vector2 size;

            if (mode == ViewMode.MainRoom)
            {
                center = InGameCore.GRID.GetZoneCenter(ZoneType.MainRoom);
                size = InGameCore.GRID.GetZoneSize(ZoneType.MainRoom);
                padding = 3.5f;
            }
            else // Overview: 맵 전체
            {
                center = InGameCore.GRID.WorldCenter;
                size = new Vector2(InGameCore.GRID.WorldWidth, InGameCore.GRID.WorldHeight);
                padding = 0.3f;
            }

            // Safe Area 비율 기준으로 구역이 꽉 차는 ortho size 계산 (Fill)
            Rect safe = Screen.safeArea;
            float safeAspect = safe.width / safe.height;

            float w = size.x + padding;
            float h = size.y + padding;
            float sizeByW = w / safeAspect / 2f;
            float sizeByH = h / 2f;
            if (mode == ViewMode.MainRoom)
                orthoSize = sizeByW;
            else
                orthoSize = Mathf.Max(sizeByW, sizeByH);


            pos = new Vector3(center.x, center.y, -10f);
        }

        // 부드러운 전환 코루틴
        IEnumerator TransitionTo(Vector3 targetPos, float targetSize)
        {
            Vector3 startPos = cam.transform.position;
            float startSize = cam.orthographicSize;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / transitionTime;
                float e = ease.Evaluate(Mathf.Clamp01(t));
                cam.transform.position = Vector3.Lerp(startPos, targetPos, e);
                cam.orthographicSize = Mathf.Lerp(startSize, targetSize, e);
                yield return null;
            }

            cam.transform.position = targetPos;
            cam.orthographicSize = targetSize;
            transition = null;
        }

        // 애니메이션 없이 즉시 적용 (시작용)
        void ApplyImmediate(ViewMode mode)
        {
            currentMode = mode;
            UpdateUI(mode);
            GetTarget(mode, out Vector3 pos, out float size);
            cam.transform.position = pos;
            cam.orthographicSize = size;
        }

        void UpdateUI(ViewMode mode)
        {
            if (overviewUI != null) overviewUI.SetActive(mode == ViewMode.Overview);
            if (mainRoomUI != null) mainRoomUI.SetActive(mode == ViewMode.MainRoom);
        }
    }
}