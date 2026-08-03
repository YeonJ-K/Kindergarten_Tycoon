using TMPro;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class POPUP_ErrorMessage : UIBase
    {
        public override UIType UIType { get { return UIType.ErrorMessage; } }
        
        [SerializeField] TextMeshProUGUI errorMessage;
        
        public void SetMessage(string msg)
        {
            errorMessage.text = msg;
        }
    }
}
