using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class FurnitureData
    {
        public int id; 
        public FurnitureType type;
        public string furnitureName; // 스프라이트 파일명
        public string spritePath;
        public int width = 1;
        public int height = 1;
        public int price;

        public Vector2Int spritePlacePos;
    }
    
    [System.Serializable]
    public class ActiveFurnitureData : FurnitureData
    {
        public List<Vector2Int> usingPos = new List<Vector2Int>();
 
        public NeedType processNeed;       // 해결하는 요구
        public int availableKidCount = 1;  // 동시 사용 가능 인원
        public float usingSec;             // 해결에 걸리는 시간
    }
 
    [System.Serializable]
    public class InActiveFurnitureData : FurnitureData
    {
        public int satisfaction;           // 만족도
        public ZoneType satisfiedZone;     // 만족도가 적용되는 구역
        public int setId;                  // 0이면 세트 없음
    }
}
