using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Settings")]
    public TextMeshProUGUI RoundText;
    public float CurrentRound ;

    [Header("Teams")]
    public List<CharacterManager> playerTeam = new List<CharacterManager>();
    public List<CharacterManager> enemyTeam = new List<CharacterManager>();

    // Danh sách lượt thực sự, để quản lý thứ tự turn
    public List<CharacterManager> turnOrder = new List<CharacterManager>();
    private int currentTurnIndex = 0;

    void Awake()
    {
        // Singleton chuẩn
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void GameStart()
    {
        // Cập nhật thứ tự lượt trước khi bắt đầu
        UpdateTurnOrder();
        CurrentRound = 1;
        RoundText.text = $"{CurrentRound} / {BattleManager.Instance.MaxRound}";
        if (turnOrder.Count > 0)
        {
            currentTurnIndex = 0;
            StartTurn(turnOrder[currentTurnIndex]);
        }
    }

    // Cập nhật thứ tự lượt dựa trên speed
    public void UpdateTurnOrder()
    {
        turnOrder.Clear();
        turnOrder.AddRange(playerTeam);
        turnOrder.AddRange(enemyTeam);
        turnOrder.Sort((a, b) => b.Stats.CSpeed.CompareTo(a.Stats.CSpeed));
    }
    
    void StartTurn(CharacterManager character)
    {
        if (turnOrder.Count == 0) return;
        if (character == null) return;
        if (character.Hud.isDead)
        {
            Debug.Log($"{character.Stats.baseStats.characterName} is dead, skipping turn.");
            EndTurn();
            return;
        }
        Debug.Log($"Tới lượt: {character.Stats.baseStats.characterName}");

        // Gọi sự kiện → nhân vật tự chọn skill / tấn công
        var turnActor = character.GetComponent<ITurnActor>();
        if (turnActor != null)
            turnActor.OnMyTurn();
        else
            Debug.LogWarning($"{character.name} không có ITurnActor!");
    }

    // Gọi khi 1 character kết thúc lượt
    public void EndTurn()
    {
        if (turnOrder.Count == 0) return;
        currentTurnIndex++;
        if (currentTurnIndex >= turnOrder.Count)
        {
            // Vòng lặp lượt mới
            UpdateTurnOrder();
            currentTurnIndex = 0;
            CurrentRound++;
            UpdateUI();
        }

        if (turnOrder.Count > 0)
            StartTurn(turnOrder[currentTurnIndex]);

        CheckTeamDefeat();
    }

    // Quản lý đăng ký / xóa nhân vật
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

    public void Remove(CharacterManager charData)
    {
        if (charData == null) return;

        if (charData.Hud.isPlayer)
            playerTeam.Remove(charData);
        else
            enemyTeam.Remove(charData);
    }
    private void CheckTeamDefeat()
    {
        if (playerTeam.TrueForAll(c => c.Hud.isDead))
        {
            Debug.Log("Enemy Team Wins!");
            turnOrder.Clear();
        }
        else if (enemyTeam.TrueForAll(c => c.Hud.isDead))
        {
            Debug.Log("Player Team Wins!");
            turnOrder.Clear();
        }
    }
    public void UpdateUI()
    {
        RoundText.text = $"{CurrentRound} / {BattleManager.Instance.MaxRound}";
    }
}
