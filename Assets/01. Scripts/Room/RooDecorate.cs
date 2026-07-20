using System;
using System.Collections.Generic;
using UnityEngine;

public class RooDecorate : MonoBehaviour
{
    [SerializeField] RoomTheme theme;

    private void Start()
    {
        if (theme == null)
            return;
        
        foreach (var zoneRect in GridMap.instance.presetZones)
        {
            for (int x = zoneRect.x; x < zoneRect.x + zoneRect.width; x++)
            {
                for (int y = zoneRect.y; y < zoneRect.y + zoneRect.height; y++)
                {
                    if (GridMap.instance.GetCell(x, y).zone != zoneRect.type) continue;
                    GameObject go = new GameObject();
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = theme.GetRoomSprite(zoneRect.type);
                    sr.sortingOrder = 0;
                    go.transform.position = GridMap.instance.GridToWorld(x, y);
                }    
            }
        }
    }
}
