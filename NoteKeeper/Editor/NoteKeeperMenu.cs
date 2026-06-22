using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace NoteKeeper
{
    public class NoteKeeperWindow : EditorWindow
    {
        private Vector2 _scroll;
        private static GUIStyle _elementBox;
        private List<Note> _notes;

        [MenuItem("Note Keeper/Notes Directory")]
        static void OpenNoteKeeper()
        {
            // Get existing window or create one
            NoteKeeperWindow window = GetWindow<NoteKeeperWindow>("Note Keeper", true);
            window.minSize = new Vector2(320, 500);

            // Dockable window type
            window.Show();
        }

        void OnGUI()
        {
            _elementBox = new GUIStyle("GroupBox");
            _elementBox.padding = new RectOffset(NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding, NoteKeeper.EditorPadding);

            _notes = AssetDatabase.FindAssets($"t:" + typeof(Note).Name).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Note>).ToList();

            EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            EditorGUILayout.LabelField("Note Assets", new GUIStyle("AM HeaderStyle") { alignment = TextAnchor.MiddleCenter });

            // Show message if no notes exist, otherwise draw clickable list
            if (_notes.Count < 1)
            {
                EditorGUILayout.HelpBox("No note assets found in the project.", MessageType.Info, true);
            }
            else
            {
                foreach (Note note in _notes)
                {
                    EditorGUILayout.BeginVertical(_elementBox);
                    //if (EditorGUILayout.DropdownButton(new GUIContent(note.name), FocusType.Passive))
                    if (GUILayout.Button(note.name))
                    {
                        // Make sure we're looking at the project window
                        EditorUtility.FocusProjectWindow();
                        // Change and ping the selection
                        Selection.activeObject = note;
                        EditorGUIUtility.PingObject(note);
                    }
                    EditorGUI.indentLevel++;

                    // Draw note information
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Notes: " + note.NotesCount(), EditorStyles.miniLabel, GUILayout.Width(70));
                    //EditorGUILayout.Space(NoteKeeper.EditorPadding);

                    EditorGUI.indentLevel--;
                    string path = AssetDatabase.GetAssetPath(note);
                    EditorGUILayout.LabelField(path, EditorStyles.miniLabel);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}