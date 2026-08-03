using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class POPUP_BuyFurniture : UIBase
    {
        public override UIType UIType { get { return UIType.BuyFurnitureUI;  } }

        [SerializeField] private TextMeshProUGUI furnitureNameTxt;
        [SerializeField] private TextMeshProUGUI furniturePriceTxt;
        [SerializeField] private TextMeshProUGUI furnitureSatisfactionTxt;
        [SerializeField] private TextMeshProUGUI furnitureZoneTxt;
        [SerializeField] private Image FurnitureImage;
        
        [Header("SET")]
        [SerializeField] private GameObject SETFurniture;
        [SerializeField] private TextMeshProUGUI SETsatisfactionTxt;
        [SerializeField] private TextMeshProUGUI SETFurnitureListTxt;

        public void SettingFurniture(string name, int price, int satisfaction, string spritePath, ZoneType zone)
        {
            furnitureNameTxt.text = name;
            furniturePriceTxt.text = price.ToString();
            furnitureSatisfactionTxt.text = satisfaction.ToString();
            FurnitureImage.sprite = Resources.Load<Sprite>(spritePath);
            switch (zone)
            {
                case ZoneType.None :
                    furnitureZoneTxt.text = "공용";
                    break;
                case ZoneType.Entrance:
                    furnitureZoneTxt.text = "현관";
                    break;
                case ZoneType.MainRoom:
                    furnitureZoneTxt.text = "메인룸";
                    break;
                case ZoneType.RestRoom:
                    furnitureZoneTxt.text = "화장실";
                    break;
                case ZoneType.PlayRoom:
                    furnitureZoneTxt.text = "놀이방";
                    break;
                case ZoneType.SleepRoom:
                    furnitureZoneTxt.text = "수면실";
                    break;
                case ZoneType.DiningRoom:
                    furnitureZoneTxt.text = "식당";
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
