using UnityEngine;

namespace Infos
{
    public enum UserSex
    {
        Male,
        Female
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
        Requesting,
        MovingToZone,
        Doing,
        StressUp,
        Exiting
    }

    public enum KidsID
    {
        Girl01 = 1,
        Girl02,
        Girl03,
        Girl04,
        Girl05,
        
        Boy01 = 10,
        Boy02,
        Boy03,
        Boy04,
        Boy05,
    }
}
