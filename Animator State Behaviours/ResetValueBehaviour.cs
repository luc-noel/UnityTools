using UnityEngine;
using PhantomCompass;
using System;

public class ResetValueBehaviour : StateMachineBehaviour
{
    [Tooltip("Int, Float or Bool")]
    public string parameterName;
    public bool resetOnEnter = false;
    public bool resetOnExit = false;
    [Tooltip("Float: value as in\nInt: returns the unrounded integer\nBool: Converts 0 to false and all other values to true.")]
    public float resetValue = -1;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (resetOnEnter)
        {
            ResetFloatOrInt(animator, parameterName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (resetOnExit)
        {
            ResetFloatOrInt(animator, parameterName);
        }
    }

    private void ResetFloatOrInt(Animator animator, string parameter)
    {
        if (animator.HasBool(parameter))
        {
            animator.SetBool(parameter, Convert.ToBoolean(resetValue));
        }
        else if (animator.HasFloat(parameter))
        {
            animator.SetFloat(parameter, resetValue);
        }
        else if (animator.HasInteger(parameter))
        {
            animator.SetInteger(parameter, (int)resetValue);
        }
    }
}