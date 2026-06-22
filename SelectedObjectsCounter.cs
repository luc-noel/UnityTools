/*
* Displays a count of the selected objects from the Hierarchy or Project at the bottom right corner of the Scene view.
* Toggleable using the C keybind or Tools menu item
*/
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SelectedObjectCounter : EditorWindow
{
    private const string _menuName = "Tools/Show Selection Count";
    private const string _menuHotkey = " _c";

    private static bool _isEnabled
    {
        get => EditorPrefs.GetBool(_menuName);
        set => EditorPrefs.SetBool(_menuName, value);
    }

    static SelectedObjectCounter()
    {
        ActivateCounter();
    }

    private static void ActivateCounter()
    {
        if (_isEnabled)
        {
            // Add the GUI overlay to SceneView
            SceneView.duringSceneGui += DrawCounterGUI;
            // Force repaint so the GUI immediately refreshes instead of waiting for the next user action
            SceneView.RepaintAll();
        }
        else
        {
            // Remove the GUI overlay from SceneView
            SceneView.duringSceneGui -= DrawCounterGUI;
            SceneView.RepaintAll();
        }
    }

    // UI set-up
    private static void DrawCounterGUI(SceneView sceneview)
    {
        Handles.BeginGUI();

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace(); // Push text to the bottom

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // Push text to the right
        GUILayout.Box(string.Format("[ Selected: {0} ]", Selection.objects.Length.ToString()));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        Handles.EndGUI();
    }

    // Menu item that toggles _isEnabled on/off
    [MenuItem(_menuName + _menuHotkey, false, priority: 120)]
    private static void ToggleAction()
    {
        _isEnabled = !_isEnabled;
        ActivateCounter();
    }

    // isValidateFunction makes this run before ToggleAction() because they affect the same menu item (_menuName)
    // Literally only necessary to make Unity update the checkmark in the menu to be on/off
    [MenuItem(_menuName + _menuHotkey, isValidateFunction: true, priority: 120)]
    private static bool ToggleActionValidate()
    {
        Menu.SetChecked(_menuName, _isEnabled);
        return true;
    }
}
#endif