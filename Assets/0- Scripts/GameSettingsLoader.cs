using System.Collections;
using System.Collections.Generic;
using System.IO;
using Player.StateMachine;
using UnityEngine;

public class GameSettingsLoader : MonoBehaviour{

    [SerializeField] private GameSettings _settings;
    [SerializeField] private string _settingsFilePath;
    
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private CameraManager _camera;
    [SerializeField] private UISettingsManager _uiSettingsManager;
    void Awake() {
        _settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        UpdateSettings();
    }

    public void UpdateSettings() {
        _settings = LoadSettings();
        UpdateComponentsValues();
    }

    public void UpdateComponentsValues() {
        if (_camera != null) {
            _uiManager.ShowInputs = _settings.ShowUIControls;
            _camera.InvertedPitch = _settings.InvertPitch;
            _camera.InvertedYaw = _settings.InvertYaw;
            _camera.GeneralSensitivityMultiplier = _settings.CameraSensitivity;

            
        }
        _uiSettingsManager.UpdateAllComponentsOnLoad(_settings);
    }
    
    public GameSettings LoadSettings() {
        if (File.Exists(_settingsFilePath)) {
            string json = File.ReadAllText(_settingsFilePath); // Read file
            return JsonUtility.FromJson<GameSettings>(json); // Convert JSON to object
        }

        return new GameSettings(); 
    }
}
