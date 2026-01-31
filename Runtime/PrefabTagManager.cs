using UnityEngine;
using System.Collections.Generic;

public class SceneBuildTagManager : MonoBehaviour
{
    [Header("ビルド時にEditorOnlyにするリスト")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("Blueprint ID 設定 (空なら常に有効)")]
    [Tooltip("ここにIDを登録すると、そのIDでビルドする時のみ実行されます")]
    public List<string> targetBlueprintIds = new List<string>();

    [Header("この機能を有効にするか")]
    public bool isEnabled = true;
}
