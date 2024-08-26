using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderBarHealth : MonoBehaviour{
     [SerializeField]private Slider _healthSlider;
     [SerializeField]private Slider _easeSlider;

    [SerializeField] private float _health;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _lerpSpeed;

    private void Awake() {
    }

    public void Initialize(float startValue) {
        _health = startValue;
        _maxValue = startValue;
    }

    public void UpdateUI()
    {

        if (_healthSlider?.value != _health) {
            _healthSlider.value = Mathf.Lerp(_healthSlider.value, _health, _lerpSpeed);
            _easeSlider.value = Mathf.Lerp(_easeSlider.value, _health, _lerpSpeed /10f );
        }
    }


    public void Set(float health) {
        _health = health;
    }

  
}
