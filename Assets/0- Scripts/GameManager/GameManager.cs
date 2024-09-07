using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    [SerializeField] private Collider _startGate;
    [SerializeField] private GameObject _endGate;
    [SerializeField] private bool _active = false;
    [SerializeField] private float _timer;
    [SerializeField] private UIManager _uiManager;

    [SerializeField] private TextMeshProUGUI _timerUI;
    void Start() {
        _timer = 0;
    }

    void Update() {
        
        _timerUI.SetText((Mathf.Round(_timer*100)/100).ToString());
        if (_active){ _timer += Time.deltaTime;}
    }


    public void StartTimer() {
        _active = true;
    }


    public void EndTimer() {
        _active = false;
        _uiManager.OpenNextLevelPanel();
    }

    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
