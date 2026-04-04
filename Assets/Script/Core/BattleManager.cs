using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class BattleManager : NetworkBehaviour
{
    [System.Serializable]
    public class CharacterPrefabEntry
    {
        public string id;
        public NetworkObject prefab;
    }

    public static BattleManager Instance;

    [Header("Prefab Map")]
    public List<CharacterPrefabEntry> prefabMap;
    private Dictionary<string, NetworkObject> prefabDict;

    [Header("Spawn Points")]
    public List<Transform> PlayerSpawnPoints;
    public List<Transform> EnemySpawnPoints;

    public float MaxRound;

    private void Awake()
    {
        Instance = this;

        // 🔥 INIT DICTIONARY
        prefabDict = new Dictionary<string, NetworkObject>();

        foreach (var e in prefabMap)
        {
            string key = e.id.Trim();

            if (!prefabDict.ContainsKey(key))
            {
                prefabDict.Add(key, e.prefab);
            }
            else
            {
                Debug.LogError($"❌ Trùng ID prefab: {key}");
            }
        }
    }

    // =========================
    // SPAWN TEAM
    // =========================
    public void SpawnTeam(PlayerRef player, NetworkCharacterData[] lineup, bool isPlayer)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        var spawnPoints = isPlayer ? PlayerSpawnPoints : EnemySpawnPoints;

        for (int i = 0; i < lineup.Length; i++)
        {
            if (i >= spawnPoints.Count) break;

            var data = lineup[i];

            string id = data.characterID.ToString();
            Realm realm = (Realm)data.realm;

            // 🔥 LẤY DATA TỪ DATAMANAGER
            var baseStats = DataManager.Instance.AllData.GetCharacter(id, realm);

            if (baseStats == null)
            {
                Debug.LogError($"❌ Không tìm thấy character: {id} - realm {realm}");
                continue;
            }

            // 🔥 LẤY PREFAB
            var prefabGO = baseStats.characterPrefab;

            if (prefabGO == null)
            {
                Debug.LogError($"❌ Prefab NULL: {id}");
                continue;
            }

            var netObj = prefabGO.GetComponent<NetworkObject>();

            if (netObj == null)
            {
                Debug.LogError($"❌ Prefab chưa có NetworkObject: {id}");
                continue;
            }

            // 🔥 SPAWN
            var obj = Runner.Spawn(
                netObj,
                spawnPoints[i].position,
                Quaternion.identity,
                player
            );

            var chara = obj.GetComponent<CharacterManager>();

            // 🔥 SET DATA
            chara.Stats.baseStats = baseStats;
            chara.Stats.CurrentLevel = data.level;

            chara.Stats.InitializeStats();

            // 🔥 TEAM
            chara.Hud.typeTeam = isPlayer ? TypeTeam.Player : TypeTeam.Enemy;

            Debug.Log($"✅ Spawn {id} (realm {realm}) tại slot {i}");
        }
    }

    // =========================
    // GET PREFAB (NEW)
    // =========================
    NetworkObject GetPrefabByID(string id)
    {
        if (prefabDict.TryGetValue(id, out var prefab))
            return prefab;

        return null;
    }
}