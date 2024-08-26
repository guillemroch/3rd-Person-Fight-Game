using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderBarStormlight : MonoBehaviour{
     [SerializeField]private Slider _stormlightSlider;
     [SerializeField]private Slider _breathedSlider;
      [SerializeField]private Slider _easeSlider;

    [SerializeField] private float _stormlight;
    [SerializeField] private float _stormlightHeld;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _lerpSpeed;

    private void Awake() {
    }

    public void Initialize(float startValue) {
        _stormlight = startValue;
        _stormlightHeld = startValue;
        _maxValue = startValue;
    }

    public void UpdateUI()
    {
        if (_stormlightSlider?.value != _stormlight) {
            _stormlightSlider.value = Mathf.Lerp(_stormlightSlider.value, _stormlight, _lerpSpeed);
        }

        if (_breathedSlider?.value != _stormlightHeld) {
            _breathedSlider.value = Mathf.Lerp(_breathedSlider.value, _stormlightHeld, _lerpSpeed);
            _easeSlider.value = Mathf.Lerp(_easeSlider.value, _stormlightHeld, _lerpSpeed /10f );
        }
    }


    public void Set(float stormlight, float breathedStormlight) {
        _stormlight = stormlight;
        _stormlightHeld = stormlight + breathedStormlight;
    }

    public void Up(float amount) {
        _stormlight += amount;
        if (_stormlight > _maxValue) _stormlight = _maxValue;
    }
}
