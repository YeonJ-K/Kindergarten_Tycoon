using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class BasicMapDataConverter : ExcelConverterBase
    {
        [MenuItem("Kindergarten/Convert Basic Map Data")]
        public static void Convert()
        {
            new BasicMapDataConverter().Run();
        }

        private void Run()
        {
            var sheet = OpenSheet("Assets/DataTable/Kindergarten_Tycoon.xlsx",
                "BasicMapData", out var colMap);
            if (sheet == null) return;

            var table = new MapMetaData.BasicMapDataJson { zones = new() };

            for (int r = 1; r < sheet.Rows.Count; r++)
            {
                DataRow row = sheet.Rows[r];
                if (string.IsNullOrWhiteSpace(Cell(row, colMap, "ZoneType"))) continue;

                try
                {
                    var zone = new MapZoneData
                    {
                        zone               = ParseZoneType(Cell(row, colMap, "ZoneType")),
                        hasDoor            = ParseBool(Cell(row, colMap, "HasDoor")),
                        door               = ParseVector2Int(Cell(row, colMap, "Door")),
                        startX             = ParseInt(Cell(row, colMap, "Start X")),
                        startY             = ParseInt(Cell(row, colMap, "Start Y")),
                        width              = ParseInt(Cell(row, colMap, "Width"), 1),
                        height             = ParseInt(Cell(row, colMap, "Height"), 1),
                        floorSpriteName    = Cell(row, colMap, "FloorSpriteName"),
                        hasActiveFurniture = ParseBool(Cell(row, colMap, "HasActiveFurniture")),
                        furnitureCount     = ParseInt(Cell(row, colMap, "FurnitureCount")),
                    };
                    table.zones.Add(zone);
                }
                catch (Exception e)
                {
                    Debug.LogError($"BasicMapData {r + 1}행 파싱 실패 (zone={Cell(row, colMap, "ZoneType")}): {e.Message}");
                }
            }

            WriteJson(table, "Assets/Resources/Data/BasicMapData.json");
        }
    }
}
