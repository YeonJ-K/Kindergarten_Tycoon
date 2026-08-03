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
        public List<SetFurnitureData> sets;
    }

    private Dictionary<int, ActiveFurnitureData> activeDict = new();
    private Dictionary<int, InActiveFurnitureData> inactiveDict = new();
    private Dictionary<int, SetFurnitureData> setDict = new();

    public void Setting(string jsonText)
    {
        var data = JsonUtility.FromJson<FurnitureTableJson>(jsonText);
        if (data == null) { Debug.LogError("가구 데이터 파싱 실패"); return; }

        activeDict.Clear();
        inactiveDict.Clear();
        setDict.Clear();
        foreach (var d in data.actives)   activeDict[d.id]   = d;
        foreach (var d in data.inactives) inactiveDict[d.id] = d;
        if (data.sets != null)
            foreach (var s in data.sets)  setDict[s.setId]   = s;
    }
    
    public List<ActiveFurnitureData> GetAllActives() => new List<ActiveFurnitureData>(activeDict.Values);
    public List<InActiveFurnitureData> GetAllInactives() => new List<InActiveFurnitureData>(inactiveDict.Values);
    public ActiveFurnitureData GetActive(int id) => activeDict.TryGetValue(id, out var d) ? d : null;
    public InActiveFurnitureData GetInactive(int id) => inactiveDict.TryGetValue(id, out var d) ? d : null;

    // 세트 정의 반환 (없으면 null)
    public SetFurnitureData GetSet(int setId) => setDict.TryGetValue(setId, out var s) ? s : null;

    // 세트 구성 가구의 이름 목록 반환 (setId 0이거나 미정의면 빈 목록)
    public List<string> GetSetMemberNames(int setId)
    {
        var names = new List<string>();
        var set = GetSet(setId);
        if (set == null) return names;
        foreach (var id in set.memberIds)
            if (inactiveDict.TryGetValue(id, out var d)) names.Add(d.furnitureName);
        return names;
    }
}
