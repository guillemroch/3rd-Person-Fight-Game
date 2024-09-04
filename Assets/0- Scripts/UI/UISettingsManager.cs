using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : MonoBehaviour{

    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Slider _cameraSensitivitySlider;
    [SerializeField] private Toggle _cameraPitchToggle;
    [SerializeField] private Toggle _cameraYawToggle;
    [SerializeField] private TextMeshProUGUI _cameraSensitivityValue;


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gameObject.SetActive(false);
        }
    }

    public void CameraSensitibityChanged() {
        _cameraSensitivityValue.SetText((Mathf.Round(_cameraSensitivitySlider.value * 100)/100f).ToString());
        _cameraManager.GeneralSensitivityMultiplier = _cameraSensitivitySlider.value;
    }

    public void ToggleYawInverse() {
        _cameraManager.InvertedYaw = _cameraYawToggle.isOn;
    }

    public void TogglePitchInverse() {
        
        _cameraManager.InvertedPitch = _cameraPitchToggle.isOn;
    }
} 
