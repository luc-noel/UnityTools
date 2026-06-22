using System;
using UnityEngine;

namespace NoteKeeper
{
    [Serializable]
    public class NoteData
    {
        [TextArea(0, 8)]
        public string Text = "";
        [Range(0, 5)]
        public int Tag = 0;
        public long Timestamp;

        public NoteData()
        {
            Text = NoteKeeper.DefaultSymbol;
            Tag = 0;
            Timestamp = DateTime.Now.Ticks;
        }
    }
}