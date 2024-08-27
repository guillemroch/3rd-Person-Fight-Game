using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour{
    [SerializeField] private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetVariable(string variable, bool value) {
        _animator.SetBool(variable, value);
    }

    public void ResetJump() {
        _animator.SetBool("IsJumping", true);
        
    }

    public void ResetFall() {
        _animator.SetBool("End", true);
    }
}
