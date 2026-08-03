using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class FurnitureItem : MonoBehaviour
    {
        private FurnitureType thisType;
        [SerializeField] private Image furnitureImage;
        [SerializeField] private TextMeshProUGUI itemPriceTxt;
        private ActiveFurnitureData activeData;
        private InActiveFurnitureData inActiveData;
        private bool isInventory;

        public void ItemSetting(FurnitureData data, bool isInventory = false)
        {
            this.isInventory = isInventory;

            if (data is ActiveFurnitureData active)
            {
                activeData = active;
                inActiveData = null;
                thisType = FurnitureType.Active;
            }
            else if (data is InActiveFurnitureData inactive)
            {
                inActiveData = inactive;
                activeData = null;
                thisType = FurnitureType.InActive;
            }

            furnitureImage.sprite = Resources.Load<Sprite>(data.spritePath);
            itemPriceTxt.text = data.price.ToString();
        }

        public void ClickItem()
        {
            if (isInventory)
            {
                // 보유 가구는 구매가 아니라 배치 모드로 진입
                // TODO: 보유 가구를 실제 Furniture로 생성 후 InGameCore.EDITOR.BeginPlace(furniture)
                return;
            }

            if (thisType == FurnitureType.Active)
            {
                var furniturePopup = GameCore.UI.OpenUI<POPUP_BuyActiveFurniture>(UIType.BuyActiveFurnitureUI);
                furniturePopup.SettingFurniture(activeData.furnitureName, activeData.price, activeData.availableKidCount, activeData.usingSec, activeData.spritePath, activeData.processNeed);
            }


            else
            {
                var furniturePopup = GameCore.UI.OpenUI<POPUP_BuyInActiveFurniture>(UIType.BuyAInActiveFurnitureUI);
                furniturePopup.SettingFurniture(inActiveData.furnitureName, inActiveData.price, inActiveData.satisfaction, inActiveData.spritePath, inActiveData.satisfiedZone);
                if (inActiveData.setId != 0)
                {
                    var set = GameCore.DATA.Furniture.GetSet(inActiveData.setId);
                    if (set != null)
                    {
                        var setNames = GameCore.DATA.Furniture.GetSetMemberNames(inActiveData.setId);
                        furniturePopup.SettingSetFurniture(setNames, set.setEffectSatisfaction);
                    }
                }
            }

            
        }
    }
}