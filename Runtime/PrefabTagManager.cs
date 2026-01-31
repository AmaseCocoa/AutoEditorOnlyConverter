using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PrefabTagManager", menuName = "Tools/Prefab Tag Manager")]
public class PrefabTagManager : ScriptableObject
{
    [Tooltip("ビルド時にEditorOnlyにしたいPrefabのリスト")]
    public List<GameObject> targetPrefabs = new List<GameObject>();

    [Tooltip("このマネージャーによる自動変換を有効にするか")]
    public bool isEnabled = true;
}
