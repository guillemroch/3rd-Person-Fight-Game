using System;
using System.Collections;
using System.Collections.Generic;
using Player.StateMachine;
using UnityEngine;

public class ParticleDirectionControl : MonoBehaviour{
    public PlayerStateMachine playerStateMachine;
    public ParticleSystem particleSystem;

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update() {
        transform.rotation = Quaternion.identity; 

    }
}
