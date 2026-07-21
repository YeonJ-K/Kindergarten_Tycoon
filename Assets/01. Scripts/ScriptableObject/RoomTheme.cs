using System.Collections.Generic;
using UnityEngine;
using Infos;

[CreateAssetMenu(fileName = "RoomTheme", menuName = "Scriptable Objects/RoomTheme")]
public class RoomTheme : ScriptableObject
{
    public Sprite[] roomSprite;
    
    public Sprite GetRoomSprite(ZoneType zone) => roomSprite[(int)zone-1];
}
