using System.Collections;
using UnityEngine;

[System.Serializable]
public class SkillController
{
    public SkillBase normalSkill;
    public SkillBase ultimateSkill;

    public IEnumerator UseSkill(CharacterManager caster, CharacterManager target)
    {
        // 1. Play attack animation
        if (caster.Hud != null && caster.Hud.animator != null)
        {
            caster.Hud.animator.SetTrigger("Attack");
        }

        // 2. Chờ animation vung tay / ra đòn
        yield return new WaitForSeconds(0.25f);

        // 3. Cast skill
        if (ultimateSkill != null &&caster.Hud.currentMana >= caster.Stats.baseStats.maxMana )
        {
            Debug.Log($"{caster.Stats.baseStats.characterName} uses ultimate skill: {ultimateSkill.skillName}");
            ultimateSkill.Cast(caster, target);
            // trừ mana 
            caster.Hud.UseMana(caster.Hud.currentMana);
        }
        else if (normalSkill != null)
        {
            Debug.Log($"{caster.Stats.baseStats.characterName} uses normal skill: {normalSkill.skillName}");
            normalSkill.Cast(caster, target);
        }
    }
}
