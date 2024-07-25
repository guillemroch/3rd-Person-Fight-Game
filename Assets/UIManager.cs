using System;
using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class UIManager : MonoBehaviour{
    [Header("Health")]
    [SerializeField] private UISliderBar _healthBar;
    [SerializeField] private UISliderBar _stormlightBar;

    public UISliderBar HealthBar { get => _healthBar; set => _healthBar = value; }
    public UISliderBar StormlightBar { get => _stormlightBar; set => _stormlightBar = value; }


    public void Initialize(PlayerStateMachine psm) {
        _stormlightBar.Initialize(psm.Stormlight);
    }

    public void UpdateUI() {
        _healthBar.UpdateUI();
        _stormlightBar.UpdateUI();
    }
    
}
