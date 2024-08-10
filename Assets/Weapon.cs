using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour{
    
    [SerializeField] private float _damage;
    public float Damage => _damage;
    
}
