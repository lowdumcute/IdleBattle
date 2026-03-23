using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Skill/Ultimate/Ather")]
public class AtherUltimate : SkillBase
{
    [Header("Damage")]
    public float baseDamage = 15f;
    public float attackRatio = 0.8f;

    protected override void Execute(
        CharacterManager caster,
        CharacterManager _)
    {
        var enemies = caster.Hud.isPlayer
            ? TurnManager.Instance.enemyTeam
            : TurnManager.Instance.playerTeam;

        enemies = enemies.Where(e => !e.Hud.isDead).ToList();

        foreach (var target in enemies)
        {
            float rawDamage =
                baseDamage +
                caster.Stats.CAttack * attackRatio;

            float damage = rawDamage * caster.Stats.CAttack /
                (caster.Stats.CAttack + target.Stats.CDef);

            if (Random.value < caster.Stats.CCriticalRate)
            {
                damage *= 1f + caster.Stats.CCriticalDamage;
            }

            int finalDamage = Mathf.Max(1, Mathf.CeilToInt(damage));

            DamageResolver.ApplyDamage(target, finalDamage);

        }
    }
}
