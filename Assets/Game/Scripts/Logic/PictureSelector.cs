using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Logic
{
    public class PictureSelector : MonoBehaviour
    {
        [SerializeField]
        private Image picture;
        [SerializeField]
        private Sprite[] sprites;

        public int ActivePicture { get; private set; } = 0;

        public void ReplacePicture()
        {
            ActivePicture += 1;

            if (ActivePicture == sprites.Length)
            {
                ActivePicture = 0;
            }

            picture.sprite = sprites[ActivePicture];
        }
    }
}
