using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator = null;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsHit = Animator.StringToHash("IsHit");

    public void SetMoving(bool isOn)
    {
        this.animator.SetBool(IsMoving, isOn);
    }

    public void SetHit(bool isOn)
    {
        this.animator.SetBool(IsHit, isOn);
    }
}
