using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _startAdventureButton;
    [SerializeField] private Button _joinAdventureButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private GameObject _mainPage;
    [SerializeField] private GameObject _adventurePage;
    [SerializeField] private GameObject _joinAdventurePage;
    [SerializeField] private GameObject _settingsPage;
    
    private GameObject _currentPage;

    

    private void Start() {
        _startAdventureButton.onClick.AddListener(() => ChangePage(_adventurePage));
        _joinAdventureButton.onClick.AddListener(() => ChangePage(_joinAdventurePage));
        _settingsButton.onClick.AddListener(() => ChangePage(_settingsPage));
        _quitButton.onClick.AddListener(Application.Quit);

        _currentPage = _mainPage;
    }

    void ChangePage(GameObject page) {
        _currentPage.SetActive(false);
        page.SetActive(true);
        _currentPage = page;
    }

    public void Back() {
        _currentPage.SetActive(false);
        _mainPage.SetActive(true);
        _currentPage = _mainPage;
    }
    
}