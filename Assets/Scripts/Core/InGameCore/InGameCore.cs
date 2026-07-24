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

        
        [HideInInspector] public static bool isCoreReady = false;
        [HideInInspector] public static bool isGamePlay = false;
 // ------- Init
        
        public T Get<T>() where T : BaseManager
        {
            var type = typeof(T);
            return managerDict.ContainsKey(type) ? managerDict[type] as T : null;
        }

        public async void Start()
        {

            managerDict.Clear();
            // 로딩하기

            float gauge = 0;

            for (int i = 0; i < inGameManagers.Count; i++)
            {
                await inGameManagers[i].Task_Init();

                var type = inGameManagers[i].GetType();
                managerDict.Add(type, inGameManagers[i]);

                gauge += 5;
                //CanvasRoot.instance.SetLoadingGaugeText(gauge);
            }

            await UniTask.Delay(500);
            //CanvasRoot.instance.SetLoadingGaugeText(70);

            // StaticGameData.Init();
            for (int i = 0; i < inGameManagers.Count; i++)
                inGameManagers[i].Init();

            isCoreReady = true;
        }
        
    }
}
