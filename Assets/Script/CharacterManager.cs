using UnityEngine;
using System.Linq;
using System.Collections;
using Fusion;

[System.Serializable]
public class CharacterManager : MonoBehaviour, ITurnActor
{
    public HudSystem Hud;
    public CurrentStats Stats;

    [Header("Skills")]
    public SkillController Skills = new SkillController();

    public void OnMyTurn()
    {
        StartCoroutine(OnMyTurnCoroutine());
    }

    private IEnumerator OnMyTurnCoroutine()
    {
        Debug.Log($"{Stats.baseStats.characterName}'s turn!");

        var enemies = TurnSystem.Instance.GetEnemies(Hud.typeTeam);
        enemies = enemies.Where(e => e != null && !e.Hud.isDead).ToList();

        if (enemies.Count == 0)
        {
            Debug.Log("No targets available.");
            TurnSystem.Instance.EndTurn();
            yield break;
        }

        CharacterManager target = enemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .First();

        Vector3 originalPosition = transform.position;

        if (Stats.baseStats.attackType == AttackType.Melee)
        {
            yield return StartCoroutine(MoveToTarget(target.transform.position, 1f, 15f));
            yield return StartCoroutine(AttackTarget(target));
            yield return StartCoroutine(MoveToTarget(originalPosition, 0f, 15f));
        }
        else
        {
            yield return StartCoroutine(AttackTarget(target));
        }

        yield return new WaitForSeconds(0.2f);

        TurnSystem.Instance.EndTurn();
    }

    private IEnumerator MoveToTarget(Vector3 destination, float stopDistance, float moveSpeed = 5f)
    {
        while (Vector3.Distance(transform.position, destination) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator AttackTarget(CharacterManager target)
    {
        yield return StartCoroutine(Skills.UseSkill(this, target));
    }

    public void OnDeath()
    {
        if (Hud.isDead) return;
        Hud.Die();
    }
}

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