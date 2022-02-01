using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
/// <summary>
/// Scene auto loader.
/// </summary>
/// <description>
/// This class adds a File > Scene Autoload menu containing options to select
/// a "master scene" enable it to be auto-loaded when the user presses play
/// in the editor. When enabled, the selected scene will be loaded on play,
/// then the original scene will be reloaded on stop.
///
/// Based on an idea on this thread:
/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>
[InitializeOnLoad]
public static class SceneAutoLoader
{
    // Static constructor binds a playmode-changed callback.
    // [InitializeOnLoad] above makes sure this gets execusted.
    static SceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    // Menu items to select the "master" scene and control whether or not to load it.
    [MenuItem("File/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
        if (!string.IsNullOrEmpty(masterScene))
        {
            MasterScene = masterScene;
            LoadMasterOnPlay = true;
        }
    }
    [MenuItem("File/Scene Autoload/Load Master On Play", true)]
    private static bool ShowLoadMasterOnPlay()
    {
        return !LoadMasterOnPlay;
    }
    [MenuItem("File/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = true;
    }
    [MenuItem("File/Scene Autoload/Don't Load Master On Play", true)]
    private static bool ShowDontLoadMasterOnPlay()
    {
        return LoadMasterOnPlay;
    }
    [MenuItem("File/Scene Autoload/Don't Load Master On Play")]
    private static void DisableLoadMasterOnPlay()
    {
        LoadMasterOnPlay = false;
    }
    // Play mode change callback handles the scene load/reload.
    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!LoadMasterOnPlay) return;
        if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // User pressed play -- autoload master scene.
            PreviousScene = GetCurrentScenesNames();
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(MasterScene, OpenSceneMode.Single);
            }
            else
            {
                // User cancelled the save operation -- cancel play as well.
                EditorApplication.isPlaying = false;
            }
        }
        if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // User pressed stop -- reload previous scene.
            if (SceneAutoLoader.PreviousScene != SceneAutoLoader.MasterScene)
                EditorApplication.update += ReloadLastScene;
        }
    }
    public static void ReloadLastScene()
    {
        string[] scenes = SceneAutoLoader.PreviousScene.Split(';');
        if (!EditorApplication.isPlaying && EditorSceneManager.GetActiveScene().path != scenes[0])
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                EditorSceneManager.OpenScene(scenes[i], i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }
            EditorApplication.update -= ReloadLastScene;
        }
    }

    private static string GetCurrentScenesNames()
    {
        int count = EditorSceneManager.sceneCount;
        string scenes = "";
        for (int i = 0; i < count; i++)
        {
            scenes += EditorSceneManager.GetSceneAt(i).path + ((i < count - 1) ? ";" : "");
        }
        return scenes;
    }

    // Properties are remembered as editor preferences.
    private const string cEditorPrefLoadMasterOnPlay = "CONTRA_ONLINE_SceneAutoLoader.LoadMasterOnPlay";
    private const string cEditorPrefMasterScene = "CONTRA_ONLINE_SceneAutoLoader.MasterScene";
    private const string cEditorPrefPreviousScene = "CONTRA_ONLINE_SceneAutoLoader.PreviousScene";
    public static bool LoadMasterOnPlay
    {
        get { return EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false); }
        set { EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value); }
    }
    private static string MasterScene
    {
        get { return EditorPrefs.GetString(cEditorPrefMasterScene, "Master.unity"); }
        set { EditorPrefs.SetString(cEditorPrefMasterScene, value); }
    }
    private static string _previousScene;
    public static string PreviousScene
    {
        get { return EditorPrefs.GetString(cEditorPrefPreviousScene, _previousScene); }
        set
        {
            _previousScene = value;
            EditorPrefs.SetString(cEditorPrefPreviousScene, value);
        }
    }
}