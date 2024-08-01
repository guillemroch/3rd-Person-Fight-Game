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
    [SerializeField] private GameObject _selectedOutline;
    [SerializeField] private GameObject _inRangeOutline;
    [SerializeField] private float _smoothTime = 50f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private Transform _cameraTransform;
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }

    public void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _playerRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;
    }

    public void Interact(out int value) {
        _active = true;
        value = (int)_weight;
        _gravityDirection = _playerRigidbody.velocity + Vector3.up*_rigidbody.mass;
    }

   public void Release() {
        
        Debug.Log("Released!");
        _active = false;
        _gravityDirection = _cameraTransform.forward * 10;
    }

    public void Update() {
        
        
        if (_active) {
            
            Vector3 offset =  _cameraTransform.forward * (Vector3.Distance(_playerTransform.position, _cameraTransform.position) * _distance);
            Vector3 velocity = _rigidbody.velocity;
            transform.position = Vector3.SmoothDamp(transform.position, _cameraTransform.position + offset, ref velocity, _smoothTime);
            _rigidbody.velocity = velocity;
            _gravityDirection = Vector3.down * 10;
            _selectedOutline.SetActive(true);
            _inRangeOutline.SetActive(false);
        }
        else {
            _selectedOutline.SetActive(false);
            _rigidbody.AddForce(_gravityDirection);
        }
    }

    public void ActivateOverlay() {
        _inRangeOutline.SetActive(true);
    }

    public void DeactivateOverlay() {
        _inRangeOutline.SetActive(false);
    }

    public void OnDrawGizmos() {
        if (!_active)
            return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _gravityDirection);

    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        ActivateOverlay();
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
        DeactivateOverlay();
    }
}
