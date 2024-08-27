using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfuseModesUI : MonoBehaviour{
    [SerializeField] private GameObject _fullLash;
    [SerializeField] private GameObject _basicLash;
    [SerializeField] private GameObject _inverseLash;


    public void SwitchMode(Infusable.InfusingMode mode) {
        _fullLash.SetActive(false);
        _basicLash.SetActive(false);
        _inverseLash.SetActive(false);
        switch (mode) {
            case Infusable.InfusingMode.Basic:
                _basicLash.SetActive(true);
                break;
            case Infusable.InfusingMode.Full:
                _fullLash.SetActive(true);
                break;
            case Infusable.InfusingMode.Inverse :
                _inverseLash.SetActive(true);
                break;
        }
    }
}
