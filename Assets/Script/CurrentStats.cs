using UnityEngine;

[System.Serializable]
public class CurrentStats
{
    public CharacterBaseStats baseStats;
    [Header("Current Stats")]

    public float CSpeed;
    public float CAttack;
    public float CDef;
    public float MHealth;
    public float MMana;

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
