using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.Play(0, -1, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
