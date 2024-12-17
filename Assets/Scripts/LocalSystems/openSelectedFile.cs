using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openSelectedFile
{
    public string openFileDialog()
    {
        //拡張子フィルタ
        var extensions = new[]
        {
            new ExtensionFilter("zipファイル","zip"),
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
        //拡張子フィルタ
        var extensions = new[]
        {
            new ExtensionFilter("pngファイル","png"),
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
