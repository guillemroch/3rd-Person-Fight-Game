using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class GameManager : MonoBehaviour{
    [SerializeField] private Collider _startGate;
    [SerializeField] private GameObject _endGate;
    [SerializeField] private bool _active = false;
    [SerializeField] private float _timer;

    [SerializeField] private TextMeshProUGUI _timerUI;
    void Start() {
        _timer = 0;
    }

    void Update() {
        
        _timerUI.SetText(((int)_timer).ToString());
        if (_active){ _timer += Time.deltaTime;}
    }


    public void StartTimer() {
        _active = true;
    }


    public void EndTimer() {
        _active = false;
    }
}
