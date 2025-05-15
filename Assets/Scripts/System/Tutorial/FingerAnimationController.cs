using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fingerAnimationType
{
    None = 0,
    Touch = 1,
    DragUp = 2,
    DragDown = 3,
    DragRight = 4,
    DragLeft = 5,
}
[RequireComponent(typeof(Animator))]
public class FingerAnimationController : MonoBehaviour
{
    private static readonly int AnimationType = Animator.StringToHash("AnimationType");
    public fingerAnimationType fingerAnimationType = fingerAnimationType.None;
    
    private Animator animator;
    private int _animType;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(AnimationType,(int)fingerAnimationType);
    }

    private void Update()
    {
        if ((int)fingerAnimationType == animator.GetInteger(AnimationType))
        {
            return;
        }
        animator.SetInteger(AnimationType,(int)fingerAnimationType);
    }
}
