using System;
using System.Reflection;

namespace YEONJI.Kindergarten
{
    public static class EnumHelper
    {
        public static string enumName(this Enum e) => GetEnum(e);

        public static string GetEnum(Enum e)
        {
            Type type = e.GetType();
            FieldInfo field = type.GetField(e.ToString());
            if (field.GetCustomAttributes(typeof(EnumName), false) is EnumName[] attrs && attrs.Length > 0)
                return attrs[0].Value;
            
            return e.ToString();
        }
    }

    public class EnumName : Attribute
    {
        private string _value;
        public EnumName(string value) => _value = value;
        public string Value => _value;
    }

    public enum UIType
    {
        None,
        
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_FindCard")]
        MiniGame_FindCardPlay,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_HibiscusPlay")]
        MiniGame_HibiscusPlay,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_RoundAlert")]
        RoundAlert,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_ErrorMessage")]
        ErrorMessage,
        [UIAttrType("Prefab/Canvas_Screen/MainUI")]
        MainUI,
        [UIAttrType("Prefab/Canvas_Screen/GameRoundUI")]
        GameRoundUI,
        [UIAttrType("Prefab/Canvas_HUD/GameStatusUI")]
        GameStatusUI,
        [UIAttrType("Prefab/Canvas_Screen/EditUI")]
        EditUI,
        [UIAttrType("Prefab/Canvas_Screen/LoadingUI")]
        LoadingUI,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_ResultUI")]
        ResultUI,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_BuyActiveFurniture")]
        BuyActiveFurnitureUI,
        [UIAttrType("Prefab/Canvas_POPUP/POPUP_BuyInActiveFurniture")]
        BuyAInActiveFurnitureUI,
        [UIAttrType("Prefab/Canvas_HighLight/FurnitureEditUI")]
        FurnitureEditUI,
    }
    
    public enum Canvas_SortOrder
    {
        [EnumName("화면")] SCREEN = 1000,
        [EnumName("허드")] HUD = 2000,
        [EnumName("팝업")] POPUP = 3000,
        [EnumName("하이라이트")] HIGHLIGHT = 5000,
    }

    public enum StateEnterPopupType
    {
        None,
        Clear,
        AllClear,
    }

    public enum GameState
    {
        Lobby,
        SettingRoom,
        SectionSetting,
        RoundPlay,
        StressEvent,
    }

    public enum KidType
    {
        None = -1,
        
        [EnumName("KidGirl_01(Clone)")] Girl01 = 0,
        [EnumName("KidGirl_02(Clone)")] Girl02 = 1,
        [EnumName("KidGirl_03(Clone)")] Girl03 = 2,
        [EnumName("KidGirl_04(Clone)")] Girl04 = 3,
        [EnumName("KidGirl_05(Clone)")] Girl05 = 4,
        
        [EnumName("KidBoy_01(Clone)")] Boy01 = 10,
        [EnumName("KidBoy_02(Clone)")] Boy02 = 11,
        [EnumName("KidBoy_03(Clone)")] Boy03 = 12,
        [EnumName("KidBoy_04(Clone)")] Boy04 = 13,
        [EnumName("KidBoy_05(Clone)")] Boy05 = 14,
    }

    public enum UserSex
    {
        None = -1,
        [EnumName("Player_m(Clone)")] Male = 0,
        [EnumName("Player_w(Clone)")] Female = 1
    }
    
    public enum ZoneType
    {
        None,
        Entrance,
        MainRoom,
        RestRoom,
        PlayRoom,
        SleepRoom,
        DiningRoom,
        WallPaper
    }

    public enum WallPiece
    {
        None,

        // 바깥 테두리 직선
        Top, Bottom, Left, Right,
        // 바깥 테두리 코너
        TopCornerLeft, TopCornerRight, BottomCornerLeft, BottomCornerRight,
        
        // 공유 중간 벽
        MiddleLeft, MiddleRight, Middle2,
        
        // 위쪽 테두리에서 세로 중간벽 갈라짐
        LeftTop, RightTop,
        
        // 아래쪽 테두리에서 세로 중간벽 갈라짐
        BottomLeftMiddle, BottomRightMiddle,
        
        // 가로 중간벽 + 세로 중간벽 T자
        MiddleLeftMiddle, MiddleRightMiddle,
    }
    

    public enum KidState
    {
        Entering,
        Wandering,
        Waiting,
        MovingToZone,
        Doing,
        StressUp,
        Exiting
    }

    public enum KidEmotion
    {
        Normal,
        Tired,
        Angry
    }

    public enum NeedLevel
    {
        VeryBad = 1,
        Bad = 2,
        Normal = 3,
        Good = 4,
        VeryGood = 5
    }

    public enum NeedType
    {
        None = -1,
        Hunger,
        Toilet,
        Sleep,
        All
    }

    public enum CardMiniGameLevel
    {
        EASY,
        NORMAL,
        HARD
    }

    public enum FurnitureType
    {
        Active,
        InActive
    }

    public enum ActiveFurnitureId
    {
        None = 0,
        Toilet,
        Bed,
        TableWithChair
    }
    
}
