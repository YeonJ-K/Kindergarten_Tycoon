using UnityEngine;

namespace YEONJI.Kindergarten
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Data/RoomData")]
    public class RoomData : ScriptableObject
    {
        public ZoneType type;

        public Vector2Int defaultSize;
        // public List<ObjectData> requiredObjects;
    }
}