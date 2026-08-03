using System;
using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class Furniture : MonoBehaviour
    {
        [SerializeField] protected GameObject furnitureFloor;
        [SerializeField] protected SpriteRenderer furnitureSprite;

        // 엑셀 데이터로 읽어서 가져올 값
        protected int id;
        protected FurnitureType type;
        protected string furnitureName;
        protected string spritePath;
        protected int width;
        protected int height;
        protected int price;
        protected Vector2Int spritePlacePos;

        // 게임 상에서 필요한 변수
        protected Vector2Int placedCell;
        protected bool isReplace;

        private readonly List<GameObject> floorTiles = new List<GameObject>();

        public virtual void Init(FurnitureData data, Vector2Int anchor)
        {
            if (data == null)
            {
                // 가구 data null 로깅
                return;
            }

            id = data.id;
            type = data.type;
            furnitureName = data.furnitureName;
            spritePath = data.spritePath;
            width = data.width;
            height = data.height;
            price = data.price;
            spritePlacePos = data.spritePlacePos;
            placedCell = anchor;

            if (furnitureFloor != null)
                furnitureFloor.SetActive(false);

            SettingFurnitureSprite();
            SettingFurnitureFloor();
            SetReplaceMode(false);
        }

        // ---- Get
        public int GetFurnitureId() => id;
        public FurnitureType GetFurnitureType() => type;
        public string GetFurnitureName() => furnitureName;
        public Vector2Int GetFurnitureSize() => new Vector2Int(width, height);
        public int GetFurniturePrice() => price;
        public Vector2Int GetFurnitureAnchor() => placedCell;
        public bool IsReplaceMode() => isReplace;
            
        // ---- Main
        protected virtual void SettingFurnitureFloor()
        {
            ClearFurnitureFloor();
            if (furnitureFloor == null) return;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector2Int cell = placedCell + new Vector2Int(i, j);
                    Vector3 world = InGameCore.GRID.GridToWorld(cell.x, cell.y);
                    GameObject tile = Instantiate(furnitureFloor, world, Quaternion.identity);
                    tile.SetActive(true);
                    floorTiles.Add(tile);
                }
            }
        }

        protected virtual void SettingFurnitureSprite()
        {
            if (furnitureSprite == null) return;
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            Debug.Log(spritePath + furnitureName);
            if (sprite == null)
            {
                // 스프라이트 찾을 수 없음 로깅
                return;
            }

            furnitureSprite.sprite = sprite;
            Vector2Int cell = placedCell + spritePlacePos;
            furnitureSprite.transform.position = InGameCore.GRID.GridToWorld(cell.x, cell.y);
            furnitureSprite.sortingOrder = Mathf.RoundToInt(-furnitureSprite.transform.position.y * 100);
        }

        private void ClearFurnitureFloor()
        {
            for (int i = 0; i < floorTiles.Count; i++)
            {
                if (floorTiles[i] != null)
                    Destroy(floorTiles[i]);
            }
            floorTiles.Clear();
        }

        public void SetReplaceMode(bool on)
        {
            isReplace = on;
            for (int i = 0; i < floorTiles.Count; i++)
            {
                if (floorTiles[i] != null)
                    floorTiles[i].SetActive(on);
            }
        }

        public void SetFloorColor(Color color)
        {
            for (int i = 0; i < floorTiles.Count; i++)
            {
                if (floorTiles[i] == null) continue;
                var sr = floorTiles[i].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = color;
            }
        }

        public void SetFurnitureAlpha(float alpha)
        {
            furnitureSprite.color = new Color(1f,1f,1f,alpha);
        }


        public List<Vector2Int> GetOccupiedCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells.Add(placedCell + new Vector2Int(x, y));
 
            return cells;
        }
        
        public void SetFurnitureAnchor(Vector2Int anchor)
        {
            placedCell = anchor;
            SettingFurnitureSprite();
            SettingFurnitureFloor();
            SetReplaceMode(isReplace);
        }
    }    
}