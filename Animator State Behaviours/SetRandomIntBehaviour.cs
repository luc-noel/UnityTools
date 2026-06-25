using UnityEngine;
using PhantomCompass;

public class SetRandomIntBehaviour : StateMachineBehaviour
{
    public string parameterName = "Randomiser";

    [Header("On Enter")]
    [Tooltip("Toggle on to output non-repeating numbers.")]
    public bool dontRepeat = false;
    [Tooltip("Sets the parameter to a value from the minimum inclusive to the maximum exclusive.\nX = minimum. Y = maximum.")]
    public Vector2Int randomRange = new(0, 3);

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.HasParameter(parameterName))
        {
            int random = Random.Range(randomRange.x, randomRange.y);

            if (dontRepeat && random == animator.GetInteger(parameterName))
            {
                // Increment the random value or loop back around to the minimum to guarantee different value
                random = random < randomRange.y - 1 ? ++random : randomRange.x;
                animator.SetInteger(parameterName, random);
            }
            else
                animator.SetInteger(parameterName, random);
        }
    }
}