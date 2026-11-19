using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class AnimatorUtility
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        return System.Array.Exists(animator.parameters, p => p.name == paramName);
    }

    /*public static bool HasBool(this Animator animator, string paramName)
    {
        return System.Array.Exists(animator.parameters, p => p.GetType() == AnimatorControllerParameter.Bool && p.name == paramName);
    }

    public static bool HasInteger(this Animator animator, string paramName)
    {
        return System.Array.Exists(animator.parameters, p => p.GetType() == AnimatorControllerParameter.Int && p.name == paramName);
    }

    public static bool HasFloat(this Animator animator, string paramName)
    {
        return System.Array.Exists(animator.parameters, p => p.GetType() == AnimatorControllerParameter.Float && p.name == paramName);
    }

    public static bool HasTrigger(this Animator animator, string paramName)
    {
        return System.Array.Exists(animator.parameters, p => p.GetType() == AnimatorControllerParameter.Trigger && p.name == paramName);
    }*/
}