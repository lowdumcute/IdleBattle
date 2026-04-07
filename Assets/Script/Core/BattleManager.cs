using UnityEngine;
using System.Collections.Generic;
using Fusion;
using System.Linq;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance;

    [Header("Spawn Points")]
    public List<InfoSpawn> PlayerSpawnPoints;
    public List<InfoSpawn> EnemySpawnPoints;
    [Header("info Round")]
    public int MaxRound = 20;
    private Dictionary<PlayerRef, NetworkCharacterData[]> playerLineups = new();

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // CLIENT → HOST gửi lineup
    // =========================
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendLineup(NetworkCharacterData[] lineup, RpcInfo info = default)
    {
        playerLineups[info.Source] = lineup;

        Debug.Log($"📩 Nhận lineup từ player: {info.Source}");

        // 🔥 Khi đủ 2 player thì spawn
        if (playerLineups.Count >= 2)
        {
            SpawnAll();
        }
    }

    // =========================
    void SpawnAll()
    {
        var players = playerLineups.Keys.ToList();

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var lineup = playerLineups[player];

            bool isPlayer = (i == 0); // player đầu là Player, sau là Enemy

            SpawnTeam(player, lineup, isPlayer);
        }
    }

    // =========================
    public void SpawnTeam(PlayerRef player, NetworkCharacterData[] lineup, bool isPlayer)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        var spawnPoints = isPlayer ? PlayerSpawnPoints : EnemySpawnPoints;

        foreach (var data in lineup)
        {
            string id = data.characterID.ToString();
            Position pos = (Position)data.position;

            var spawnInfo = spawnPoints.Find(s => s.position == pos);

            if (spawnInfo == null || spawnInfo.spawnPoint == null)
            {
                Debug.LogError($"❌ Không tìm spawn point: {pos}");
                continue;
            }

            var baseStats = DataManager.Instance.AllData.GetCharacter(id, (Realm)data.realm);

            var netObj = baseStats.characterPrefab.GetComponent<NetworkObject>();

            var obj = Runner.Spawn(
                netObj,
                spawnInfo.spawnPoint.position,
                Quaternion.identity,
                player
            );

            var chara = obj.GetComponent<CharacterManager>();

            chara.Stats.baseStats = baseStats;
            chara.Stats.CurrentLevel = data.level;
            chara.Stats.InitializeStats();

            chara.Hud.typeTeam = isPlayer ? TypeTeam.Player : TypeTeam.Enemy;

            Debug.Log($"✅ Spawn {id} cho player {player}");
        }
    }
}

[System.Serializable]
public class InfoSpawn
{
    public Position position;
    public Transform spawnPoint;
}