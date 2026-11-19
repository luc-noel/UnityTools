using UnityEngine;

public static class AnimatorUtility
{
    public static bool HasParameter(this Animator animator, string parameterName)
    {
        bool value = System.Array.Exists(animator.parameters, p => p.name == parameterName);
        Debug.Assert(value, "No parameter matching \"" + parameterName + "\" was found on " + animator.runtimeAnimatorController.name + ".");
        return value;
    }

    public static bool HasBool(this Animator animator, string parameterName)
    {
        bool value = System.Array.Exists(animator.parameters, p => p.type == AnimatorControllerParameterType.Bool && p.name == parameterName);
        Debug.Assert(value, "No bool parameter matching \"" + parameterName + "\" was found on " + animator.runtimeAnimatorController.name + ".");
        return value;
    }

    public static bool HasInt(this Animator animator, string parameterName)
    {
        bool value = System.Array.Exists(animator.parameters, p => p.type == AnimatorControllerParameterType.Int && p.name == parameterName);
        Debug.Assert(value, "No int parameter matching \"" + parameterName + "\" was found on " + animator.runtimeAnimatorController.name + ".");
        return value;
    }

    public static bool HasFloat(this Animator animator, string parameterName)
    {
        bool value = System.Array.Exists(animator.parameters, p => p.type == AnimatorControllerParameterType.Float && p.name == parameterName);
        Debug.Assert(value, "No float parameter matching \"" + parameterName + "\" was found on " + animator.runtimeAnimatorController.name + ".");
        return value;
    }

    public static bool HasTrigger(this Animator animator, string parameterName)
    {
        bool value = System.Array.Exists(animator.parameters, p => p.type == AnimatorControllerParameterType.Trigger && p.name == parameterName);
        Debug.Assert(value, "No trigger parameter matching \"" + parameterName + "\" was found on " + animator.runtimeAnimatorController.name + ".");
        return value;
    }
}