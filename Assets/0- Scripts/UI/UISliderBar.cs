using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderBar : MonoBehaviour{
    private Slider _sliderComponent;

    [SerializeField] private float _value;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _lerpSpeed;

    private void Awake() {
        _sliderComponent = GetComponent<Slider>();
    }

    public void Initialize(float startValue) {
        _value = startValue;
        _maxValue = startValue;
    }

    public void UpdateUI()
    {
        if (_sliderComponent?.value != _value) {
            _sliderComponent.value = Mathf.Lerp(_sliderComponent.value, _value, _lerpSpeed);
        }
    }


    public void Set(float amount) {
        _value = amount;
    }

    public void Up(float amount) {
        _value += amount;
        if (_value > _maxValue) _value = _maxValue;
    }
}
