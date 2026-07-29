using System.Collections.Generic;
using UnityEngine;
using YEONJI.Kindergarten;


public class FurnitureMetaData
{
    [System.Serializable]
    public class FurnitureTableJson
    {
        public List<ActiveFurnitureData> actives;
        public List<InActiveFurnitureData> inactives;    
    }

    private Dictionary<int, ActiveFurnitureData> activeDict = new();
    private Dictionary<int, InActiveFurnitureData> inactiveDict = new();

    public void Setting(string jsonText)
    {
        var data = JsonUtility.FromJson<FurnitureTableJson>(jsonText);
        if (data == null) { Debug.LogError("가구 데이터 파싱 실패"); return; }

        activeDict.Clear();
        inactiveDict.Clear();
        foreach (var d in data.actives)   activeDict[d.id]   = d;
        foreach (var d in data.inactives) inactiveDict[d.id] = d;
    }
    
    public ActiveFurnitureData GetActive(int id) => activeDict.TryGetValue(id, out var d) ? d : null;
    public InActiveFurnitureData GetInactive(int id) => inactiveDict.TryGetValue(id, out var d) ? d : null; 

}
