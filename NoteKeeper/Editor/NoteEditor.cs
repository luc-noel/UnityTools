using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

// https://catlikecoding.com/unity/tutorials/editor/custom-list/
namespace NoteKeeper
{
    [CustomEditor(typeof(Note))]
    public class NoteEditor : Editor
    {
        private Note _target;
        private SerializedProperty _notes;
        private ReorderableList _list;

        private SerializedProperty _theme;
        // The editor for the NoteTheme scriptable object
        private NoteThemeEditor _cachedEditor;
        // Caching a copy of the settings to check if it gets changed
        private NoteTheme _cachedTheme;

        private GUIStyle _textStyle, _popupStyle;

        void OnEnable()
        {
            _target = target as Note;
            _notes = serializedObject.FindProperty("Notes");
            _theme = serializedObject.FindProperty("Theme");
            _cachedTheme = null;

            InitialiseList();
        }

        private SerializedProperty ElementPropertyRelative(string property, int index)
        {
            return _list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(property);
        }

        #region ReorderableList Methods
        private void InitialiseList()
        {
            _list = new ReorderableList(serializedObject, _notes, true, false, true, true)
            {
                drawElementCallback = DrawListItems,
                drawElementBackgroundCallback = DrawListItemsBackground,
                elementHeightCallback = SetElementHeights,
                onAddCallback = AddEmptyListItem
            };

            // Necessary to prevent DrawListItems from recolouring the add/delete buttons
            _list.drawFooterCallback = (rect) =>
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                // Creates a copy of the ReorderableList default values to use to draw the default footer
                ReorderableList.Defaults defaults = new ReorderableList.Defaults();
                defaults.DrawFooter(rect, _list);
            };

            _list.multiSelect = true;
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (_notes.arraySize < 1)
                return;

            SerializedProperty text = ElementPropertyRelative("Text", index);
            SerializedProperty tag = ElementPropertyRelative("Tag", index);

            GUI.contentColor = Color.white; // Reset any colouring from previous GUI

            // Change font styling for selected items
            if (!isActive || !isFocused)
                _textStyle.fontStyle = FontStyle.Normal;
            else
                _textStyle.fontStyle = FontStyle.Bold;

            float tagSize = 30; // Lock tag size to offset the whole area by the size
            rect.width -= tagSize + NoteKeeper.EditorPadding; // Make horizontal space for the tag

            // Even though we set the list element heights, this is necessary again to set the spacing between elements nicely
            rect.height = EditorGUI.GetPropertyHeight(text) - EditorGUIUtility.singleLineHeight - NoteKeeper.EditorPadding;
            rect.y += NoteKeeper.EditorPadding / 2; // recenter top and bottom of list

            // Draw the text area for notes
            text.stringValue = EditorGUI.TextArea(rect, text.stringValue, _textStyle);

            // Recolour the GUI after creating the text area
            // This prevents the notes font from being recoloured
            Color colour = _cachedTheme.GetColour(tag.intValue);
            if (!isActive || !isFocused)
                colour.a = 0.5f; // lower opacity when not selected
            GUI.contentColor = colour;

            // Position the tag
            rect.x = rect.xMax + NoteKeeper.EditorPadding;
            //rect.y += EditorGUIUtility.singleLineHeight;
            // Set the tag rect so the clicking area isn't as large as the text area
            rect.size = new Vector2(tagSize, tagSize);

            // Draw the tag, using the short symbols and not the full text
            tag.intValue = EditorGUI.Popup(rect, tag.intValue, _cachedTheme.GetShortSymbols(), _popupStyle);
        }

        void DrawListItemsBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (_notes.arraySize < 1)
                return;

            // Colour the notes based on their tag colour
            SerializedProperty tag = ElementPropertyRelative("Tag", index);

            Color colour = _cachedTheme.GetColour(tag.intValue);
            if (!isActive || !isFocused)
                colour.a = 0.5f;

            GUI.backgroundColor = colour;
        }

        private float SetElementHeights(int index)
        {
            // Line height for empty array
            if (_notes.arraySize < 1)
                return EditorGUIUtility.singleLineHeight;

            // Sets the height of a text area, padding handled in DrawListItems
            SerializedProperty element = ElementPropertyRelative("Text", index);
            return EditorGUI.GetPropertyHeight(element) - EditorGUIUtility.singleLineHeight;
        }

        private void AddEmptyListItem(ReorderableList list)
        {
            // Increase the ReorderableList array by one
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;

            // Settings for the new element to go in the added index
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Text").stringValue = "";
            element.FindPropertyRelative("Tag").intValue = 0;
            element.FindPropertyRelative("Timestamp").longValue = DateTime.Now.Ticks;
        }
        #endregion

        public override void OnInspectorGUI()
        {
            // GUI styles initialisation
            _textStyle = new GUIStyle(EditorStyles.textArea);
            _textStyle.alignment = TextAnchor.MiddleLeft;
            _textStyle.padding = new RectOffset(NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding);

            _popupStyle = new GUIStyle(EditorStyles.popup);
            _popupStyle.alignment = TextAnchor.MiddleCenter;
            _popupStyle.fontStyle = FontStyle.Bold;
            _popupStyle.fontSize = 16;

            serializedObject.Update();

            // Warning for missing theme, don't draw notes without it
            if (_theme.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Missing NoteTheme reference. Create a NoteTheme asset if none exist, then assign it to continue editing notes.", MessageType.Error);
                EditorGUILayout.ObjectField(_theme);
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                // Check if the referenced theme has been swapped
                if (_cachedTheme != _theme.objectReferenceValue)
                    _cachedTheme = null;
                if (_cachedTheme == null)
                {
                    _cachedTheme = (NoteTheme)_theme.objectReferenceValue;
                    _cachedEditor = (NoteThemeEditor)CreateEditor(_theme.objectReferenceValue);
                }

                EditorGUI.BeginChangeCheck();

                // Draw the NoteTheme editor and ReorderableList
                _cachedEditor.serializedObject.Update();
                _cachedEditor.OnInspectorGUI();

                // Draw a NoteTheme reference picker
                Rect themeRect = _cachedEditor.ObjectFieldRect;
                themeRect.x = themeRect.xMax - EditorGUIUtility.singleLineHeight - NoteKeeper.EditorPadding / 2;
                themeRect.size = Vector2.one * 20.0f;
                EditorGUI.ObjectField(themeRect, _theme, GUIContent.none);

                _list.DoLayoutList();

                // Draw the sorting buttons, pushed to the right side of the editor
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (EditorGUILayout.DropdownButton(new GUIContent("Sort by Tag"), FocusType.Keyboard, GUILayout.Width(100)))
                {
                    GenericMenu order = new GenericMenu();
                    // Re-intialise the whole list to draw it correctly, Repaint wouldn't capture it
                    order.AddItem(new GUIContent("Ascending ↥"), false, () =>
                    {
                        _target.SortByTag(true); InitialiseList();
                    });
                    order.AddItem(new GUIContent("Descending ↧"), false, () =>
                    {
                        _target.SortByTag(false); InitialiseList();
                    });
                    order.ShowAsContext();
                }

                if (EditorGUILayout.DropdownButton(new GUIContent("Sort by Time"), FocusType.Keyboard, GUILayout.Width(100)))
                {
                    GenericMenu order = new GenericMenu();
                    // Re-intialise the whole list to draw it correctly, Repaint wouldn't capture it
                    order.AddItem(new GUIContent("Ascending ↥"), false, () => { _target.SortByTimestamp(true); InitialiseList(); });
                    order.AddItem(new GUIContent("Descending ↧"), false, () => { _target.SortByTimestamp(false); InitialiseList(); });
                    order.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    // Record complete object undo necessary to capture the NoteTheme SO and refresh it accordingly
                    Undo.RegisterCompleteObjectUndo(_target, "Edit Notes");
                    //Undo.FlushUndoRecordObjects(); // Maybe necessary if some settings don't get captured, currently not needed
                    EditorUtility.SetDirty(_target);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}