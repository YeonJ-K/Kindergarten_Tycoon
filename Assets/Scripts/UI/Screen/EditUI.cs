using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class EditUI : UIBase
    {
        public override UIType UIType { get { return UIType.EditUI; } }

        [SerializeField] private Toggle activeFurnitureList;
        [SerializeField] private Toggle inactiveFurnitureList;
        [SerializeField] private Toggle inventory;

        [Header("Item")]
        [SerializeField] private FurnitureItem itemPrefab;
        [SerializeField] private Transform itemContent;   // 스크롤 Content (아이템 부모)

        private readonly List<FurnitureItem> spawnedItems = new List<FurnitureItem>();

        protected override void OnAwake()
        {
            base.OnAwake();

            activeFurnitureList.onValueChanged.AddListener(on => { if (on) ShowActive(); });
            inactiveFurnitureList.onValueChanged.AddListener(on => { if (on) ShowInactive(); });
            inventory.onValueChanged.AddListener(on => { if (on) ShowInventory(); });
        }

        public override void OpenUI()
        {
            base.OpenUI();

            // 기본 탭: Active (이미 켜져 있으면 리스너가 안 불리므로 직접 호출)
            if (activeFurnitureList.isOn) ShowActive();
            else activeFurnitureList.isOn = true;
        }

        // ----- Tabs
        private void ShowActive()
        {
            var list = new List<FurnitureData>();
            foreach (var d in GameCore.DATA.Furniture.GetAllActives()) list.Add(d);
            Rebuild(list, false);
        }

        private void ShowInactive()
        {
            var list = new List<FurnitureData>();
            foreach (var d in GameCore.DATA.Furniture.GetAllInactives()) list.Add(d);
            Rebuild(list, false);
        }

        private void ShowInventory()
        {
            var list = new List<FurnitureData>();
            foreach (var id in GameCore.DATA.OwnedFurnitureIds)
            {
                FurnitureData d = GameCore.DATA.Furniture.GetActive(id)
                                  ?? (FurnitureData)GameCore.DATA.Furniture.GetInactive(id);
                if (d != null) list.Add(d);
            }
            Rebuild(list, true);
        }

        // ----- Instantiate
        private void Rebuild(List<FurnitureData> dataList, bool isInventory)
        {
            ClearItems();
            foreach (var data in dataList)
            {
                FurnitureItem item = Instantiate(itemPrefab, itemContent, false);
                item.ItemSetting(data, isInventory);
                spawnedItems.Add(item);
            }
        }

        private void ClearItems()
        {
            for (int i = 0; i < spawnedItems.Count; i++)
                if (spawnedItems[i] != null) Destroy(spawnedItems[i].gameObject);
            spawnedItems.Clear();
        }
    }
}
