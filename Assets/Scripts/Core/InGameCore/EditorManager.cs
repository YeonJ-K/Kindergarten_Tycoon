using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    // 맵 편집 담당.
    public class EditorManager : BaseManager
    {
        private enum EditState
        {
            None,
            Dragging,
            Confirming
        }

        private EditState state = EditState.None;
        private Camera cam;

        private Furniture editingFurniture;
        private const float placingAlpha = 0.6f;
        
        private RectTransform furnitureButtons;
        private RectTransform buttonCanvas;
        private Button checkButton;
        private Button cancelButton;   
        private bool canPlace;

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            cam = Camera.main;


            base.Init();
        }

        private void Update()
        {
            if (state == EditState.None) return;

            // 라운드가 시작되면 편집 불가 — 안전장치
            if (InGameCore.ROUND.roundStart)
            {
                CancelPlace();
                return;
            }

            switch (state)
            {
                case EditState.Dragging:
                    UpdateDragging();
                    break;
                case EditState.Confirming:
                    UpdateConfirming();
                    break;
            }
        }

        // Start Edit
        public void BeginPlace(Furniture furniture)
        {
            if (furniture == null) return;
            editingFurniture = furniture;
            state = EditState.Dragging;
            
            var editUI = GameCore.UI.OpenUI<FurnitureEdit>(UIType.FurnitureEditUI, Canvas_SortOrder.HIGHLIGHT);
            furnitureButtons = editUI.buttonGroup;
            checkButton = editUI.checkButton;
            cancelButton = editUI.cancelButton;
            buttonCanvas = GameCore.UI.GetRectUIHighLight();
            
            editingFurniture.SetReplaceMode(true);
            editingFurniture.SetFurnitureAlpha(placingAlpha);
            checkButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            checkButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);

        }
        
        private void UpdateDragging()
        {
            if (!TryGetPointerCell(out Vector2Int cell)) return;
            
            editingFurniture.SetFurnitureAnchor(cell);
            canPlace = InGameCore.GRID.CanPlace(editingFurniture.GetOccupiedCells());
            editingFurniture.SetFloorColor(canPlace ? Color.gray : Color.red);

            if (IsPointerReleased())
            {
                state = EditState.Confirming;
                ShowButtonsAtFurniture();
            }
        }
        
        private void UpdateConfirming()
        {
            UpdateButtonPosition();

            if (IsPointerPressed() && !IsPointerOverUI())
            {
                state = EditState.Dragging;
                HideButtons();
            }
        }
        
        private void UpdateButtonPosition()
        {
            // 가구 위쪽에 띄우기 위해 살짝 올림
            Vector3 worldPos = editingFurniture.transform.position + Vector3.up * 1f;

            // 월드 → 화면
            Vector2 screenPos = cam.WorldToScreenPoint(worldPos);

            // 화면 → Canvas 로컬 (Camera 모드라 카메라를 넘겨야 함)
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    buttonCanvas, screenPos, cam, out Vector2 localPos))
            {
                furnitureButtons.anchoredPosition = localPos;
            }
        }
        
        public void OnConfirm()
        {
            if (state != EditState.Confirming) return;
            if (!canPlace) return;   // 못 놓는 자리면 무시

            // TODO: 입구 막힘 검사 (스폰→메인룸 경로 살아있나)

            // 칸 점유 + id 발급 + 표 등록 (FurnitureManager 쪽)
            // TODO: InGameCore.FUR.ConfirmPlace(editingFurniture);

            editingFurniture.SetFurnitureAlpha(1f);                 // 알파 원복
            editingFurniture.SetReplaceMode(false); // Floor 끄기

            // TODO: 소유 데이터에 앵커 기록 (JSON 저장)

            EndPlace();
        }
        
        public void OnCancel()
        {
            if (state != EditState.Confirming) return;

            // TODO: 보관함으로 되돌리기 (소유는 유지, 앵커 비움)
            // TODO: 화면에서 치우기 (Destroy 또는 숨김)

            EndPlace();
        }
        
        private void CancelPlace()
        {
            // 라운드 시작 등으로 강제 종료
            EndPlace();
        }
        
        private void EndPlace()
        {
            HideButtons();
            editingFurniture = null;
            state = EditState.None;
        }
        
        private bool TryGetPointerCell(out Vector2Int cell)
        {
            cell = default;
            if (!GetPointerWorld(out Vector3 world)) return false;
            cell = InGameCore.GRID.WorldToGrid(world);
            return true;
        }
        
        private bool GetPointerWorld(out Vector3 world)
        {
            world = default;

#if UNITY_EDITOR
            world = cam.ScreenToWorldPoint(Input.mousePosition);
            return true;
#else
            if (Input.touchCount == 0) return false;
            world = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            return true;
#endif
        }
        
        private bool IsPointerReleased()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonUp(0);
#else
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
#endif
        }
        
        private bool IsPointerPressed()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
#else
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
#if UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#else
            return Input.touchCount > 0 &&
                   EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        }
        
        private void ShowButtonsAtFurniture()
        {
            furnitureButtons.gameObject.SetActive(true);
            UpdateButtonPosition();
        }

        private void HideButtons()
        {
            if (furnitureButtons != null)
                furnitureButtons.gameObject.SetActive(false);
        }
        
    }
}
