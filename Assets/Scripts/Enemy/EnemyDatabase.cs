using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : Singleton<EnemyDatabase>
{
    [Serializable]
    public class EnemyPrefabEntry
    {
        public Types.EnemyType enemyType;
        public EnemyControllerBase prefab;
    }

    [Header("Enemy Prefabs")]
    [SerializeField]
    private List<EnemyPrefabEntry> enemyPrefabsList = new List<EnemyPrefabEntry>();

    private Dictionary<Types.EnemyType, EnemyControllerBase> enemyPrefabsDict;

    private Dictionary<Types.EnemyType, EnemyControllerBase> EnemyPrefabs
    {
        get {
            if (enemyPrefabsDict == null)
            {
                enemyPrefabsDict = new Dictionary<Types.EnemyType, EnemyControllerBase>();
                foreach (var entry in enemyPrefabsList)
                {
                    if (entry.prefab != null)
                        enemyPrefabsDict[entry.enemyType] = entry.prefab;
                }
            }
            return enemyPrefabsDict;
        }
    }

    public EnemyControllerBase GetEnemyPrefabByType(Types.EnemyType enemyType)
    {
        EnemyPrefabs.TryGetValue(enemyType, out EnemyControllerBase prefab);
        return prefab;
    }
    
}