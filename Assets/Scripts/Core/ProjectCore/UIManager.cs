using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class UIManager : BaseManager
    {
        private Transform trUIScreen;
        private Transform trUIPopup;
        private Transform trUIHud;

        private RectTransform rectUIScreen;
        private RectTransform rectUIPopup;
        private RectTransform rectUIHud;

        private Camera mainCamera;

        private readonly Dictionary<UIType, UIBase> dicUI = new Dictionary<UIType, UIBase>();
        private readonly List<UIBase> openUIList = new List<UIBase>();

        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            SetMainCamera();
            SetTrUIParent(CanvasRoot.instance.trScreenParent, CanvasRoot.instance.trPopupParent, CanvasRoot.instance.trHudParent);
            base.Init();
            
        }

        // -------- Set
        public void SetTrUIParent(Transform trScreen, Transform trPopup, Transform trHud)
        {
            if (trScreen == null || trPopup == null)
            {
                // 로깅
                return;
            }

            trUIScreen = trScreen;
            trUIPopup = trPopup;
            trUIHud = trHud;

            rectUIScreen = trUIScreen.GetComponent<RectTransform>();
            rectUIPopup = trUIPopup.GetComponent<RectTransform>();
            rectUIHud = trUIHud.GetComponent<RectTransform>();
        }

        public void SetMainCamera() => mainCamera = Camera.main;

        public void ResetUIList()
        {
            dicUI.Clear();
        }

        // -------- Get
        public Camera GetMainCamera() => mainCamera;

        public UIBase GetUI(UIType type)
        {
            if (dicUI.ContainsKey(type))
                return dicUI[type];

            return null;
        }

        public T GetUI<T>(UIType type)
        {
            if (dicUI.ContainsKey(type))
                return dicUI[type].GetComponent<T>();

            return default(T);
        }

        private Transform GetParent(Canvas_SortOrder sort)
        {
            switch (sort)
            {
                case Canvas_SortOrder.POPUP:
                    return trUIPopup;
                case Canvas_SortOrder.SCREEN:
                    return trUIScreen;
                case Canvas_SortOrder.HUD:
                    return trUIHud;
                default:
                    return null;
            }
        }

        public RectTransform GetRectUIScreen() => rectUIScreen;
        public RectTransform GetRectUIPopup() => rectUIPopup;
        public RectTransform GetRectUIHud() => rectUIHud;

        // -------- Main 
        public async UniTask OpenUIAsync(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP,
            Action openAfter = null, Action closeAfter = null)
        {
            var openUI = GetUI(uiType);
            if (openUI == null)
            {
                openUI = await LoadUIAsync_Resources(uiType, sortOrder);


                if (openUI == null)
                {
                    // 에러 로그 UI 로드 실패
                    return;
                }
            }
            WireOpenCloseCallbacks(openUI, openAfter, closeAfter);
            if (!openUIList.Contains(openUI))
                openUIList.Add(openUI);

            openUI.OpenUI();
            RefreshPopupSortingOrder();
        }

        public T OpenUI<T>(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP, Action openAfter = null,
            Action closeAfter = null) where T : UIBase
        {
            var openUI = GetUI(uiType);
            if (openUI == null)
            {
                openUI = LoadUISync_Resources(uiType, sortOrder);

                if (openUI == null)
                {
                    // 에러 로그 UI 로드 실패
                    return null;
                }
            }
            WireOpenCloseCallbacks(openUI, openAfter, closeAfter);
            if (!openUIList.Contains(openUI))
                openUIList.Add(openUI);

            openUI.OpenUI();
            RefreshPopupSortingOrder();
            return openUI as T;
        }
        
        // -------- Logic 
        
        private void WireOpenCloseCallbacks(UIBase openUI, Action openAfter, Action closeAfter)
        {
            openUI.callOpenAfter = () =>
            {
                openAfter?.Invoke();
            };

            openUI.callCloseAfter = () =>
            {
                openUIList.Remove(openUI);
                closeAfter?.Invoke();
                RefreshPopupSortingOrder();
            };
        }
        
        private async UniTask<UIBase> LoadUIAsync_Resources(UIType uiType, Canvas_SortOrder sortOrder)
        {
            if (trUIScreen == null || trUIPopup == null || trUIHud == null)
            {
                // UI parent 없음 로깅
                return null;
            }

            Transform parentTr = GetParent(sortOrder);
            string path = UIAttrUtil.GetUIAttributeResourceName(uiType);
            
            ResourceRequest req = Resources.LoadAsync<GameObject>(path);
            await req.ToUniTask();

            var prefab = req.asset as GameObject;
            if (prefab == null)
            {
                // 리소스 로드 실패 로깅
                return null;
            }
            
            var go = GameObject.Instantiate(prefab, parentTr, worldPositionStays: false);
            var uiBase = go.GetComponent<UIBase>();
            if (uiBase == null)
            {
                // 리소스 생성 실패 로깅
                GameObject.Destroy(go);
                return null;
            }
            
            dicUI[uiType] = uiBase;
            return uiBase;
        }
        
        private UIBase LoadUISync_Resources(UIType uiType, Canvas_SortOrder sortOrder)
        {
            if (trUIScreen == null || trUIPopup == null || trUIHud == null)
            {
                // UI parent 없음 로깅
                return null;
            }

            Transform parentTr = GetParent(sortOrder);
            string path = UIAttrUtil.GetUIAttributeResourceName(uiType);

            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                // 리소스 로드 실패 로깅
                return null;
            }

            var go = GameObject.Instantiate(prefab, parentTr, worldPositionStays: false);
            var uiBase = go.GetComponent<UIBase>();
            if (uiBase == null)
            {
                // 리소스 생성 실패 로깅
                GameObject.Destroy(go);
                return null;
            }

            dicUI[uiType] = uiBase;
            return uiBase;
        }
        
        private void RefreshPopupSortingOrder()
        {
            int count = openUIList.Count;
            int popupIndex = (int)Canvas_SortOrder.POPUP;
            int screenIndex = (int)Canvas_SortOrder.SCREEN;

            for (int i = 0; i < count; i++)
            {
                if (openUIList[i] == null)
                    continue;

                Canvas baseCanvas = openUIList[i].GetCanvas();
                if (baseCanvas != null && openUIList[i].sortOrder == Canvas_SortOrder.POPUP)
                {
                    baseCanvas.sortingOrder = popupIndex;
                    popupIndex += 10;
                }
                else if (baseCanvas != null && openUIList[i].sortOrder == Canvas_SortOrder.SCREEN)
                {
                    baseCanvas.sortingOrder = screenIndex;
                    screenIndex += 10;
                }
            }
        }
    }
}