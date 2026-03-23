using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("Base Info")]
    public string skillName;

    public void Cast(CharacterManager caster, CharacterManager target)
    {
        Execute(caster, target);
    }

    protected abstract void Execute(
        CharacterManager caster,
        CharacterManager target
    );
}
