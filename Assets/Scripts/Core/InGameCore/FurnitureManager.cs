using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    [System.Serializable]
    public class FurniturePlacement
    {
        public int furniturePlacementId;
        public GameObject prefab;
        public Vector2Int anchor;
    }

    public class FurnitureManager : BaseManager
    {
        public static int furnitureNum { get; private set; }

        // 임시로 [SerializedFiled로 놓기]
        [SerializeField] private List<FurniturePlacement> furniturePlacements;
        // 임시로 [SerializedFiled로 놓기]
        [SerializeField] private GameObject activePrefab;
        [SerializeField] private GameObject inactivePrefab;
        
        private Dictionary<int, ActiveFurniture> activeFurnitureDict = new();
        private Dictionary<int, InActiveFurniture> inactiveFurnitureDict = new();
        
        
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }



        public override void Init()
        {
            base.Init();
            PlaceFurniture();            
        }

        // Temp
        private void PlaceFurniture()
        {
            var data = GameCore.DATA.Furniture.GetActive(0);
            Debug.Log(data == null ? "데이터 null" : $"불러옴: {data.furnitureName}, {data.width}x{data.height}");
            data.usingPos.Add(new Vector2Int(0, 0));

            Vector2Int anchor = new Vector2Int(1, 1);
            
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int x = 0; x < data.width; x++)
            for (int y = 0; y < data.height; y++)
                cells.Add(anchor + new Vector2Int(x, y));
            
            if (InGameCore.GRID.CanPlace(cells))
            {
                GameObject prefab = (data.type == FurnitureType.Active) ? activePrefab : inactivePrefab;
                GameObject go = Instantiate(prefab, InGameCore.GRID.GridToWorld(anchor.x, anchor.y),Quaternion.identity);
                ActiveFurniture furniture = go.GetComponent<ActiveFurniture>();
                furnitureNum++;
                furniture.Init(data, anchor);
                var occupiedCells = furniture.GetOccupiedCells();
                var usePosition = furniture.GetUseAbsolutePos();
                foreach (var pos in usePosition)
                {
                    occupiedCells.Remove(pos);
                }

                InGameCore.GRID.Place(occupiedCells, furnitureNum);
                activeFurnitureDict.Add(furnitureNum, furniture);
                
                for (int i = 0; i < data.usingPos.Count; i++)
                {
                    Vector2Int abs = anchor + data.usingPos[i];
                }
            }
        }

        public ActiveFurniture FindNeedsFurniture(NeedType needs)
        {
            foreach (var furniture in activeFurnitureDict.Values)
            {
                if (furniture.GetFurnitureNeedsType() != needs) continue;
                if (furniture.CanUseActiveFurniture()) return furniture;
            }

            return null;
        }

        public void LoadUserFurniture()
        {
            
        }

        public ZoneType GetActiveFurnitureZone(NeedType needs)
        {
            switch (needs)
            {
                case NeedType.Hunger :
                    return ZoneType.DiningRoom;
                
                case NeedType.Toilet :
                    return ZoneType.RestRoom;
                
                case NeedType.Sleep :
                    return ZoneType.SleepRoom;
                
            }
            return ZoneType.None;
        }

    }
}