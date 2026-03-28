using UnityEngine;
using System.Linq;
using System.Collections;

[System.Serializable]
public class CharacterManager : MonoBehaviour, ITurnActor
{
    public HudSystem Hud;
    public CurrentStats Stats;

    [Header("Skills")]
    public SkillController Skills = new SkillController();

    void Start()
    {
        if (TurnSystem.Instance == null)
        {
            Debug.LogError("❌ TurnSystem chưa tồn tại!");
            return;
        }

        // 🔥 Register đúng hệ thống
        TurnSystem.Instance.Register(this);

        // init stats
        Stats.InitializeStats();

        // setup UI
        Hud.owner = this;
        Hud.SetUI(Stats.MHealth, Stats.ManaBonus, Stats.CurrentLevel, Stats.baseStats.characterName);
    }

    public void OnMyTurn()
    {
        StartCoroutine(OnMyTurnCoroutine());
    }

    private IEnumerator OnMyTurnCoroutine()
    {
        Debug.Log($"{Stats.baseStats.characterName}'s turn!");

        var enemies = TurnSystem.Instance.GetEnemies(Hud.isPlayer);
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