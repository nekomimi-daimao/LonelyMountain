using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace Nekomimi.Daimao.OpenAllPrefab.Editor
{
    public static class OpenAllPrefab
    {
        [MenuItem("Tools/Open All Prefab")]
        public static void OpenAll()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            var prefabsLength = prefabs.Length;

            Debug.Log(prefabsLength);

            try
            {
                for (var index = 0; index < prefabsLength; index++)
                {
                    var p = prefabs[index];

                    Debug.Log($"{p}");

                    var canceled = EditorUtility.DisplayCancelableProgressBar(
                        p,
                        $"{index}/{prefabsLength}",
                        (float)index / prefabsLength
                    );
                    if (canceled)
                    {
                        Debug.LogWarning($"{nameof(OpenAllPrefab)} canceled");
                        return;
                    }

                    var loadPrefabContents = PrefabUtility.LoadPrefabContents(p);
                    PrefabUtility.SaveAsPrefabAsset(loadPrefabContents, p);
                    PrefabUtility.UnloadPrefabContents(loadPrefabContents);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();

            Debug.Log($"{nameof(OpenAllPrefab)} succeeded");

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
