using System;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class LoadingUI : UIBase
    {
        public override UIType UIType { get { return UIType.LoadingUI; } }
        [SerializeField] CanvasGroup loadingGroup;
        [SerializeField] TextMeshProUGUI loadingText;

        public override void OpenUI()
        {
            float originalAlpha = 1f;
            loadingGroup.alpha = originalAlpha;
            base.OpenUI();
        }

        async void Start()
        {
            await UniTask.WaitUntil(() => InGameCore.isGameReady);
            CloseLoading();
        }
        
        public void CloseLoading(Action action = null)
        {
            // 페이드 시작과 동시에 MainUI를 먼저 열어, 로딩창 뒤에서 바로 드러나게 함
            OpenMainUI();
            loadingGroup.DOFade(0, 1f).OnComplete(() =>
            {
                action?.Invoke();
                CloseUI();
            });
        }

        private void OpenMainUI()
        {
            var mainUI = GameCore.UI.OpenUI<MainUI>(UIType.MainUI, Canvas_SortOrder.SCREEN);
            if (mainUI == null) return;

            mainUI.SettingPlayerProfileBox(GameCore.DATA.userSex, GameCore.DATA.userName,
                GameCore.DATA.gameDate, GameCore.DATA.money);
        }
    }
}
