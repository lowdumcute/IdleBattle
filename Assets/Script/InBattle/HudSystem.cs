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

    [HideInInspector] public CharacterManager owner;

    [Networked] public TypeTeam typeTeam {get; set;}
    public bool isDead;

    // NETWORKED DATA
    [Networked] public float maxHealth { get; set; }
    [Networked] public float maxMana { get; set; }
    [Networked] public float currentHealth { get; set; }
    [Networked] public float currentMana { get; set; }



    public override void Spawned()
    {
        UpdateUI();
        if (owner == null)
            owner = GetComponentInParent<CharacterManager>();

        // ✅ THÊM DÒNG NÀY
        if (owner.Stats.MHealth <= 0)
            owner.Stats.InitializeStats();

        if (Object.HasStateAuthority)
        {
            maxHealth = owner.Stats.MHealth;
            maxMana = owner.Stats.MMana;
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
    }
    public void Update()
    {
        if (Object.HasStateAuthority)
        {
            // H = trừ máu
            if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamage(10f);
                Debug.Log("Trừ 10 máu");
            }

            // G = cộng mana
            if (Input.GetKeyDown(KeyCode.G))
            {
                restoreMana(10);

                Debug.Log("Cộng 10 mana");
            }  
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
    }

    // =========================
    // DAMAGE
    // =========================
    public void TakeDamage(float dmg)
    {
        if (!Object.HasStateAuthority) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);

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

        gameObject.SetActive(false);
    }
}