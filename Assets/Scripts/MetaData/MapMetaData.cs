using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    // BasicMapData 시트 1행 = 구역 하나의 정의
    [System.Serializable]
    public class MapZoneData
    {
        public ZoneType zone;
        public bool hasDoor;
        public Vector2Int door;
        public int startX;
        public int startY;
        public int width = 1;
        public int height = 1;
        public string floorSpriteName;      // 바닥 스프라이트 경로
        public bool hasActiveFurniture;     // 초기 Active 가구 배치 여부(추후 사용)
        public int furnitureCount;          // 초기 배치 가구 수(추후 사용)
    }

    public class MapMetaData
    {
        [System.Serializable]
        public class BasicMapDataJson
        {
            public List<MapZoneData> zones;
        }

        private List<MapZoneData> zones = new();
        public IReadOnlyList<MapZoneData> Zones => zones;

        // 존 범위에서 자동 계산한 맵 크기 (WallPaper 장식은 제외)
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public void Setting(string jsonText)
        {
            var data = JsonUtility.FromJson<BasicMapDataJson>(jsonText);
            if (data == null || data.zones == null) { Debug.LogError("맵 데이터 파싱 실패"); return; }

            zones = data.zones;
            RecalcSize();
        }

        private void RecalcSize()
        {
            int w = 0, h = 0;
            foreach (var z in zones)
            {
                if (z.zone == ZoneType.WallPaper) continue; // 장식은 크기 계산 제외
                w = Mathf.Max(w, z.startX + z.width);
                h = Mathf.Max(h, z.startY + z.height);
            }
            MapWidth = w;
            MapHeight = h;
        }
    }
}
