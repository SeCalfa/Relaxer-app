using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class TitleBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleName;
        [SerializeField] private TextMeshProUGUI status;
        [SerializeField] private Image picture;

        private Title _title;

        public void Init(RelaxApplication app, Title title, GameObject selectedFilmPage, GameObject selectedSerialPage, TitleBoxType titleBoxType, Sprite picture)
        {
            _title = title;

            if (titleBoxType == TitleBoxType.Film)
            {
                GetComponent<Button>().onClick.AddListener(delegate
                {
                    app.OpenPage(selectedFilmPage);
                    app.FillSelectedFilmPage($"{_title.name} ({_title.year})", picture, title);
                });
            }
            else if (titleBoxType == TitleBoxType.Serial)
            {
                GetComponent<Button>().onClick.AddListener(delegate
                {
                    app.OpenPage(selectedSerialPage);
                    app.FillSelectedSerialPage($"{_title.name} ({_title.year})", picture, title);
                });
            }
            
            titleName.text = $"{_title.name} ({_title.year})";
            status.text = _title.titleStatus.ToString();

            this.picture.sprite = picture;
        }
    }
}
