using System;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class ActiveFurniture : Furniture
    {
        [SerializeField] private GameObject usingKidsProfile;

        private Vector2Int resourcePos;
        private Vector2Int usePos;
        private ActiveFurnitureId furnitureId;
        private int availableKidCount;
        private float usingTime;


        // 임시용
        protected override void Awake()
        {
            base.Awake();
            spritePath = "Objects/furniture";
            furnitureName = "toilet01";
            furnitureType = 0;
            furnitureId = ActiveFurnitureId.Toilet;
            furnitureSprite.sprite = Resources.Load<Sprite>(spritePath + furnitureName);
            width = 1;
            height = 2;
            resourcePos = new Vector2Int(0, 1);
            usePos = new Vector2Int(0, 0);
            availableKidCount = 1;
        }

    }
}