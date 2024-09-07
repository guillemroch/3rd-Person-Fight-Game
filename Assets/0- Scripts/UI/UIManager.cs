using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour{

    [Header("References")] [SerializeField]
    private PlayerStateMachine psm;
    
    [Header("Health")]
    [SerializeField] private UISliderBarHealth _healthBar;
    [SerializeField] private UISliderBarStormlight _stormlightBar;
    
    [Header("Menu and Inputs")]
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private bool _isPaused;
    [SerializeField] private GameObject _configurationWindow;

    [Header("Input helper")] 
    [SerializeField] private InputsUIHelper _inputs;
    [SerializeField] private bool _showInputs;
    public bool ShowInputs { get => _showInputs; set => _showInputs = value; }

    [Header("Infusing Modes")] 
    [SerializeField] private InfuseModesUI _modes;

    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _nextLevelPanel;
    public UISliderBarHealth HealthBar { get => _healthBar; set => _healthBar = value; }
    public UISliderBarStormlight StormlightBar { get => _stormlightBar; set => _stormlightBar = value; }
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    public void Initialize(PlayerStateMachine psm) {
        _stormlightBar.Initialize(psm.Stormlight);
        _healthBar.Initialize(psm.Health);
    }

    public void Update() {
        _inputs.gameObject.SetActive(_showInputs); 
        _stormlightBar.Set(psm.Stormlight, psm.BreathedStormlight);
        _healthBar.Set(psm.Health);
        _healthBar.UpdateUI();
        _stormlightBar.UpdateUI();

        if (_inputManager.ConfigurationInput) {
            _inputManager.ResetConfigurationInput();
            TogglePause();
        }
        
        if (_showInputs)
            _inputs.UpdateKeys();
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

    public void ToggleConfigurationWindow() {
       
       _configurationWindow.SetActive(!_configurationWindow.activeSelf); 
    }

    public void ExitButton() {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void SetKeyStates(InputsUIHelper.KeyUIStates state) {
        _inputs.UIState = state;
    }

    public void SwitchInfuseModes(Infusable.InfusingMode mode) {
        _modes.SwitchMode(mode);
    }

    public void ToggleShowInput() {
        _inputs.gameObject.SetActive(!_inputs.gameObject.activeSelf);
    }

    public void GoBackToMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void ResetLevel() {
        psm.ResetLevel();
    }

    public void OpenDeadPanel() {
        _deathPanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    
      public void OpenNextLevelPanel() {
            _nextLevelPanel.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
}
