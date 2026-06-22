using System.Collections.Generic;
using UnityEngine;

namespace NoteKeeper
{
    [CreateAssetMenu(menuName = "Note Keeper/Note")]
    public class Note : ScriptableObject
    {
        [SerializeField]
        private List<NoteData> Notes = new List<NoteData>();
        [SerializeField]
        private NoteTheme Theme;

        public void SortByTag(bool ascending)
        {
            if (ascending)
                Notes.Sort((x, y) => x.Tag.CompareTo(y.Tag));
            else
                Notes.Sort((x, y) => y.Tag.CompareTo(x.Tag));
        }

        public void SortByTimestamp(bool ascending)
        {
            if (ascending)
                Notes.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            else
                Notes.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
        }

        public int NotesCount()
        {
            return Notes.Count;
        }
    }
}