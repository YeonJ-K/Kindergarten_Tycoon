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
            loadingGroup.DOFade(0, 1.5f).OnComplete(() =>
            {
                GameCore.UI.OpenUI<MainUI>(UIType.MainUI);
                CloseUI();
            });
        }
    }
}
