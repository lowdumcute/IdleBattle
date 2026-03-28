using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BattlePVPManager : NetworkBehaviour
{
    public static BattlePVPManager Instance;

    public List<Transform> Player1SpawnPoints;
    public List<Transform> Player2SpawnPoints;

    public int MaxRound = 30;

    private Dictionary<PlayerRef, NetworkCharacterData[]> playerLineups 
        = new Dictionary<PlayerRef, NetworkCharacterData[]>();

    private bool isBattleStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            Debug.Log("Master ready, waiting lineup...");
        }
    }

    public void SendMyLineup()
    {
        var data = LineUpPVP.Instance.GetNetworkLineup();
        RPC_SendLineup(Runner.LocalPlayer, data);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendLineup(PlayerRef player, NetworkCharacterData[] lineup)
    {
        if (!playerLineups.ContainsKey(player))
            playerLineups.Add(player, lineup);

        Debug.Log($"Received lineup from {player}");

        if (playerLineups.Count >= 2 && !isBattleStarted)
        {
            isBattleStarted = true;
            StartCoroutine(StartBattle());
        }
    }

    IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(1f);

        var players = new List<PlayerRef>(playerLineups.Keys);

        SpawnTeam(playerLineups[players[0]], Player1SpawnPoints, true);
        SpawnTeam(playerLineups[players[1]], Player2SpawnPoints, false);

        // 🔥 đợi CharacterManager Register
        yield return new WaitForSeconds(1f);

        TurnPVPManager.Instance.GameStart();
    }

    void SpawnTeam(NetworkCharacterData[] team, List<Transform> points, bool isPlayer1)
    {
        int count = Mathf.Min(team.Length, points.Count);

        for (int i = 0; i < count; i++)
        {
            var data = team[i];
            Transform point = points[i];

            Character c = CreateCharacter(data);

            if (c == null)
            {
                Debug.LogError("❌ Character null → skip");
                continue;
            }

            NetworkObject obj = Runner.Spawn(
                c.currentStats.baseStats.characterPrefab,
                point.position,
                point.rotation
            );

            obj.transform.localScale = Vector3.one * 61f;

            CharacterManager cm = obj.GetComponent<CharacterManager>();
            if (cm != null)
            {
                cm.Stats = c.currentStats;
                cm.Hud.isPlayer = isPlayer1;
            }
        }
    }

    Character CreateCharacter(NetworkCharacterData data)
    {
        Character c = new Character();

        c.star = data.star;
        c.currentStats = new CurrentStats();

        Realm realm = (Realm)data.realm;

        var baseStats = DataManager.Instance.AllData
            .GetCharacter(data.characterID.ToString(), realm);

        if (baseStats == null)
        {
            Debug.LogError($"❌ Không tìm thấy ID: {data.characterID}");
            return null;
        }

        c.currentStats.baseStats = baseStats;
        c.currentStats.CurrentLevel = data.level;

        c.currentStats.InitializeStats();

        return c;
    }
}