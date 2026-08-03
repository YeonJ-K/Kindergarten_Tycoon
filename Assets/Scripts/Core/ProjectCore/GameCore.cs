using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{

    public class GameCore : SingletonMono<GameCore>
    {
        // ------- Param
        [Header("Base Managers")] 
        public List<BaseManager> managerList;
        private Dictionary<Type, BaseManager> managerDict = new Dictionary<Type, BaseManager>();
        public float InitProgress { get; private set; }
        public static UIManager UI { get => Instance.Get<UIManager>(); }
        public static DataManager DATA { get => Instance.Get<DataManager>(); }
        public static SoundManager SOUND { get => Instance.Get<SoundManager>(); }
        public static ResourceManager RSS { get => Instance.Get<ResourceManager>(); }
        
        [HideInInspector] public static bool isCoreReady = false;

        // ------- Init
        
        public T Get<T>() where T : BaseManager
        {
            var type = typeof(T);
            return managerDict.ContainsKey(type) ? managerDict[type] as T : null;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private async void Start()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Application.targetFrameRate = 60; // 프레임 고정
            Time.timeScale = 1;

            managerDict.Clear();
            // 로딩하기
            CanvasRoot.instance.SetLoadingIsOn(true);
            CanvasRoot.instance.SetLoadingGaugeText(0);
            float gauge = 0;

            for (int i = 0; i < managerList.Count; i++)
            {
                await managerList[i].Task_Init();

                var type = managerList[i].GetType();
                managerDict.Add(type, managerList[i]);

                gauge = (float)(i + 1) / managerList.Count * 50f;
                CanvasRoot.instance.SetLoadingGaugeText(gauge);
            }
            

            // StaticGameData.Init();
            for (int i = 0; i < managerList.Count; i++)
                managerList[i].Init();
            
            await UniTask.Delay(250);


            isCoreReady = true;

            //TutorialStep();


        }
        
        // ------- Init
        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                // 멈춘 거 로깅
                if (GameCore.Instance != null)
                {
                    if (GameCore.UI != null)
                    {
                        // 게임이 강제로 닫히면 뭘 저장해야할까
                    }

                }
            }
        }

        public void OnApplicationQuit()
        {
            // 게임 강제 종료 로깅
            if (GameCore.Instance != null)
            {
                
                
            }
        }
        
        // ------- Logic
        private void TutorialStep()
        {
            
        }
    }
}
