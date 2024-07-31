using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Nekomimi.Daimao.CollectDependencies.Editor
{
    public static class CollectDependencies
    {
        [MenuItem("Assets/Collect Dependencies", false, priority: 30)]
        public static void CollectFromMenu()
        {
            var targetObject = Selection.activeObject;
            if (targetObject == null)
            {
                Debug.LogError($"{nameof(CollectDependencies)} no file selected");
                return;
            }

            var targetPath = AssetDatabase.GetAssetPath(targetObject);
            var results = Collect(targetPath, true);

            var pathResult = Path.Combine(Application.temporaryCachePath, $"{nameof(CollectDependencies)}.txt");
            var utf8Encoding = new UTF8Encoding(false);
            File.WriteAllText(pathResult, $"{targetPath}{Environment.NewLine}---{Environment.NewLine}", utf8Encoding);
            File.AppendAllLines(pathResult, results, utf8Encoding);
            EditorUtility.RevealInFinder(pathResult);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string[] Collect(string targetPath, bool progress = false)
        {
            Debug.Log($"{nameof(CollectDependencies)}:{targetPath}");

            var allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(s => s.StartsWith("Assets/"))
                .Where(s => !s.EndsWith(".cs"))
                .Where(File.Exists)
                .ToArray();
            var allAssetsLength = allAssets.Length;
            var set = new HashSet<string>(allAssetsLength);

            try
            {
                for (var i = 0; i < allAssets.Length; i++)
                {
                    var assetPath = allAssets[i];
                    var dependencies = AssetDatabase.GetDependencies(assetPath, true);
                    if (dependencies.Contains(targetPath))
                    {
                        set.Add(assetPath);
                    }

                    if (progress)
                    {
                        var canceled = EditorUtility.DisplayCancelableProgressBar(
                            targetPath,
                            $"{i}/{allAssetsLength}",
                            (float)i / allAssetsLength
                        );
                        if (canceled)
                        {
                            Debug.LogWarning($"{nameof(CollectDependencies)} canceled");
                            return Array.Empty<string>();
                        }
                    }
                }
            }
            finally
            {
                if (progress)
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            set.Remove(targetPath);
            var result = set.ToArray();
            Array.Sort(result);
            return result;
        }
    }
}
