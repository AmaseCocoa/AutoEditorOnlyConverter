using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;
using VRC.Core;

public class PrefabTagBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    private List<(GameObject obj, string originalTag)> _changedObjects = new List<(GameObject, string)>();

    public void OnPreprocessBuild(BuildReport report)
    {
        _changedObjects.Clear();

        var manager = Object.FindObjectOfType<SceneBuildTagManager>();
        if (manager == null || !manager.isEnabled) return;

        var pipelineManager = Object.FindObjectOfType<PipelineManager>();
        string currentId = pipelineManager != null ? pipelineManager.blueprintId : "";

        if (!string.IsNullOrEmpty(currentId) && manager.targetBlueprintIds.Contains(currentId))
        {
            return;
        }

        foreach (var go in manager.targetObjects)
        {
            if (go == null) continue;
            ApplyTagRecursive(go);
        }
    }

    private void ApplyTagRecursive(GameObject go)
    {
        if (go.tag != "EditorOnly")
        {
            _changedObjects.Add((go, go.tag));
            go.tag = "EditorOnly";
        }

        foreach (Transform child in go.transform)
        {
            ApplyTagRecursive(child.gameObject);
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        foreach (var item in _changedObjects)
        {
            if (item.obj != null) item.obj.tag = item.originalTag;
        }
        _changedObjects.Clear();
    }
}

