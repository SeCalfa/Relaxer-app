using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Logic;
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
        
        [Header("Home page")]
        [SerializeField] private TextMeshProUGUI homeName;
        [SerializeField] private Transform homeInProgressTitlesParent;

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
        [SerializeField] private PictureSelector newFilmPictureSelector;
        
        [Header("New serial fields")]
        [SerializeField] private TMP_InputField newSerialName;
        [SerializeField] private TMP_InputField newSerialYear;
        [SerializeField] private TMP_InputField newSerialsSeasons;
        [SerializeField] private PictureSelector newSerialPictureSelector;
        
        [Header("Selected film page")]
        [SerializeField] private TextMeshProUGUI selectedFilmTitle;
        [SerializeField] private Image selectedFilmLogo;
        [SerializeField] private GameObject selectedFilmPlanedButton;
        [SerializeField] private GameObject selectedFilmInProgressButton;
        [SerializeField] private GameObject selectedFilmFrozenButton;
        
        [Header("Selected serial page")]
        [SerializeField] private TextMeshProUGUI selectedSerialTitle;
        [SerializeField] private Image selectedSerialLogo;
        [SerializeField] private GameObject selectedSerialPlanedButton;
        [SerializeField] private GameObject selectedSerialInProgressButton;
        [SerializeField] private GameObject selectedSerialFrozenButton;

        [Space]
        [SerializeField] private Color statusButtonActiveColor;
        [SerializeField] private Color statusButtonInactiveColor;

        [Space]
        [SerializeField] private Sprite[] movieSprites;

        private GameObject _activePage;
        private CategoryStruct _activeSection;
        private NewTitlePageStruct _activeNewTitle;
        private List<GameObject> _activeTitleBoxes = new();
        private List<GameObject> _inProgressTitleBoxes = new();

        private JsonHandler _jsonHandler;
        private Account _account;
        private Title _selectedTitle;
        
        private void Awake()
        {
            InitApplicationData();
            
            _activePage = homePage;
            _activeSection = categoryStruct[0];
            _activeNewTitle = newTitlePageStruct[1];

            InitApplicationView();
            InitInProgressTitles();
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

            // Set name
            homeName.text = _account.name;
        }

        public void InitInProgressTitles()
        {
            ClearInProgressTitles();
            
            _account.films.Where(f => f.titleStatus == TitleStatus.InProgress).ToList().ForEach(film =>
            {
                var currentTitleBox = Instantiate(titleBox, homeInProgressTitlesParent);
                currentTitleBox.GetComponent<TitleBox>().Init(this, film, selectedFilmPage, selectedSerialPage, TitleBoxType.Film, movieSprites[film.pictureId]);
                    
                _inProgressTitleBoxes.Add(currentTitleBox);
            });
            
            _account.serials.Where(f => f.titleStatus == TitleStatus.InProgress).ToList().ForEach(serial =>
            {
                var currentTitleBox = Instantiate(titleBox, homeInProgressTitlesParent);
                currentTitleBox.GetComponent<TitleBox>().Init(this, serial, selectedFilmPage, selectedSerialPage, TitleBoxType.Serial, movieSprites[serial.pictureId]);
                    
                _inProgressTitleBoxes.Add(currentTitleBox);
            });
        }

        private void ClearInProgressTitles()
        {
            foreach (var titleBox in _inProgressTitleBoxes)
            {
                Destroy(titleBox);
            }
            
            _inProgressTitleBoxes.Clear();
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
                ID = Guid.NewGuid().ToString(),
                name = filmName,
                pictureId = newFilmPictureSelector.ActivePicture,
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
                ID = Guid.NewGuid().ToString(),
                name = serialName,
                pictureId = newSerialPictureSelector.ActivePicture,
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
                    currentTitleBox.GetComponent<TitleBox>().Init(this, film, selectedFilmPage, selectedSerialPage, TitleBoxType.Film, movieSprites[film.pictureId]);
                    
                    _activeTitleBoxes.Add(currentTitleBox);
                });
            }
            else if (category == 2) // Serials
            {
                _account.serials.ForEach(serial =>
                {
                    var currentTitleBox = Instantiate(titleBox, titlesParent);
                    currentTitleBox.GetComponent<TitleBox>().Init(this, serial, selectedFilmPage, selectedSerialPage, TitleBoxType.Serial, movieSprites[serial.pictureId]);
                    
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

        public void FillSelectedFilmPage(string selectedFilmTitle, Sprite picture, Title title)
        {
            this.selectedFilmTitle.text = selectedFilmTitle;
            _selectedTitle = title;

            selectedFilmLogo.sprite = picture;

            ResetButtonFilmStatus(title);
        }
        
        public void FillSelectedSerialPage(string selectedSerialTitle, Sprite picture, Title title)
        {
            this.selectedSerialTitle.text = selectedSerialTitle;
            _selectedTitle = title;

            selectedSerialLogo.sprite = picture;

            ResetButtonSerialStatus(title);
        }

        public void SetPlanedStatusFilm()
        {
            _selectedTitle.titleStatus = TitleStatus.Planed;
            ResetButtonFilmStatus(_selectedTitle);
        }

        public void SetInProgressStatusFilm()
        {
            _selectedTitle.titleStatus = TitleStatus.InProgress;
            ResetButtonFilmStatus(_selectedTitle);
        }

        public void SetFrozenStatusFilm()
        {
            _selectedTitle.titleStatus = TitleStatus.Frozen;
            ResetButtonFilmStatus(_selectedTitle);
        }
        
        public void SetPlanedStatusSerial()
        {
            _selectedTitle.titleStatus = TitleStatus.Planed;
            ResetButtonSerialStatus(_selectedTitle);
        }

        public void SetInProgressStatusSerial()
        {
            _selectedTitle.titleStatus = TitleStatus.InProgress;
            ResetButtonSerialStatus(_selectedTitle);
        }

        public void SetFrozenStatusSerial()
        {
            _selectedTitle.titleStatus = TitleStatus.Frozen;
            ResetButtonSerialStatus(_selectedTitle);
        }

        public void RemoveSelectedTitle()
        {
            var filmToRemove = _account.films.FirstOrDefault(f => f.ID == _selectedTitle.ID);
            var serialToRemove = _account.serials.FirstOrDefault(f => f.ID == _selectedTitle.ID);

            if (filmToRemove != null)
            {
                _account.films.Remove(filmToRemove);
                return;
            }
            
            if (serialToRemove != null)
            {
                _account.serials.Remove(serialToRemove);
            }
        }

        private void ResetButtonFilmStatus(Title title)
        {
            if (title.titleStatus == TitleStatus.Planed)
            {
                selectedFilmPlanedButton.GetComponent<Image>().color = statusButtonActiveColor;
                selectedFilmInProgressButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedFilmFrozenButton.GetComponent<Image>().color = statusButtonInactiveColor;
            }
            else if (title.titleStatus == TitleStatus.InProgress)
            {
                selectedFilmPlanedButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedFilmInProgressButton.GetComponent<Image>().color = statusButtonActiveColor;
                selectedFilmFrozenButton.GetComponent<Image>().color = statusButtonInactiveColor;
            }
            else if (title.titleStatus == TitleStatus.Frozen)
            {
                selectedFilmPlanedButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedFilmInProgressButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedFilmFrozenButton.GetComponent<Image>().color = statusButtonActiveColor;
            }
        }
        
        private void ResetButtonSerialStatus(Title title)
        {
            if (title.titleStatus == TitleStatus.Planed)
            {
                selectedSerialPlanedButton.GetComponent<Image>().color = statusButtonActiveColor;
                selectedSerialInProgressButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedSerialFrozenButton.GetComponent<Image>().color = statusButtonInactiveColor;
            }
            else if (title.titleStatus == TitleStatus.InProgress)
            {
                selectedSerialPlanedButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedSerialInProgressButton.GetComponent<Image>().color = statusButtonActiveColor;
                selectedSerialFrozenButton.GetComponent<Image>().color = statusButtonInactiveColor;
            }
            else if (title.titleStatus == TitleStatus.Frozen)
            {
                selectedSerialPlanedButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedSerialInProgressButton.GetComponent<Image>().color = statusButtonInactiveColor;
                selectedSerialFrozenButton.GetComponent<Image>().color = statusButtonActiveColor;
            }
        }
    }
}
