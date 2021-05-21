using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CustomHotkeys : MonoBehaviour
{

    [MenuItem("GameObject/Custom/Toggle _z")]
    private static void Toggle()
    {

        foreach (Transform t in Selection.transforms)
        {
            
            t.gameObject.SetActive(!t.gameObject.activeInHierarchy);

        }

        if (!EditorApplication.isPlaying)
        {

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        }

    }

    [MenuItem("GameObject/Custom/Play _x")]
    private static void Play()
    {

        EditorApplication.isPlaying = !EditorApplication.isPlaying;

    }
    
    [MenuItem("GameObject/Custom/Pause _c")]
    private static void Pause()
    {

        EditorApplication.isPaused = !EditorApplication.isPaused;

    }
    
    [MenuItem("GameObject/Custom/Step _v")]
    private static void Step()
    {

        EditorApplication.Step();

    }
    
}
