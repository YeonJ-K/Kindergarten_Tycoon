using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    [System.Serializable]
    public class FurniturePlacement
    {
        public GameObject prefab;
        public Vector2Int anchor;
    }

    public class FurnitureManager : BaseManager
    {
        // 유저 데이터 가져와서 각 위치에 맞는 가구 불러오기
        private int furnitureType; // 0 : Active / 1 : InActive

        // 저장할 데이터 -> 가구 타입, 가구 ID (enum ActiveFurnitureId), 가구 이름, 위치, 지역
        // 유저 데이터 만들면 Start나 Awake에서 가구 가져오기

        // 임시로 [SerializedFiled로 놓기]
        [SerializeField] private List<FurniturePlacement> furniturePlacements;
        
        
        public override async UniTask Task_Init()
        {
            await base.Task_Init();
        }

        public override void Init()
        {
            base.Init();
        }

    }
}