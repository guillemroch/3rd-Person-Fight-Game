using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infusable : MonoBehaviour , Interactable{

    public enum InfusingMode{
        Basic,
        Full,
        Inverse
    }
   
    [Header("Interaction")] 
    [SerializeField] private bool _active = false;

    [SerializeField] private InfusingMode _infusingMode;
    
    [Header("Full Lashing")] 
    [SerializeField] private float _weight = 2f;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;
    [SerializeField] private float _lashForce = 1;
   
    [Header("Object movement")]
    [SerializeField] private float _distance = 2f;
    [SerializeField] private float _smoothTime = 50f;
    [SerializeField] private float _maxSpeed = 20f;
    
    [Header("Stormlight")]
    [SerializeField] private float _chargedStormlight = 100;
    [SerializeField] private float _stormlightCost = 1f;
    [SerializeField] private float _stormlightBaseCost = 1f;
    [SerializeField] private float _stormlightLashCost = 3f;
    
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject _selectedOutline;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private GameObject _inRangeOutline;
    [SerializeField] private Transform _playerTransform;
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }

    public void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _playerRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        _cameraTransform = Camera.main.transform;
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void Interact(out int value) {
        value = 1;
    }
    public void BasicLash() {
        
    }

    public void InverseLash() {
        
    }
    public void FullLash(out int value) {
        _active = true;
        value = (int) _stormlightCost;
        _gravityDirection = _playerRigidbody.velocity + Vector3.up*_rigidbody.mass;
        _stormlightCost = _stormlightBaseCost;

    }

   public void Release() {
        
        Debug.Log("Released!");
        _active = false;
        _gravityDirection = _cameraTransform.forward * (10 * _lashForce);
        //_chargedStormlight = 100;
   }

    public void Update() {
        
        
        if (_active) {
            if (_infusingMode == InfusingMode.Inverse) return;
            
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();
            
            _selectedOutline.SetActive(true);
            _inRangeOutline.SetActive(false);
            
            //Object orbiting
            Vector3 offset =  _cameraTransform.forward * (Vector3.Distance(_playerTransform.position, _cameraTransform.position) * _distance);
            Vector3 velocity = _rigidbody.velocity;
            transform.position = Vector3.SmoothDamp(transform.position, _cameraTransform.position + offset, ref velocity, _smoothTime);
            _rigidbody.velocity = velocity;
            _gravityDirection = Vector3.down * 10;
            
            
            
        }
        else {
            _selectedOutline.SetActive(false);
            _rigidbody.AddForce(_gravityDirection);
            _chargedStormlight -= 0.1f;
            if (_chargedStormlight <= 0) {
                _particleSystem.Stop();
                _gravityDirection = Vector3.down * 10;
            }
        }
    }

    public void AddLash() {
        _chargedStormlight += 200;
        Debug.Log("aDD Lash");
        _lashForce++;
        _stormlightCost = _stormlightLashCost;

        _selectedOutline.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

    }

    public void UnLash() {
        _chargedStormlight -= _chargedStormlight > 100 ? 100 : 0;
        _lashForce -= _lashForce > 0 ? 1 : 0;
        _stormlightCost = -_stormlightLashCost;
        
        _selectedOutline.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
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

    private void OnCollisionStay(Collision other) {
        if (_active) return;
        // Debug.Log("Object stayed in collision: " + other.gameObject.name); 

    }
}
