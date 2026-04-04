using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance;

    [Header("Settings")]
    public TextMeshProUGUI RoundText;
    [Networked]public int CurrentRound {get; set;}

    [Header("Teams")]
    public List<CharacterManager> playerTeam = new List<CharacterManager>();
    public List<CharacterManager> enemyTeam = new List<CharacterManager>();

    public List<CharacterManager> turnOrder = new List<CharacterManager>();
    private int currentTurnIndex = 0;

    private bool isGameStarted = false;

    public override void Spawned()
    {
        if (Instance == null)
            Instance = this;
    }
    public override void Render()
    {
        UpdateUI();
    }
    public void GameStart()
    {
        if (!HasStateAuthority) return;

        playerTeam.RemoveAll(c => c == null);
        enemyTeam.RemoveAll(c => c == null);

        Debug.Log($"PlayerTeam: {playerTeam.Count} | EnemyTeam: {enemyTeam.Count}");

        if (playerTeam.Count == 0 || enemyTeam.Count == 0)
        {
            Debug.LogWarning(" Team chưa sẵn sàng!");
            return;
        }

        UpdateTurnOrder();

        if (turnOrder.Count == 0)
        {
            Debug.LogWarning(" TurnOrder rỗng!");
            return;
        }

        CurrentRound = 1;

        currentTurnIndex = 0;
        isGameStarted = true;

        UpdateUI();

        StartTurn(turnOrder[currentTurnIndex]);
    }

    public void UpdateTurnOrder()
    {
        turnOrder.Clear();

        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);

        turnOrder.RemoveAll(c => c == null || c.Hud.isDead);

        turnOrder.Sort((a, b) => b.Stats.CSpeed.CompareTo(a.Stats.CSpeed));
    }

    void StartTurn(CharacterManager character)
    {
        if (!isGameStarted || character == null) return;

        if (character.Hud.isDead)
        {
            EndTurn();
            return;
        }

        Debug.Log($" Turn: {character.Stats.baseStats.characterName}");

        var actor = character.GetComponent<ITurnActor>();

        if (actor != null)
            actor.OnMyTurn();
        else
            EndTurn();
    }

    public void EndTurn()
    {
        if (!HasStateAuthority) return; // chỉ host điều khiển turn

        if (!isGameStarted || turnOrder.Count == 0) return;

        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            CurrentRound++; //  auto sync client

            if (BattleManager.Instance != null &&
                CurrentRound > BattleManager.Instance.MaxRound)
            {
                Debug.Log(" Hết round → hòa");
                isGameStarted = false;
                return;
            }

            UpdateTurnOrder();
            currentTurnIndex = 0;
        }

        if (turnOrder.Count > 0)
            StartTurn(turnOrder[currentTurnIndex]);

        CheckTeamDefeat();
    }

    public void Register(CharacterManager charData)
    {
        if (charData == null) return;

        if (charData.Hud.typeTeam == TypeTeam.Player)
        {
            if (!playerTeam.Contains(charData))
                playerTeam.Add(charData);
        }
        else
        {
            if (!enemyTeam.Contains(charData))
                enemyTeam.Add(charData);
        }
    }

    private void CheckTeamDefeat()
    {
        if (playerTeam.Count > 0 && playerTeam.TrueForAll(c => c.Hud.isDead))
        {
            Debug.Log("🔥 Enemy Wins!");
            isGameStarted = false;
        }
        else if (enemyTeam.Count > 0 && enemyTeam.TrueForAll(c => c.Hud.isDead))
        {
            Debug.Log("🔥 Player Wins!");
            isGameStarted = false;
        }
    }

    void UpdateUI()
    {
        if (RoundText != null)
        {
            float maxRound = BattleManager.Instance.MaxRound ;

            RoundText.text = $"Round {CurrentRound} / {maxRound}";
        }
    }
}