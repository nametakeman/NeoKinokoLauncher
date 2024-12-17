using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openSelectedFile
{
    public string openFileDialog()
    {
        //�g���q�t�B���^
        var extensions = new[]
        {
            new ExtensionFilter("zip�t�@�C��","zip"),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0 && paths[0].Length > 0)
        {
            return paths[0];
        }
        else return "error";
    }

    public string openFileDialogImage()
    {
        //�g���q�t�B���^
        var extensions = new[]
        {
            new ExtensionFilter("png�t�@�C��","png"),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0 && paths[0].Length > 0)
        {
            return paths[0];
        }
        else return "error";
    }

    public string openFileDialogAny()
    {
        var extensions = new[]
{
            new ExtensionFilter("Any","*"),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0 && paths[0].Length > 0)
        {
            return paths[0];
        }
        else return "error";
    }

    public string openFolderDialog()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
        if (paths.Length > 0 && paths[0].Length > 0)
        {
            return paths[0];
        }
        else return "error";
    }
}
