using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MysteryMist/Configs/ScenePrefab", fileName = "ScenePrefabConfig")]
public class ScenePrefabConfiguration : ScriptableObject
{
    public List<GameObject> PrefabsToInstantiate; // Assign prefabs in the Inspector

}
