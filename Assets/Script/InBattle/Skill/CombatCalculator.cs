using UnityEngine;

public static class CombatCalculator
{
    public static int CalculateSkillDamage(
    float baseDamage,
    float atkRatio,
    float hpRatio,
    float levelScale,
    bool canCrit,
    CurrentStats caster,
    CurrentStats target
)
{
    float damage =
        baseDamage +
        caster.CAttack * atkRatio +
        caster.MHealth * hpRatio +
        caster.CurrentLevel * levelScale;

    damage = damage * caster.CAttack /
            (caster.CAttack + target.CDef);

    if (canCrit && Random.value < caster.CCriticalRate)
        damage *= 1f + caster.CCriticalDamage;

    damage *= Random.Range(0.95f, 1.05f);

    return Mathf.Max(1, Mathf.CeilToInt(damage));
}

}
