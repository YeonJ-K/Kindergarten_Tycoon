using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class InGameCore : SingletonMono<InGameCore>
    {
        [Header("Base Managers")]
        public List<BaseManager> inGameManagers;
        private Dictionary<Type, BaseManager> managerDict =  new Dictionary<Type, BaseManager>();
        
        public static RoundManager ROUND => Instance.Get<RoundManager>();
        public static KidsManager KIDS => Instance.Get<KidsManager>();
        public static GridManager GRID => Instance.Get<GridManager>();
        public static CamViewManager VIEWER => Instance.Get<CamViewManager>();
        public static FurnitureManager FUR => Instance.Get<FurnitureManager>();
        public static KidsAIManager AI => Instance.Get<KidsAIManager>();
        public static EditorManager EDITOR => Instance.Get<EditorManager>();
        protected override bool IsPersistent => false;
        [HideInInspector] public static bool isGameReady = false;
 // ------- Init
        
        public T Get<T>() where T : BaseManager
        {
            var type = typeof(T);
            return managerDict.ContainsKey(type) ? managerDict[type] as T : null;
        }
        
        private async void Start()
        {
            await UniTask.WaitUntil(() => GameCore.isCoreReady);
            managerDict.Clear();
            
            for (int i = 0; i < inGameManagers.Count; i++)
            {
                await inGameManagers[i].Task_Init();

                var type = inGameManagers[i].GetType();
                managerDict.Add(type, inGameManagers[i]);
                float gauge = 50f + (float)(i+1) / inGameManagers.Count * 50f;
                CanvasRoot.instance.SetLoadingGaugeText(gauge);
            }
            
            for (int i = 0; i < inGameManagers.Count; i++)
                inGameManagers[i].Init();
            isGameReady = true;
            await UniTask.Delay(250);
            CanvasRoot.instance.SetLoadingGaugeText(100);
            await UniTask.Delay(500);
            CanvasRoot.instance.SetLoadingIsOn(false);
        }
        
    }
}
