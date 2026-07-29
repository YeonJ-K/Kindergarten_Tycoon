using System;
using TMPro;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class LoadingUI : UIBase
    {
        public override UIType UIType { get { return UIType.LoadingUI; } }
        
        [SerializeField] TextMeshProUGUI loadingText;

        public void CloseLoading(Action action = null)
        {
            
        }
    }
}
