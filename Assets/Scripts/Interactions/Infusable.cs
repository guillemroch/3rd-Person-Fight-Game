using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infusable : MonoBehaviour , Interactable{
    [SerializeField] private float _weight = 2f;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _distance = 2f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;
    [SerializeField] private bool _active = false;
    [SerializeField] private float _outlineMaterialWidth = 1.1f;
    [SerializeField] private MeshRenderer _meshRenderer; 
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }

    public void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _playerRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Interact(out int value) {
        _active = true;
        value = (int)_weight;
        _gravityDirection = _playerRigidbody.velocity + Vector3.up*_rigidbody.mass;
    }

    public void Release() {
        
        Debug.Log("Released!");
        _active = false;
        _gravityDirection = _playerTransform.forward * 10;
    }

    public void Update() {
        
        
        if (_active) {
            transform.position = _playerTransform.position + _playerTransform.forward * 3f + _playerTransform.up * 2f;
            Vector3.SmoothDamp()
            _gravityDirection = Vector3.down * 10;
            _meshRenderer.sharedMaterials[0].SetFloat("_Width", _outlineMaterialWidth);
        }
        else {
            _rigidbody.AddForce(_gravityDirection);
             _meshRenderer.sharedMaterials[0].SetFloat("_Width", 0 );
        }
    }

    public void Overlay() {
    }

    public void OnDrawGizmos() {
        if (!_active)
            return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _gravityDirection);

    }
}
