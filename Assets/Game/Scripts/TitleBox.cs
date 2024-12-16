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

        public void Init(RelaxApplication app, Title title, GameObject selectedFilmPage, GameObject selectedSerialPage, TitleBoxType titleBoxType)
        {
            _title = title;

            if (titleBoxType == TitleBoxType.Film)
            {
                GetComponent<Button>().onClick.AddListener(delegate
                {
                    app.OpenPage(selectedFilmPage);
                    app.FillSelectedFilmPage($"{_title.name} ({_title.year})");
                });
            }
            else if (titleBoxType == TitleBoxType.Serial)
            {
                GetComponent<Button>().onClick.AddListener(delegate
                {
                    app.OpenPage(selectedSerialPage);
                });
            }
            
            titleName.text = $"{_title.name} ({_title.year})";
            status.text = _title.titleStatus.ToString();
        }
    }
}
