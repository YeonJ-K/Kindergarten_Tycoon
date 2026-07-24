using UnityEngine;

namespace YEONJI.Kindergarten
{
    public class UseActiveProfile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer profile;
        private string profilePath = "Kids/Profiles/";

        public void SettingProfile(string kidName)
        {
            profile.sprite = Resources.Load<Sprite>(profilePath + kidName);
        }
    }
}