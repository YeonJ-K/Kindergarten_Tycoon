using System;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class GameRoundUI : MonoBehaviour
    {
        public bool IsStatusOpen { get; private set; }
        MainRoomUI mainRoomUI;

        private KidAgent selectedKid;

        private void Awake()
        {
            mainRoomUI = gameObject.transform.GetChild(0).gameObject.GetComponent<MainRoomUI>(); 
        }

        private void Start()
        {
            IsStatusOpen = false;
            mainRoomUI.InitUI();
        }

        private void Update()
        {
            if (selectedKid == null) return;
            
            var needs = selectedKid.Context.needs;
            for (int i = 0; i < (int)NeedType.All; i++)
            {
                NeedType type = (NeedType)i;
                NeedLevel level = needs.Get(type);
                if (level <= NeedLevel.Normal)
                {
                    mainRoomUI.OpenTimer(type);
                    mainRoomUI.SettingTimer(type, needs.GetTimerRatio(type));
                }
                else
                {
                    mainRoomUI.CloseTimer(type);
                }

            }
        }

        public void OpenStatusBox(KidAgent kid)
        {
            if (selectedKid != null && selectedKid != kid)
                selectedKid.Context.releaseWaiting = true;
            
            selectedKid = kid;
            IsStatusOpen = true;
            Debug.Log(kid.name);
            string kidName = kid.name.Replace("(Clone)", "");
            kidName = kidName.Replace("Kid", "");
            kidName = kidName.Replace("_", "");
            
            for (int i = 0; i < (int)NeedType.All; i++)
            {
                NeedType type = (NeedType)i;
                NeedLevel level = kid.Context.needs.Get(type);
                mainRoomUI.SettingStatusBox(type, level, kidName);
            }
            mainRoomUI.StatusBoxSliding(IsStatusOpen);
        }

        public void CloseStatusBox()
        {
            if (selectedKid != null)
            {
                selectedKid.Context.releaseWaiting = true;
                selectedKid = null;
            }

            if (IsStatusOpen)
            {
                IsStatusOpen = false;
                mainRoomUI.StatusBoxSliding(IsStatusOpen);
            }
        }

        public void ClickToiletButton()
        {
            if (selectedKid == null) return;
            if (selectedKid.Context.needs.Get(NeedType.Toilet) > NeedLevel.Normal) return;
            selectedKid.Context.playerProcessNeed = NeedType.Toilet;
            selectedKid.Context.machine.ChangeState(new MovingToZoneState());
            CloseStatusBox();
        }
        
        public void ClickKitchenButton()
        {
            if (selectedKid == null) return;
            if (selectedKid.Context.needs.Get(NeedType.Hunger) > NeedLevel.Normal) return;
            selectedKid.Context.playerProcessNeed = NeedType.Hunger;
            selectedKid.Context.machine.ChangeState(new MovingToZoneState());
            CloseStatusBox();
        }

        public void ClickPlayRoomButton()
        {
            if (selectedKid == null) return;
            if (!selectedKid.Context.needs.wantPlay) return;
            selectedKid.Context.playerProcessNeed = NeedType.None;
            selectedKid.Context.machine.ChangeState(new MovingToZoneState());
            CloseStatusBox();
        }

        public void ClickSleepRoomButton()
        {
            if (selectedKid == null) return;
            if (selectedKid.Context.needs.Get(NeedType.Sleep) > NeedLevel.Normal) return;
            selectedKid.Context.playerProcessNeed = NeedType.Sleep;
            selectedKid.Context.machine.ChangeState(new MovingToZoneState());
            CloseStatusBox();
        }
    }
}
