using UnityEngine;

namespace Infos
{
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

    public enum Dir
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum WallPiece
    {
        None,
        Top, Bottom, Left, Right,
        Middle, Middle2,
        TopLeft, TopRight, BottomLeft, BottomRight,
        Top3way, Bottom3way, Left3way, Right3way,
        MiddleCross,
        MiddleComboUp, MiddleComboDown, MiddleComboLeft, MiddleComboRight,
    }
}
