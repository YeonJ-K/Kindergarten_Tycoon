using UnityEngine;
using UnityEngine.UI;


namespace YEONJI.Kindergarten
{
    public class FurnitureEdit : UIBase
    {
        public override UIType UIType { get { return  UIType.FurnitureEditUI; } }
        public RectTransform buttonGroup;

        public Button checkButton;
        public Button cancelButton;

    }
}