using UnityEngine;
using Fusion;

public class AutoCombatController : NetworkBehaviour
{
    public CharacterManager owner;
    private CharacterManager currentTarget;
    private float attackTimer;
    [SerializeField] private Animator anim;
    private float localAttackCooldown;
    void Awake()
    {
        if (owner == null)
            owner = GetComponent<CharacterManager>();
    }

    public override void Spawned()
    {
        attackTimer = 0f;
    }
    public override void Render()
    {
        if (anim == null || owner == null || owner.Hud.isDead) return;

        // FIX: client tự tìm target
        if (currentTarget == null || currentTarget.Hud.isDead)
        {
            currentTarget = FindNearestEnemy();
        }

        if (currentTarget == null)
        {
            localAttackCooldown = 0f;
            anim.SetBool("Move", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float attackRange = owner.Stats.baseStats.attackRange;

        bool isMovingLocal = distance > attackRange;
        anim.SetBool("Move", isMovingLocal);

        if (!isMovingLocal)
        {
            localAttackCooldown -= Time.deltaTime;

            if (localAttackCooldown <= 0f)
            {
                anim.SetTrigger("Attack");
                localAttackCooldown = 1f / owner.Stats.baseStats.attackSpeed;
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        // CHỈ host/state authority xử lý
        if (!Object.HasStateAuthority) return;

        if (owner == null || owner.Hud.isDead) return;

        // 1. Tìm target nếu chưa có hoặc target chết
        if (currentTarget == null || currentTarget.Hud.isDead)
        {
            currentTarget = FindNearestEnemy();

            if (currentTarget != null)
            {
                // tìm target từ battlemanager
            }
        }

        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        float attackRange = owner.Stats.baseStats.attackRange;

        // 2. Nếu chưa trong range → di chuyển tới
        if (distance > attackRange)
        {
            MoveToTarget(currentTarget.transform.position);
        }
        else
        {
            attackTimer += Runner.DeltaTime;

            float attackRate = owner.Stats.baseStats.attackSpeed;

            if (attackTimer >= 1f / attackRate)
            {
                attackTimer = 0f;
                DoAttack(currentTarget);
            }
        }
    }

    // FIND NEAREST ENEMY
    CharacterManager FindNearestEnemy()
    {
        float minDist = float.MaxValue;
        CharacterManager target = null;

        HudSystem[] all = FindObjectsByType<HudSystem>(FindObjectsSortMode.None);

        foreach (var h in all)
        {
            if (h == null || h == owner.Hud) continue;
            if (h.isDead) continue;

            //  khác team
            if (h.typeTeam == owner.Hud.typeTeam) continue;

            float dist = Vector3.Distance(transform.position, h.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                target = h.GetComponentInParent<CharacterManager>();
            }
        }

        return target;
    }

    // =========================
    // MOVE
    // =========================
    void MoveToTarget(Vector3 targetPos)
    {
        if (owner.Hud.isDead) return;

        float moveSpeedMultiplier = 0.2f;
        Vector3 dir = (targetPos - transform.position);

        if (dir.sqrMagnitude < 0.001f) return;

        float speed = owner.Stats.CSpeed * moveSpeedMultiplier;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Runner.DeltaTime
        );
    }

    // =========================
    // ATTACK
    // =========================
    void DoAttack(CharacterManager target)
    {
        if (target == null) return;
        float atk = owner.Stats.CAttack;
        float def = target.Stats.CDef;

        float damage = Mathf.Max(1f, atk - def);

        if (Random.value < owner.Stats.CCriticalRate)
        {
            damage *= owner.Stats.CCriticalDamage;
        }

        target.Hud.TakeDamage(damage);
    }
}