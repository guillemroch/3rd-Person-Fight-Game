using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputsUIHelper : MonoBehaviour
{
   public enum KeyUIStates{
      Grounded,
      Lashings,
      Infusing,
   }

   //TODO: Fix Inputs so that they are dinamic and not hardcoded
   [SerializeField] private KeyUIStates _uiState;
   [SerializeField] private GameObject _groundedKeys;
   [SerializeField] private GameObject _infusingKeys;
   [SerializeField] private GameObject _lashingsKeys;

   public KeyUIStates UIState { get => _uiState; set => _uiState = value; }

   public void UpdateKeys() {
      _groundedKeys.SetActive(false);
      _infusingKeys.SetActive(false);
      _lashingsKeys.SetActive(false);
      switch (_uiState) {
         case KeyUIStates.Grounded:
            _groundedKeys.SetActive(true);
            break;
         case KeyUIStates.Lashings:
            _lashingsKeys.SetActive(true);
            break;
         case KeyUIStates.Infusing:
            _infusingKeys.SetActive(true);
            break;
      }
   }
}
