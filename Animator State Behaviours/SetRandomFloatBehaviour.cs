using UnityEngine;
using PhantomCompass;

public class SetRandomFloatBehaviour : StateMachineBehaviour
{
    public string parameterName = "Randomiser";

    [Header("On Enter")]
    [Tooltip("Toggle on to output non-repeating numbers.")]
    public bool dontRepeat = false;
    [Tooltip("When enforcing non-repeating numbers the next value is incremented by this amount.")]
    public float repeatIncrement = 1.0f;
    [Tooltip("Sets the parameter to a value from the minimum inclusive to the maximum inclusive.\nX = minimum. Y = maximum.")]
    public Vector2 randomRange = new(0, 3);

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.HasParameter(parameterName))
        {
            float random = Random.Range(randomRange.x, randomRange.y);

            if (dontRepeat && random == animator.GetInteger(parameterName))
            {
                // Increment the random value or loop back around to the minimum to guarantee different value
                random = random < randomRange.y - repeatIncrement ? ++random : randomRange.x;
                animator.SetFloat(parameterName, random);
            }
            else
                animator.SetFloat(parameterName, random);
        }
    }
}