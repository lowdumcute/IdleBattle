using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Character> playersTeam = new List<Character>();
    public List<Character> enemiesTeam = new List<Character>();

    public float CurrentPowerTeam;
    public float LevelPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RefreshLineup();
    }

    // ⬇ Lấy team từ Inventory và khởi tạo
    public void RefreshLineup()
    {
        playersTeam = CharacterInventory.Instance.GetLineupCharacters();

        foreach (var c in playersTeam)
            c.currentStats.InitializeStats();

        UpdateTeamPower();
    }

    // ⬇ Tính tổng Power
    public void UpdateTeamPower()
    {
        CurrentPowerTeam = 0;

        foreach (var c in playersTeam)
            CurrentPowerTeam += c.currentStats.PowerStats;
    }

    // ⬇ Thêm tướng vào đội hình
    public void SetLineup(Character character, bool value)
    {
        character.isLineup = value;
        RefreshLineup();
    }
}

[System.Serializable]
public class Character
{
    public CurrentStats currentStats;
    public Start star;
    public bool isLineup;
    
}

[System.Serializable]
public class CurrentStats
{
    public CharacterBaseStats baseStats;

    public float CSpeed;
    public float CAttack;
    public float CDef;
    public float MHealth;
    public float MMana;
    public float ManaBonus;

    public float CCriticalRate;
    public float CCriticalDamage;

    [Header("Progression")]
    public float PowerStats;
    public float CurrentLevel = 1;

    public void InitializeStats()
    {
        MHealth = baseStats.GetHealthByLevel(CurrentLevel);
        MMana = baseStats.maxMana;
        CAttack = baseStats.GetAttackByLevel(CurrentLevel);
        CDef = baseStats.GetDefenseByLevel(CurrentLevel);

        CSpeed = baseStats.Speed;
        CCriticalRate = baseStats.CriticalRate;
        CCriticalDamage = baseStats.CriticalDamage;

        UpdatePower();
    }

    // CT: ATK×1.2 + DEF×1 + HP×0.1 + SPD×2 + CR×15 + CD×8
    public void UpdatePower()
    {
        PowerStats =
            CAttack * 1.2f +
            CDef * 1f +
            MHealth * 0.1f +
            CSpeed * 2f +
            CCriticalRate * 15f +
            CCriticalDamage * 8f;
    }
}


