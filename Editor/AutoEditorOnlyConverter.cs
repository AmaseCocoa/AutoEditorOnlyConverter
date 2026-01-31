using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;

public class PrefabTagBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    private List<(GameObject obj, string originalTag)> changedObjects = new List<(GameObject, string)>();

    public void OnPreprocessBuild(BuildReport report)
    {
        changedObjects.Clear();

        string[] guids = AssetDatabase.FindAssets("t:PrefabTagManager");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var manager = AssetDatabase.LoadAssetAtPath<PrefabTagManager>(path);

            if (manager == null || !manager.isEnabled) continue;

            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var go in allObjects)
            {
                GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (source != null && manager.targetPrefabs.Contains(source))
                {
                    ApplyTagRecursive(go);
                }
            }
        }
    }

    private void ApplyTagRecursive(GameObject go)
    {
        if (go.tag != "EditorOnly")
        {
            changedObjects.Add((go, go.tag));
            go.tag = "EditorOnly";
        }
        foreach (Transform child in go.transform)
        {
            ApplyTagRecursive(child.gameObject);
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        foreach (var item in changedObjects)
        {
            if (item.obj != null) item.obj.tag = item.originalTag;
        }
        changedObjects.Clear();
    }
}
