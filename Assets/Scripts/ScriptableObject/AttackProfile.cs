using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Data/Attack Profile")]
[Serializable]
public class AttackProfile : ScriptableObject
{
    public AnimationClip attack1;
    public AnimationClip attack2;
    public AnimationClip attack3;
    public AnimationClip comboAttack;
}
