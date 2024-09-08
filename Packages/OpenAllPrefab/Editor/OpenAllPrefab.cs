using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
// ReSharper disable once CheckNamespace
using UnityEditor;

namespace Nekomimi.Daimao.OpenAllPrefab.Editor
{
    public static class OpenAllPrefab
    {
        [MenuItem("Tools/Open All Prefab")]
        public static void OpenAll()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            var prefabsLength = prefabs.Length;


            for (var index = 0; index < prefabsLength; index++)
            {
                var p = prefabs[index];

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

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
