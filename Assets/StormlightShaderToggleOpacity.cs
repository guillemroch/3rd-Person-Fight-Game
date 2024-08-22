using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormlightShaderToggleOpacity : MonoBehaviour{
  
 [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private MeshRenderer _shader;
    [SerializeField] private float _targetOpacity = 0;
    private void Awake() {
        _shader = GetComponent<MeshRenderer>();
    }

    void Update() {
        Material mat = _shader.material;
        //mat.FindPropertyIndex("_Opacity");
        //Debug.Log(mat.GetFloat("_Opacity"));
        mat.SetFloat("_Opacity", Mathf.Lerp(mat.GetFloat("_Opacity"), _targetOpacity,  smoothTime));
    }

    public void ToggleStormlight(float target) {
        _targetOpacity = target;
    }
}
