using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Normal/Ather")]
public class AtherNormal : SkillBase
{
    [Header("Damage")]
    public float baseDamage = 20f;
    public float attackRatio = 1.0f;

    protected override void Execute(
        CharacterManager caster,
        CharacterManager target
    )
    {
        float rawDamage =
            baseDamage +
            caster.Stats.CAttack * attackRatio;

        // giảm theo DEF
        float damage = rawDamage * caster.Stats.CAttack /
            (caster.Stats.CAttack + target.Stats.CDef);

        // crit
        if (Random.value < caster.Stats.CCriticalRate)
        {
            damage *= 1f + caster.Stats.CCriticalDamage;
        }

        int finalDamage = Mathf.Max(1, Mathf.CeilToInt(damage));

        DamageResolver.ApplyDamage(target, finalDamage);

        // hồi mana mỗi hit
        caster.Hud.RecoverMana(25f);
    }
}
