using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnPVPManager : MonoBehaviour
{
    public static TurnPVPManager Instance;

    [Header("Settings")]
    public TextMeshProUGUI RoundText;
    public int CurrentRound;

    [Header("Teams")]
    public List<CharacterManager> playerTeam = new List<CharacterManager>();
    public List<CharacterManager> enemyTeam = new List<CharacterManager>();

    public List<CharacterManager> turnOrder = new List<CharacterManager>();
    private int currentTurnIndex = 0;

    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void GameStart()
    {
        Debug.Log("🔥 GameStart PVP");

        playerTeam.RemoveAll(c => c == null);
        enemyTeam.RemoveAll(c => c == null);

        if (playerTeam.Count == 0 || enemyTeam.Count == 0)
        {
            Debug.LogWarning("❌ Team chưa sẵn sàng!");
            return;
        }

        UpdateTurnOrder();

        if (turnOrder.Count == 0)
        {
            Debug.LogWarning("❌ TurnOrder rỗng!");
            return;
        }

        CurrentRound = 1;
        UpdateUI();

        currentTurnIndex = 0;
        isGameStarted = true;

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

        Debug.Log($"👉 Turn: {character.Stats.baseStats.characterName}");

        var actor = character.GetComponent<ITurnActor>();

        if (actor != null)
            actor.OnMyTurn();
        else
            EndTurn();
    }

    public void EndTurn()
    {
        if (!isGameStarted || turnOrder.Count == 0) return;

        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            CurrentRound++;

            // 🔥 dùng BattlePVPManager
            if (BattlePVPManager.Instance != null &&
                CurrentRound > BattlePVPManager.Instance.MaxRound)
            {
                Debug.Log("⏱ Hết round → hòa");
                isGameStarted = false;
                return;
            }

            UpdateTurnOrder();
            currentTurnIndex = 0;

            UpdateUI();
        }

        if (turnOrder.Count > 0)
            StartTurn(turnOrder[currentTurnIndex]);

        CheckTeamDefeat();
    }

    public void Register(CharacterManager charData)
    {
        if (charData == null) return;

        if (charData.Hud.isPlayer)
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
            int maxRound = BattlePVPManager.Instance != null 
                ? BattlePVPManager.Instance.MaxRound 
                : 0;

            RoundText.text = $"Round {CurrentRound} / {maxRound}";
        }
    }
}