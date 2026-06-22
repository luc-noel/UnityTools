/*
* Right-click menu to replace a selected file from within Unity
* Can replace files of different types or differen names while keeping the original file name
*/

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

public class EasyReplaceFile
{
    // Grey out the menu if the right-click selection isn't replaceable (no folders, no multi-selections)
    [MenuItem("Assets/Replace File", true, -11)]
    private static bool ValidSelection()
    {
        if (Selection.count > 1)
            return false;

        if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject)))
            return false;

        return true;
    }

    [MenuItem("Assets/Replace File", false, -11)]
    private static void Apply()
    {
        string selection = AssetDatabase.GetAssetPath(Selection.activeObject);
        // Get the extension so it can be passed into the editor window as the default extension filter
        string extension = Path.GetExtension(selection);
        // Remove . in front of extension, otherwise the OpenFilePanel doubles up on it
        extension = extension.Remove(0, 1);

        // Opens the OS specific file browser 
        string newFile = EditorUtility.OpenFilePanel("Replace File", "", extension);

        // Make sure the newFile actually has a name/anything was selected
        if (newFile.Length > 0)
        {
            ReplaceSelection(selection, newFile);
            // Refresh the project assets so the file change is immediately visible in the Project files
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }

    // Takes the path for the selected file and the one selected in the OS file browser and does the replacing
    static void ReplaceSelection(string oldFile, string newFile)
    {
        // True by default, assume there will be no errors
        bool userContinue = true;

        // We only display a pop-up dialogue if RequestWarningMessage is true
        if (RequestWarningMessage(oldFile, newFile, out string message))
        {
            // Pop-up dialogue with our warning, user can continue or cancel the operation
            userContinue = EditorUtility.DisplayDialog("Replace File", message, "Continue", "Cancel");
        }
        if (userContinue)
        {
            FileUtil.ReplaceFile(newFile, oldFile);
        }
    }

    // Checks whether the incoming file has a name or file extension conflict and formats a warning message if so.
    static bool RequestWarningMessage(string oldFile, string newFile, out string message)
    {
        string warning = string.Empty;

        string oldFileName = Path.GetFileNameWithoutExtension(oldFile);
        string newFileName = Path.GetFileNameWithoutExtension(newFile);
        string oldFileExtension = Path.GetExtension(oldFile);
        string newFileExtension = Path.GetExtension(newFile);

        if (oldFileName != newFileName)
        {
            warning += string.Format
            (
                // u2022 is the Unicodefor a bullet character
                "\u2022 File names \"{0}\" and \"{1}\" don't match.",
                oldFileName,
                newFileName
            );
        }
        if (oldFileExtension != newFileExtension)
        {
            // Add a newline between the file name and extension warning if both are true
            if (!string.IsNullOrEmpty(warning))
                warning += "\n";

            warning += string.Format
            (
                "\u2022 File extensions {0} and {1} don't match.",
                oldFileExtension.ToUpper(),
                newFileExtension.ToUpper()
            );
        }

        // If any warnings were added we output them to the message now
        if (!string.IsNullOrEmpty(warning))
        {
            warning += "\n\nContinue with replacing this file?";
            message = warning;
            return true;
        }
        else
        {
            // No error messages ot output
            message = string.Empty;
            return false;
        }
    }
}
#endif