using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.StateMachine;
using UnityEngine;

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

    [Header("Infusing Modes")] 
    [SerializeField] private InfuseModesUI _modes;
    
    public UISliderBarHealth HealthBar { get => _healthBar; set => _healthBar = value; }
    public UISliderBarStormlight StormlightBar { get => _stormlightBar; set => _stormlightBar = value; }
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    public void Initialize(PlayerStateMachine psm) {
        _stormlightBar.Initialize(psm.Stormlight);
        _healthBar.Initialize(psm.Health);
    }

    public void Update() {
        
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

    public void OpenConfigurationWindow() {
        
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
}
