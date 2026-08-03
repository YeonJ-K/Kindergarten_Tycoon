using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YEONJI.Kindergarten
{
    public class MainUI : UIBase
    {
        public override UIType UIType { get { return UIType.MainUI; } }
        [SerializeField] private Image playerProfileImage;
        [SerializeField] private TextMeshProUGUI playerNameTxt;
        [SerializeField] private TextMeshProUGUI gameDateTxt;
        [SerializeField] private TextMeshProUGUI playerTotalMoneyTxt;

        public void SettingPlayerProfileBox(UserSex userS, string playerName, int gameDate, int playerMoney)
        {
            switch (userS)
            {
                case UserSex.Male:
                    playerProfileImage.sprite = Resources.Load<Sprite>("Sprites/Player/01. Profile/m_Player_profile_noBG");
                    break;
                case UserSex.Female:
                    playerProfileImage.sprite = Resources.Load<Sprite>("Sprites/Player/01. Profile/w_Player_profile_noBG");
                    break;
            }
            
            playerNameTxt.text = playerName;
            gameDateTxt.text = "Day  " + gameDate.ToString();
            playerTotalMoneyTxt.text = playerMoney.ToString();
        }

        public void ClickEditButton()
        {
            GameCore.UI.OpenUI<EditUI>(UIType.EditUI);
            CloseUI();
        }

        public void ClickStartButton()
        {
            InGameCore.ROUND.StartRound();
            CloseUI();
        }
    }
}
