using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour{
    [SerializeField] private float _health = 100;
    private Animator _animator;
    [SerializeField] private float _hitTimerDelay = 2f;
    private float _timer;
    
    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        _timer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out Weapon weapon)) {
            if (_timer > 0) return;
            _timer = _hitTimerDelay;
            _health -= weapon.Damage;
            if (_health > 0) {
                _animator.CrossFade("Hit", 0.1f);
            }
            else {
                _animator.CrossFade("Death", 0.1f);
            }
        } 
    }
}
