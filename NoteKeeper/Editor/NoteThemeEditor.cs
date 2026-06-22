using System;
using UnityEditor;
using UnityEngine;

namespace NoteKeeper
{
    [CustomEditor(typeof(NoteTheme))]
    public class NoteThemeEditor : Editor
    {
        private NoteTheme _target;

        private GUIStyle _symbolStyle, _tagBoxStyle;
        private bool _editSymbols = true;

        private void OnEnable()
        {
            _target = target as NoteTheme;
        }

        private void DrawThemeGUI()
        {
            // Group the symbols and colour legend horizontally
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // forces centering

            // Positions the foldout at the start of the editor layout
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.size = Vector2.one * EditorGUIUtility.singleLineHeight;
            rect.y = rect.yMax / 2;

            // Draw the foldout
            _editSymbols = EditorGUI.Foldout(rect, _editSymbols, "", true);

            // Create tiny Legend label for foldout
            rect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(rect, "Legend", new GUIStyle(EditorStyles.miniLabel));

            // Draw the preview of the tag colours and short name
            for (int i = 0; i < _target.Options; i++)
            {
                // Create a group to apply a box styling to the tags
                EditorGUILayout.BeginHorizontal(_tagBoxStyle);
                // Colour only the text
                GUI.contentColor = _target.GetColour(i, 0.5f);

                EditorGUILayout.LabelField(_target.Symbols[i][..1], _symbolStyle, GUILayout.Width(10));

                // Put colour picker a vertical group with FlexibleSpaces to force centering
                // Couldn't get any other GUIStyles or GUILayout options to make this happen
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                _target.Colours[i] = EditorGUILayout.ColorField(new GUIContent(""), _target.Colours[i], false, false, false, GUILayout.MaxWidth(10), GUILayout.MaxHeight(10));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Draw the symbol editing fields if the foldout is opened
            if (_editSymbols)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < _target.Options; i++)
                {
                    GUI.contentColor = _target.GetColour(i, 0.5f);
                    _target.Symbols[i] = EditorGUILayout.TextField(_target.Symbols[i]);

                    // Fallback for empty string values
                    if (_target.Symbols[i] == "")
                        _target.Symbols[i] = NoteKeeper.DefaultSymbol;
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.contentColor = Color.white;
        }

        public override void OnInspectorGUI()
        {
            _symbolStyle = new GUIStyle(EditorStyles.boldLabel);
            _symbolStyle.alignment = TextAnchor.MiddleCenter;
            _symbolStyle.fontSize = 16;

            _tagBoxStyle = new GUIStyle("AC BoldHeader");
            _tagBoxStyle.alignment = TextAnchor.MiddleCenter;
            _tagBoxStyle.padding = new RectOffset(2, 2, 2, 2);
            _tagBoxStyle.margin = new RectOffset(NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding);

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            _target.SanitizeTheme();
            DrawThemeGUI();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}