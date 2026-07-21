using System;
using UnityEngine;
using Infos;

public class GameRoundUI : MonoBehaviour
{
    public bool IsStatusOpen { get; private set; }
    MainRoomUI mainRoomUI;

    private void Awake()
    {
        mainRoomUI = gameObject.transform.GetChild(0).gameObject.GetComponent<MainRoomUI>(); 
    }

    private void Start()
    {
        IsStatusOpen = false;
        mainRoomUI.InitUI();
    }

    public void OpenStatusBox(KidAgent kid)
    {
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
        if (IsStatusOpen)
        {
            IsStatusOpen = false;
            mainRoomUI.StatusBoxSliding(IsStatusOpen);
        }
    }

    public void ClickToiletButton()
    {
        
    }
    
    public void ClickKitchenButton()
    {
        
    }

    public void ClickPlayRoomButton()
    {
        
    }

    public void ClickSleepRoomButton()
    {
        
    }
}
