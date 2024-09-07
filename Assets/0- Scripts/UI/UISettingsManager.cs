using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : MonoBehaviour{

    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Slider _cameraSensitivitySlider;
    [SerializeField] private Toggle _cameraPitchToggle;
    [SerializeField] private Toggle _cameraYawToggle;
    [SerializeField] private Toggle _uiControlToggle;
    [SerializeField] private TextMeshProUGUI _cameraSensitivityValue;
    [SerializeField] private GameSettingsLoader _loader;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameSettings _settings;
    [SerializeField] private string _settingsFilePath ;

    private void Start() {
       _settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
       _settings = new GameSettings();

       if (_cameraManager != null) {
           _cameraSensitivitySlider.value = _cameraManager.GeneralSensitivityMultiplier;
           _cameraPitchToggle.isOn = _cameraManager.InvertedPitch;
           _cameraYawToggle.isOn = _cameraManager.InvertedYaw;
           _uiControlToggle.isOn = _uiManager.ShowInputs;
       }
     
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitSettingsMenu(); 
        }
        
  
    }

    public void UpdateAllComponentsOnLoad(GameSettings settings) {
          _cameraSensitivityValue.SetText((Mathf.Round(settings.CameraSensitivity * 100)/100f).ToString());
          _cameraSensitivitySlider.value =
              settings.CameraSensitivity;
          _cameraPitchToggle.isOn = settings.InvertPitch;
          _cameraYawToggle.isOn = settings.InvertYaw;
          _uiControlToggle.isOn = settings.ShowUIControls;
          if (_uiManager != null)
            _uiManager.ShowInputs = settings.ShowUIControls;
    }
    public void CameraSensitibityChanged() {
        _cameraSensitivityValue.SetText((Mathf.Round(_cameraSensitivitySlider.value * 100)/100f).ToString());
        //_cameraManager.GeneralSensitivityMultiplier = _cameraSensitivitySlider.value;

        _settings.CameraSensitivity = _cameraSensitivitySlider.value;
    }

    public void ToggleYawInverse() {
        //_cameraManager.InvertedYaw = _cameraYawToggle.isOn;

        _settings.InvertYaw = _cameraYawToggle.isOn;
    }

    public void TogglePitchInverse() {
        
        //_cameraManager.InvertedPitch = _cameraPitchToggle.isOn;

        _settings.InvertPitch = _cameraPitchToggle.isOn;
    }

    public void ToggleShowUIControls() {
        _settings.ShowUIControls = _uiControlToggle.isOn;
    }
 
    public void ExitSettingsMenu() {
        
        _settings.CameraSensitivity = _cameraSensitivitySlider.value;
        _settings.InvertYaw = _cameraYawToggle.isOn;
        _settings.InvertPitch = _cameraPitchToggle.isOn;
        _settings.ShowUIControls = _uiControlToggle.isOn;

        string json = JsonUtility.ToJson(_settings, true);
        File.WriteAllText(_settingsFilePath, json);
        
        gameObject.SetActive(false);
        _loader?.UpdateSettings();
        
    }
    
    
} 
