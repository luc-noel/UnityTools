using System;
using UnityEngine;

namespace NoteKeeper
{
    [CreateAssetMenu(menuName = "Note Keeper/Theme")]
    public class NoteTheme : ScriptableObject
    {
        // Limit only 6 theme options
        private const int _options = 6;
        public int Options { get { return _options; } }

        [SerializeField]
        public string[] Symbols = new string[_options]
        {
            "▫ None",
            "◤ Low",
            "◥ Medium",
            "◢ High",
            "◣ Bug",
            "■ TODO"
        };

        [SerializeField]
        public Color[] Colours = new Color[_options]
        {
            Color.white,
            new Color32(255, 207, 51, 255),
            new Color32(255, 145, 77, 255),
            new Color32(255, 95, 162, 255),
            new Color32(78, 125, 255, 255),
            new Color32(51, 214, 201, 255)
        };

        public Color GetColour(int index, float alpha)
        {
            Color colour = Colours[index];
            colour.a = alpha;
            return colour;
        }

        public Color GetColour(int index)
        {
            return GetColour(index, 1.0f);
        }

        // Returns a list of just the first character of the tag names
        public string[] GetShortSymbols()
        {
            string[] shortSymbols = new string[Options];

            Array.Copy(Symbols, shortSymbols, Options);
            for (int i = 0; i < shortSymbols.Length; i++)
            {
                shortSymbols[i] = shortSymbols[i][..1];
            }

            return shortSymbols;
        }

        // Ensure the tag variables never go above or below the Options count
        // Should be hard to accomplish but just in case
        public void SanitizeTheme()
        {
            if (Symbols.Length != Options)
                Array.Resize(ref Symbols, Options);

            if (Colours.Length != Options)
                Array.Resize(ref Colours, Options);

            // Replace any empty strings with default symbol
            for (int i = 0; i < Symbols.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(Symbols[i]))
                    Symbols[i] = NoteKeeper.DefaultSymbol;
            }

            // We don't allow alpha editing, so enforce full opacity
            for (int i = 0; i < Colours.Length; i++)
            {
                Colours[i].a = 1.0f;
            }
        }
    }
}