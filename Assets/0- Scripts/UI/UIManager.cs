using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.StateMachine;
using UnityEngine;

public class UIManager : MonoBehaviour{
    [Header("Health")]
    [SerializeField] private UISliderBar _healthBar;
    [SerializeField] private UISliderBar _stormlightBar;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private bool _isPaused;
    [SerializeField] private GameObject _configurationWindow;
    public UISliderBar HealthBar { get => _healthBar; set => _healthBar = value; }
    public UISliderBar StormlightBar { get => _stormlightBar; set => _stormlightBar = value; }
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    public void Initialize(PlayerStateMachine psm) {
        _stormlightBar.Initialize(psm.Stormlight);
    }

    public void Update() {
        _healthBar.UpdateUI();
        _stormlightBar.UpdateUI();

        if (_inputManager.ConfigurationInput) {
            _inputManager.ResetConfigurationInput();
            TogglePause();
        }
    }

    public void TogglePause() {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0 : 1; 
        _pauseMenu.SetActive(_isPaused);
        
        if (_isPaused) 
            Cursor.lockState = CursorLockMode.None;
        else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OpenConfigurationWindow() {
        
    }

    public void ExitButton() {
        Debug.Log("Quit");
        Application.Quit();
    }
}
