using System;
using System.Collections.Generic;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class ActiveFurniture : Furniture
    {
        // ---- Param
        [SerializeField] private GameObject usingKidsProfile;

        private List<Vector2Int> usingPos;
        private NeedType processNeed;
        private int availableKidCount;
        private float usingSec;

        private KidAgent[] slots; // 사용 위치를 누가쓰는지.
        public override void Init(FurnitureData data, Vector2Int anchor)
        {
            base.Init(data, anchor);

            if (!(data is ActiveFurnitureData activeData))
            {
                // ActiveData가 아님을 로깅
                return;
            }

            usingPos = new List<Vector2Int>(activeData.usingPos);
            processNeed = activeData.processNeed;
            usingSec = activeData.usingSec;

            availableKidCount = Mathf.Min(activeData.availableKidCount, usingPos.Count);
            slots = new KidAgent[usingPos.Count];

            if (usingKidsProfile != null)
                usingKidsProfile.SetActive(false);
        }
        
        // ---- Set
        public void SetUsingKidProfile(KidAgent kid)
        {
            if (usingKidsProfile == null) return;
 
            usingKidsProfile.SetActive(true);
 
            UseActiveProfile profile = usingKidsProfile.GetComponent<UseActiveProfile>();
            if (profile != null)
                profile.SettingProfile(kid.name);
        }


        // ---- Get
        public List<Vector2Int> GetUseAbsolutePos()
        {
            List<Vector2Int> absolutePos = new List<Vector2Int>();
            for (int i = 0; i < usingPos.Count; i++)
                absolutePos.Add(placedCell + usingPos[i]);
 
            return absolutePos;
        }
 
        public List<Vector2Int> GetUsingPos() => usingPos;
        public NeedType GetFurnitureNeedsType() => processNeed;
        public int GetAvailableKidCount() => availableKidCount;
        public float GetUsingSec() => usingSec;
        
        // ---- Main
        public bool TryOccupy(KidAgent kid, out Vector2Int usePosAbsolute)
        {
            usePosAbsolute = default;
            if (slots == null || kid == null) return false;
 
            for (int i = 0; i < slots.Length && i < availableKidCount; i++)
            {
                if (slots[i] != null) continue;
 
                slots[i] = kid;
                usePosAbsolute = placedCell + usingPos[i];
                return true;
            }
            return false;   // 자리 없음
        }

        public void Release(KidAgent kid)
        {
            if (slots == null) return;
 
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != kid) continue;
 
                slots[i] = null;
                break;
            }
 
            if (GetUsingKidCount() == 0 && usingKidsProfile != null)
                usingKidsProfile.SetActive(false);
        }
 
        public bool CanUseActiveFurniture() => GetUsingKidCount() < availableKidCount;
 
        public int GetUsingKidCount()
        {
            if (slots == null) return 0;
 
            int count = 0;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] != null) count++;
 
            return count;
        }
    }
}