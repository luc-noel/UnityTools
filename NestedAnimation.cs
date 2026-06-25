// A set of Unity menus for nesting Animation Clips under Animator Controllers
// Author: A.G. Noël, https://agnoeltech.blogspot.com/
// This work is free to use, share, and edit under CC BY 4.0: https://creativecommons.org/licenses/by/4.0/

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class NestedClipEditor : EditorWindow
{
    private string clipName = "New Clip";
    private AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;
    private bool useAdvancedOptions = false;
    private float speed = 1.0f;
    private bool useMultiplier = false;
    private string speedMultiplier = "";

    #region Menu Methods
    [MenuItem("Assets/Create/Nested Animation/Float Animation", false, 402)]
    static void FloatNestedAnimation()
    {
        CreateNestedClip(Selection.activeObject as AnimatorController, AnimatorControllerParameterType.Float, name: "New Float");
    }

    [MenuItem("Assets/Create/Nested Animation/Int Animation", false, 402)]
    static void IntNestedAnimation()
    {
        CreateNestedClip(Selection.activeObject as AnimatorController, AnimatorControllerParameterType.Int, name: "New Int");
    }

    [MenuItem("Assets/Create/Nested Animation/Bool Animation", false, 402)]
    static void BoolNestedAnimation()
    {
        CreateNestedClip(Selection.activeObject as AnimatorController, AnimatorControllerParameterType.Bool, name: "New Bool");
    }

    [MenuItem("Assets/Create/Nested Animation/Trigger Animation", false, 402)]
    static void TriggerNestedAnimation()
    {
        CreateNestedClip(Selection.activeObject as AnimatorController, AnimatorControllerParameterType.Trigger, name: "New Trigger");
    }

    [MenuItem("Assets/Create/Nested Animation/Custom Animation")]
    static void CustomNestedAnimation()
    {
        NestedClipEditor window = (NestedClipEditor)EditorWindow.GetWindow(typeof(NestedClipEditor), utility: false, title: "Create Nested Animation", focus: true);
        window.minSize = new Vector2(300, 200);
        window.maxSize = new Vector2(300, 200);
        window.Show();
    }
    #endregion

    #region Menu Validation Methods
    // Block Nested Animation menus unless animator controller is selected
    [MenuItem("Assets/Create/Nested Animation/Trigger Animation", true)]
    static bool TriggerNestedAnimationValidation()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        return false;
    }

    [MenuItem("Assets/Create/Nested Animation/Bool Animation", true)]
    static bool BoolNestedAnimationValidation()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        return false;
    }

    [MenuItem("Assets/Create/Nested Animation/Int Animation", true)]
    static bool IntNestedAnimationValidation()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        return false;
    }

    [MenuItem("Assets/Create/Nested Animation/Float Animation", true)]
    static bool FloatNestedAnimationValidation()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        return false;
    }

    [MenuItem("Assets/Create/Nested Animation/Custom Animation", true)]
    static bool CustomNestedAnimationValidation()
    {
        if (Selection.activeObject != null)
        {
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        return false;
    }
    #endregion

    private void OnGUI()
    {
        GUILayout.Label("Animation Clip Name");
        clipName = GUILayout.TextField(clipName, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space();
        GUILayout.Label("Parameter Type");
        parameterType = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup(parameterType, GUILayout.Width(150.0f));

        EditorGUILayout.Space();
        useAdvancedOptions = EditorGUILayout.Toggle("Advanced Options", useAdvancedOptions);

        if (useAdvancedOptions)
        {
            EditorGUILayout.Space();
            speed = EditorGUILayout.FloatField("Motion Speed", speed);

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(20.0f));
            useMultiplier = EditorGUILayout.Toggle("Use Multiplier", useMultiplier);
            GUILayout.EndHorizontal();

            if (useMultiplier)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(20.0f));
                speedMultiplier = EditorGUILayout.TextField("Multiplier Parameter", speedMultiplier);
                GUILayout.EndHorizontal();
            }

        }

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Forces button to right side
        if (GUILayout.Button("Create", GUILayout.Width(120.0f)))
        {
            if (Selection.activeObject.GetType() == typeof(AnimatorController))
            {
                if (!useAdvancedOptions)
                {
                    CreateNestedClip(Selection.activeObject as AnimatorController, parameterType);
                }
                else
                {
                    CreateNestedClip(Selection.activeObject as AnimatorController, parameterType, useAdvancedOptions, clipName, speed, speedMultiplier, useMultiplier);
                }
            }
            else
            {
                Debug.LogWarning("No valid animator controller selected.");
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    #region Clip Generation
    // Re-implements Unity's GenerateTriggerableTransition() from UI Buttons
    public static void CreateNestedClip( AnimatorController controller, AnimatorControllerParameterType parameterType, bool advancedOptions = true,
        // Optional parameters for advanced settings
        string name = "New Clip", float speed = 1.0f, string speedMultiplier = "", bool useMultiplier = false)
    {
        AnimationClip clip = AnimatorController.AllocateAnimatorClip(name);
        AssetDatabase.AddObjectToAsset((UnityEngine.Object)clip, (UnityEngine.Object)controller);
        AnimatorState destinationState = controller.AddMotion((Motion)clip);
        if (advancedOptions)
        {
            destinationState.speed = speed;
            destinationState.speedParameter = speedMultiplier;
            destinationState.speedParameterActive = useMultiplier;
        }
        controller.AddParameter(name, parameterType);
        controller.layers[0].stateMachine.AddAnyStateTransition(destinationState).AddCondition(AnimatorConditionMode.If, 0.0f, name);
        AssetDatabase.SaveAssets();
    }
    #endregion
}