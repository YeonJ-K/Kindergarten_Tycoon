using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class FurnitureItem : MonoBehaviour
    {
        [SerializeField] private Image furnitureImage;
        [SerializeField] private TextMeshProUGUI itemPriceTxt;

        public void ItemSetting(string path, int itemPrice)
        {
            furnitureImage.sprite = Resources.Load<Sprite>(path);
            itemPriceTxt.text = itemPrice.ToString();
        }
    }
}