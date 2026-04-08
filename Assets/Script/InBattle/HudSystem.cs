using UnityEngine;
using UnityEngine.UI;
using Fusion;
public enum TypeTeam
{
    none,
    Player,
    Enemy
}
public class HudSystem : NetworkBehaviour
{
    [Header("UI")]
    public Image healthFill;
    public Image manaFill;
    public GameObject HUDBar; // chứa health + mana bar
    [Header("Color")]
    public Color friendlyColor = Color.green;
    public Color enemyColor = Color.red;

    [HideInInspector] public CharacterManager owner;

    [Networked] public TypeTeam typeTeam {get; set;}
    private TypeTeam lastTeam = TypeTeam.none;
    [Networked] public NetworkBool isDead { get; set; }

    // NETWORKED DATA
    [Networked] public float maxHealth { get; set; }
    [Networked] public float maxMana { get; set; }
    [Networked] public float currentHealth { get; set; }
    [Networked] public float currentMana { get; set; }
    [Networked] private NetworkBool damagedFlag { get; set; }
    [Networked] private NetworkBool deathFlag { get; set; }
    private Animator anim;


    public override void Spawned()
    {
        UpdateUI();
        if (owner == null)
            owner = GetComponentInParent<CharacterManager>();

        // THÊM DÒNG NÀY
        if (owner.Stats.MHealth <= 0)
            owner.Stats.InitializeStats();

        if (Object.HasStateAuthority)
        {
            maxHealth = owner.Stats.MHealth;
            maxMana = owner.Stats.MMana;
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        UpdateDirection();
        anim = GetComponent<Animator>();
    }
    public override void Render()
    {
        if (anim == null) return;

        if (damagedFlag)
        {
            anim.SetTrigger("Damaged");

            if (Object.HasStateAuthority)
                damagedFlag = false;
        }

        if (deathFlag)
        {
            anim.SetTrigger("Death");
            HUDBar.SetActive(false);
            if (Object.HasStateAuthority)
                deathFlag = false; 
        }
    }
    public void Update()
    {
        if (Object.HasStateAuthority)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamage(10f);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                restoreMana(10);
            }  
        }

        // detect change
        if (lastTeam != typeTeam)
        {
            lastTeam = typeTeam;
            UpdateDirection();
        }

        UpdateUI();
    }

    // =========================
    // UPDATE UI
    // =========================
    void UpdateUI()
    {
        if (healthFill != null && maxHealth > 0)
            healthFill.fillAmount = currentHealth / maxHealth;

        if (manaFill != null && maxMana > 0)
            manaFill.fillAmount = currentMana / maxMana;
        UpdateColor();
    }
    void UpdateColor()
    {
        if (healthFill == null) return;

        int localId = Runner.LocalPlayer.PlayerId;

        bool isFriendly;

        if (localId == 1)
        {
            // Player 1 (host)
            isFriendly = (typeTeam == TypeTeam.Player);
        }
        else
        {
            // Player 2 (client)
            isFriendly = (typeTeam == TypeTeam.Enemy);
        }

        healthFill.color = isFriendly ? friendlyColor : enemyColor;
    }
    void UpdateDirection()
    {

        var rect = GetComponent<Transform>();
        if (rect == null) return;

        Vector3 scale = rect.localScale;

        if (typeTeam == TypeTeam.Player)
            scale.x = -Mathf.Abs(scale.x); // quay trái
        else
            scale.x = Mathf.Abs(scale.x);  // quay phải

        rect.localScale = scale;
    }
    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(float dmg)
    {
        if (!Object.HasStateAuthority) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);

        damagedFlag = true; // sync cho client

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // =========================
    // USE MANA
    // =========================
    public void UseMana(float amount)
    {
        if (!Object.HasStateAuthority) return;

        currentMana -= amount;
        currentMana = Mathf.Min(currentMana, maxMana);
    }

    public void restoreMana(float amount)
    {
        if (!Object.HasStateAuthority) return;
        currentMana += amount;
        currentMana = Mathf.Max(currentMana, 0);
    }
    // =========================
    // HEAL
    // =========================
    public void Heal(float amount)
    {
        if (!Object.HasStateAuthority) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    // =========================
    // DEATH
    // =========================
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        deathFlag = true; // sync animation

    }
}