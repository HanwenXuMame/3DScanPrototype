using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class MvxFirstTimeIntro
{
    static string firstTimeSaveFile => Path.Combine(Application.dataPath, "Plugins/Mvx2/Editor/FirstTimeCompleted.txt");
    static string sampleScenePath = "Assets/Mvx Sample Scenes/1_SimpleMvxSample.unity";

    static MvxFirstTimeIntro()
    {
        bool firstTime = !File.Exists(firstTimeSaveFile);
        if (firstTime)
        {
            File.WriteAllText(firstTimeSaveFile, "");
            AssetDatabase.Refresh();

            if (EditorUtility.DisplayDialog("MVX First Time Import", "Do you want to open sample scene?\n\n" + sampleScenePath, "Open sample scene", "Ignore"))
            {
                EditorApplication.update += OpenSceneOnNextUpdate;
            }
        }
    }

    private static void OpenSceneOnNextUpdate()
    {
        EditorApplication.update -= OpenSceneOnNextUpdate;
        EditorSceneManager.OpenScene(sampleScenePath);

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(sampleScenePath, typeof(UnityEngine.Object));
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
}
