using System;
using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class RoomDecorate : MonoBehaviour
    {
        [SerializeField] RoomTheme theme;

        public void Init()
        {
            foreach (var zoneRect in InGameCore.GRID.presetZones)
            {
                Sprite sprite = Resources.Load<Sprite>(zoneRect.floorSpriteName);
                if (sprite == null) continue;

                for (int x = zoneRect.x; x < zoneRect.x + zoneRect.width; x++)
                {
                    for (int y = zoneRect.y; y < zoneRect.y + zoneRect.height; y++)
                    {
                        GameObject go = new GameObject("Floor");
                        var sr = go.AddComponent<SpriteRenderer>();
                        sr.sprite = sprite;
                        sr.sortingOrder = 0;
                        go.transform.position = InGameCore.GRID.GridToWorld(x, y);
                    }
                }
            }
        }
    }
}