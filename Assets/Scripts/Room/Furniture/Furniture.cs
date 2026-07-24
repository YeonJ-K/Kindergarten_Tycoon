using System;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class Furniture : MonoBehaviour
    {
        // 엑셀 데이터로 읽어서 가져올 값
        protected string spritePath;
        protected int furnitureType;
        protected string furnitureName;
        protected SpriteRenderer furnitureSprite;
        protected int width;
        protected int height;
        protected int price;
        [SerializeField] protected GameObject furnitureFloor;

        // 게임 상에서 필요한 변수
        protected bool isReplace;

        protected virtual void Awake()
        {
            furnitureSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
            furnitureFloor.SetActive(false);

        }
    }
}