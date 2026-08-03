using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class POPUP_BuyActiveFurniture : UIBase
    {
        public override UIType UIType { get { return UIType.BuyActiveFurnitureUI;  } }

        [SerializeField] private TextMeshProUGUI furnitureNameTxt;
        [SerializeField] private TextMeshProUGUI furniturePriceTxt;
        [SerializeField] private TextMeshProUGUI furnitureLimitPeopleTxt;
        [SerializeField] private TextMeshProUGUI furnitureLimitTimeTxt;
        [SerializeField] private TextMeshProUGUI furnitureZoneTxt;
        [SerializeField] private Image FurnitureImage;
        

        public void SettingFurniture(string name, int price, int limitPeople, float limitTime, string spritePath, NeedType needs)
        {
            furnitureNameTxt.text = name;
            furniturePriceTxt.text = price.ToString();
            furnitureLimitPeopleTxt.text = limitPeople.ToString();
            furnitureLimitTimeTxt.text = limitTime.ToString();
            FurnitureImage.sprite = Resources.Load<Sprite>(spritePath);
            switch (needs)
            {
                case NeedType.Hunger:
                    furnitureZoneTxt.text = "식당";
                    break;
                case NeedType.Toilet:
                    furnitureZoneTxt.text = "화장실";
                    break;
                case NeedType.Sleep:
                    furnitureZoneTxt.text = "수면실";
                    break;
            }
            
        }

        public void ClickBuyFurniture()
        {
            
        }

        public void CancelBuyFurniture()
        {
            CloseUI();
        }
    }
}
