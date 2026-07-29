using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class InActiveFurniture : Furniture
    {
        private int satisfaction;
        private ZoneType satisfiedZone;
        private int setId;


        public override void Init(FurnitureData data, Vector2Int anchor)
        {
            base.Init(data, anchor);

            if (!(data is InActiveFurnitureData inactiveData))
            {
                // InActive Data 아님 로깅
                return;
            }
            
            satisfaction = inactiveData.satisfaction;
            satisfiedZone = inactiveData.satisfiedZone;
            setId = inactiveData.setId;
        }
        
        
        // ---- Get
        public int GetFurnitureSatisfaction() => satisfaction;
        public ZoneType GetFurnitureZone() => satisfiedZone;
        public int GetFurnitureSetId() => setId;
        public bool HasSet() => setId != 0;
        
    }
}