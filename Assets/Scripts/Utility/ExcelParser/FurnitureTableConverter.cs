using System.Collections.Generic;
using System.Data;
using System;
using UnityEditor;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class FurnitureTableConverter : ExcelConverterBase
    {
        [MenuItem("Kindergarten/Convert Furniture Table")]
        public static void Convert()
        {
            new FurnitureTableConverter().Run();
        }

        private void Run()
        {
            var sheet = OpenSheet("Assets/DataTable/Kindergarten_Tycoon.xlsx",
                "FurnitureTable", out var colMap);
            if (sheet == null) return;

            var table = new FurnitureMetaData.FurnitureTableJson { actives = new(), inactives = new() };

            for (int r = 1; r < sheet.Rows.Count; r++)
            {
                DataRow row = sheet.Rows[r];
                if (string.IsNullOrWhiteSpace(Cell(row, colMap, "id"))) continue;

                try
                {
                    if (ParseInt(Cell(row, colMap, "type")) == 0)
                        table.actives.Add(ParseActive(row, colMap));
                    else
                        table.inactives.Add(ParseInactive(row, colMap));
                }
                catch (Exception e)
                {
                    Debug.LogError($"{r + 1}행 파싱 실패 (id={Cell(row, colMap, "id")}): {e.Message}");
                }

            }

            WriteJson(table, "Assets/Resources/Data/FurnitureTable.json");
        }

        private void FillCommon(FurnitureData data, DataRow row, Dictionary<string, int> colMap)
        {
            data.id = ParseInt(Cell(row, colMap, "id"));
            data.type = ParseInt(Cell(row, colMap, "type")) == 0
                ? FurnitureType.Active
                : FurnitureType.InActive;
            data.furnitureName = Cell(row, colMap, "furnitureName");
            data.spritePath = Cell(row, colMap, "spritePath");
            data.width = ParseInt(Cell(row, colMap, "width"), 1);
            data.height = ParseInt(Cell(row, colMap, "height"), 1);
            data.price = ParseInt(Cell(row, colMap, "price"));
            data.spritePlacePos = ParseVector2Int(Cell(row, colMap, "spritePlacePos"));
        }
        
        private ActiveFurnitureData ParseActive(DataRow row, Dictionary<string,int> colMap)
        {
            var data = new ActiveFurnitureData();
            FillCommon(data, row, colMap);

            data.processNeed       = ParseNeedType(Cell(row, colMap, "processNeed"));
            data.availableKidCount = ParseInt(Cell(row, colMap, "availableKidCount"), 1);
            data.usingSec          = ParseFloat(Cell(row, colMap, "usingSec"));

            AddUsingPos(data.usingPos, Cell(row, colMap, "usingPos01"));
            AddUsingPos(data.usingPos, Cell(row, colMap, "usingPos02"));
            AddUsingPos(data.usingPos, Cell(row, colMap, "usingPos03"));

            return data;
        }
        
        private InActiveFurnitureData ParseInactive(DataRow row, Dictionary<string,int> colMap)
        {
            var data = new InActiveFurnitureData();
            FillCommon(data, row, colMap);

            data.satisfaction  = ParseInt(Cell(row, colMap, "satisfaction"));
            data.satisfiedZone = ParseZoneType(Cell(row, colMap, "satisfiedZone"));
            data.setId         = ParseInt(Cell(row, colMap, "setId"));

            return data;
        }
        
        private void AddUsingPos(List<Vector2Int> list, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return;
            list.Add(ParseVector2Int(raw));
        }
    }
}
