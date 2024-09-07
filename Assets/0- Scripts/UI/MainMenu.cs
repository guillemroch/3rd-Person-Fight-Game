using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private GameObject _settingsMenu;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() { }

    public void PlayButtonPressed() {
        _loadingScreen.LoadLevel(1);
        
       
    }

    public void SettingButtonPressed() {
        
        _settingsMenu.SetActive(true);
    }

    public void ExitButtonPressed() {
        Application.Quit();
    }
}
