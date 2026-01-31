using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;

#if VRC_SDK_VRCSDK3
using VRC.Core;
#endif

public class PrefabTagBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    private List<(GameObject obj, string originalTag)> _changedObjects = new List<(GameObject, string)>();

    public void OnPreprocessBuild(BuildReport report)
    {
        _changedObjects.Clear();

        var manager = Object.FindObjectOfType<SceneBuildTagManager>();
        if (manager == null || !manager.isEnabled) return;

        if (CheckShouldSkipByBlueprint(manager))
        {
            return;
        }

        foreach (var go in manager.targetObjects)
        {
            if (go == null) continue;
            ApplyTagRecursive(go);
        }
    }

    private bool CheckShouldSkipByBlueprint(SceneBuildTagManager manager)
    {
#if VRC_SDK_VRCSDK3
        var pipelineManager = Object.FindObjectOfType<PipelineManager>();
        string currentId = pipelineManager != null ? pipelineManager.blueprintId : "";

        if (!string.IsNullOrEmpty(currentId) && manager.targetBlueprintIds.Contains(currentId))
        {
            Debug.Log($"<color=cyan>[BuildTagManager]</color> Test ID detected: {currentId}. Skipping conversion.");
            return true;
        }
#endif
        return false;
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
