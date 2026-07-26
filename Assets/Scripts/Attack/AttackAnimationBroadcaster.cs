using System;
using UnityEngine;

/// <summary>
/// Add this StateMachineBehaviour to every attack state in the Animator.
/// Player.cs subscribes to onAttackAnimationFinished via GetBehaviours.
/// </summary>
public class AttackAnimationBroadcaster : StateMachineBehaviour
{
    public event Action onAttackAnimationFinished;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onAttackAnimationFinished?.Invoke();
    }
}
