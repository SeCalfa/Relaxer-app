using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Logic.JsonReader;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Scripts
{
    [Serializable]
    public struct CategoryStruct
    {
        public Category category;
        public Image background;
        public TextMeshProUGUI text;
    }
    
    [Serializable]
    public struct NewTitlePageStruct
    {
        public Category category;
        public Image background;
        public TextMeshProUGUI text;
        public GameObject content;
        public string titleText;
    }
    
    public class RelaxApplication : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private GameObject homePage;
        [SerializeField] private GameObject userPage;
        [SerializeField] private GameObject categoriesPage;
        [SerializeField] private GameObject newTitlePage;
        [SerializeField] private GameObject selectedFilmPage;
        [SerializeField] private GameObject selectedSerialPage;

        [Header("Category page")]
        [SerializeField] private CategoryStruct[] categoryStruct;
        [Space]
        [SerializeField] private Sprite categoryActive;
        [SerializeField] private Sprite categoryInactive;
        [SerializeField] private Transform titlesParent;
        [Space]
        [SerializeField] private GameObject titleBox;

        [Header("New title page")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private NewTitlePageStruct[] newTitlePageStruct;
        [Space]
        [SerializeField] private Sprite newTitleActive;
        [SerializeField] private Sprite newTitleInactive;

        [Header("New film fields")]
        [SerializeField] private TMP_InputField newFilmName;
        [SerializeField] private TMP_InputField newFilmYear;
        
        [Header("New serial fields")]
        [SerializeField] private TMP_InputField newSerialName;
        [SerializeField] private TMP_InputField newSerialYear;
        [SerializeField] private TMP_InputField newSerialsSeasons;
        
        [Header("Selected film page")]
        [SerializeField] private TextMeshProUGUI selectedFilmTitle;

        private GameObject _activePage;
        private CategoryStruct _activeSection;
        private NewTitlePageStruct _activeNewTitle;
        private List<GameObject> _activeTitleBoxes = new();

        private JsonHandler _jsonHandler;
        private Account _account;
        
        private void Awake()
        {
            InitApplicationData();
            
            _activePage = homePage;
            _activeSection = categoryStruct[0];
            _activeNewTitle = newTitlePageStruct[1];

            InitApplicationView();
        }

        private void OnDestroy()
        {
            _jsonHandler.SaveToJson(_account);
        }

        private void InitApplicationData()
        {
            _jsonHandler = new JsonHandler();

            _account = _jsonHandler.AccountInit();
        }

        private void InitApplicationView()
        {
            // Open page on start
            CloseAllPages();
            homePage.SetActive(true);
            
            // Open section on start
            OpenSection(1);
            
            // Close all new title content
            foreach (var titlePageStruct in newTitlePageStruct)
            {
                titlePageStruct.content.SetActive(false);
            }
            ChooseNewTitleCategory((int)newTitlePageStruct[0].category);
            
            // Open category on start
            InitTitleBoxes(1);
        }

        private void CloseAllPages()
        {
            homePage.SetActive(false);
            userPage.SetActive(false);
            categoriesPage.SetActive(false);
            newTitlePage.SetActive(false);
            selectedFilmPage.SetActive(false);
            selectedSerialPage.SetActive(false);
        }

        private void ClearAllTitleBoxes()
        {
            foreach (var titleBox in _activeTitleBoxes)
            {
                Destroy(titleBox);
            }
            
            _activeTitleBoxes.Clear();
        }

        public void OpenPage(GameObject page)
        {
            _activePage.SetActive(false);
            
            page.SetActive(true);
            _activePage = page;
        }

        public void OpenSection(int category)
        {
            var categoryEnum = (Category)category;
            
            if (_activeSection.category == (Category)category)
            {
                return;
            }
            
            var categorySection = categoryStruct.First(p => p.category == categoryEnum);

            _activeSection.background.sprite = categoryInactive;
            _activeSection.text.color = Color.black;
            categorySection.background.sprite = categoryActive;
            categorySection.text.color = Color.white;
            
            _activeSection = categorySection;
        }

        public void UpdateSection()
        {
            foreach (var category in categoryStruct)
            {
                category.background.sprite = categoryInactive;
                category.text.color = Color.black;
            }
            
            _activeSection.background.sprite = categoryActive;
            _activeSection.text.color = Color.white;
        }

        public void ChooseNewTitleCategory(int category)
        {
            var categoryEnum = (Category)category;
            
            if (_activeNewTitle.category == categoryEnum)
            {
                return;
            }
            
            _activeNewTitle.content.SetActive(false);
            _activeNewTitle.text.color = Color.black;
            _activeNewTitle.background.sprite = newTitleInactive;
            
            var nextNewTitle = newTitlePageStruct.First(p => p.category == categoryEnum);
            
            nextNewTitle.background.sprite = newTitleActive;
            nextNewTitle.text.color = Color.white;
            title.text = nextNewTitle.titleText;

            _activeNewTitle = nextNewTitle;
            _activeNewTitle.content.SetActive(true);
            _activeSection = categoryStruct.First(p => p.category == categoryEnum);
        }

        public void AddFilm()
        {
            var filmName = newFilmName.text;
            var filmYear = int.Parse(newFilmYear.text);
            
            _account.films.Add(new Title
            {
                name = filmName,
                pictureId = 0,
                year = filmYear,
                titleStatus = TitleStatus.Planed
            });
        }
        
        public void AddSerial()
        {
            print("newSerialYear.text: " + newSerialYear.text);
            var serialName = newSerialName.text;
            var serialYear = int.Parse(newSerialYear.text);
            var serialSeasons = int.Parse(newSerialsSeasons.text);
            
            var serial = new Serial
            {
                name = serialName,
                pictureId = 0,
                year = serialYear,
                titleStatus = TitleStatus.Planed
            };

            for (var i = 0; i < serialSeasons; i++)
            {
                serial.Seasons.Add(i, false);
            }
            
            _account.serials.Add(serial);
        }

        public void InitTitleBoxes(int category)
        {
            ClearAllTitleBoxes();
            
            if (category == 1) // Films
            {
                _account.films.ForEach(film =>
                {
                    var currentTitleBox = Instantiate(titleBox, titlesParent);
                    currentTitleBox.GetComponent<TitleBox>().Init(this, film, selectedFilmPage, selectedSerialPage, TitleBoxType.Film);
                    
                    _activeTitleBoxes.Add(currentTitleBox);
                });
            }
            else if (category == 2) // Serials
            {
                _account.serials.ForEach(film =>
                {
                    var currentTitleBox = Instantiate(titleBox, titlesParent);
                    currentTitleBox.GetComponent<TitleBox>().Init(this, film, selectedFilmPage, selectedSerialPage, TitleBoxType.Serial);
                    
                    _activeTitleBoxes.Add(currentTitleBox);
                });
            }
        }

        public void ClearAllFields()
        {
            newFilmName.text = string.Empty;
            newFilmYear.text = string.Empty;
            
            newSerialName.text = string.Empty;
            newSerialYear.text = string.Empty;
            newSerialsSeasons.text = string.Empty;
        }

        public void FillSelectedFilmPage(string selectedFilmTitle)
        {
            this.selectedFilmTitle.text = selectedFilmTitle;
        }
    }
}
