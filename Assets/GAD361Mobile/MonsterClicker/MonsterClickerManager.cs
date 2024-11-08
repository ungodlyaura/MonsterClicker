using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class MonsterClick : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI goldText;

    public static int totalXP;
    public static int totalGold;

    private const float timeToKill = 10;
    private float killTimer;

    public Weapon meleeWeapon;
    public Weapon magicWeapon;
    public Weapon spiritWeapon;
    private Weapon currentWeapon = null;

    public Button meleeButton;
    public Button magicButton;
    public Button spiritButton;
    public Sprite unequippedSprite;
    public Sprite equippedSprite;

    public enum MonsterAttribute { Melee, Magic, Spirit }
    public MonsterAttribute currentAttribute;
    public Sprite meleeSprite;
    public Sprite magicSprite;
    public Sprite spiritSprite;
    public Image attributeImage;
    public Sprite ghostSprite;
    public Sprite goblinSprite;
    public Sprite golemSprite;
    public Sprite slimeSprite;
    public Image monsterImage;

    public bool burstClickerUnlocked = false;
    private int burstClickCount = 0;
    private float burstClickTimer = 2.0f;

    public bool weaknessCursePurchased = false;
    public bool magePurchased = false;
    public bool warriorPurchased = false;
    public bool spiritualistPurchased = false;
    public int mageLevel = 0;
    public int warriorLevel = 0;
    public int spiritualistLevel = 0;
    public int clickLevel = 0;


    void Start()
    {
        Load();
        currentHealth = maxHealth;
        killTimer = 0f;
        monsterImage.sprite = slimeSprite;

        UpdateUI();
        AssignRandomAttribute();
        SetupWeaponButtons();
        UpdateWeaponButtonSprites();
        StartCoroutine(ApplyAutomatedDamage());

        FindObjectOfType<ShopUIManager>().Setup();
    }

    public void AssignRandomAttribute()
    {
        currentAttribute = (MonsterAttribute)Random.Range(0, 3);
        Debug.Log($"Monster attribute set to: {currentAttribute}");
        switch (currentAttribute)
        {
            case MonsterAttribute.Melee:
                monsterImage.sprite = goblinSprite;
                attributeImage.sprite = meleeSprite;
                break;
            case MonsterAttribute.Magic:
                monsterImage.sprite = golemSprite;
                attributeImage.sprite = magicSprite;
                break;
            case MonsterAttribute.Spirit:
                monsterImage.sprite = ghostSprite;
                attributeImage.sprite = spiritSprite;
                break;
        }

        if (weaknessCursePurchased)
        {
            attributeImage.gameObject.SetActive(true);
        }
        else
        {
            monsterImage.sprite = slimeSprite;
            attributeImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        killTimer += Time.deltaTime;

        if (burstClickerUnlocked && burstClickCount > 0)
        {
            burstClickTimer -= Time.deltaTime;
            if (burstClickTimer <= 0)
            {
                int baseDamage = currentWeapon != null ? currentWeapon.damage : 1;

                DealDamage((Mathf.Min(burstClickCount * burstClickCount, 25))* baseDamage, "Burst");
                burstClickCount = 0;
                burstClickTimer = 2f;
            }
        }
    }

    private void DealDamage(int damage, string attribute)
    {
        if (attribute == currentAttribute.ToString() && weaknessCursePurchased)
        {
            Debug.Log(attribute);
            Debug.Log(currentAttribute.ToString());
            damage = Mathf.RoundToInt(damage * 2f);
        }

        currentHealth -= damage;
        totalXP += damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GiveRewards();
            Save();
        }
        UpdateUI();
    }

    public void OnClick()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSources[1].time = 0.4f;
        audioSources[1].Play();
        int baseDamage = currentWeapon != null ? currentWeapon.damage : 1;
        baseDamage = baseDamage + clickLevel;
        if (currentWeapon == meleeWeapon)
        {
            DealDamage(baseDamage, "Melee");
        }
        else if (currentWeapon == magicWeapon)
        {
            DealDamage(baseDamage, "Magic");
        }
        else if (currentWeapon == spiritWeapon)
        {
            DealDamage(baseDamage, "Spirit");
        }
        else
        {
            DealDamage(baseDamage, "none");
        }


        if (burstClickerUnlocked)
        {
            burstClickCount++;
            Debug.Log($"Burst click count: {burstClickCount}");
        }
    }

    private IEnumerator ApplyAutomatedDamage()
    {
        while (true)
        {
            if (magePurchased)
            {
                DealDamage(1 + mageLevel, "Magic");
            }

            if (warriorPurchased)
            {
                DealDamage(1 + warriorLevel, "Melee");
            }

            if (spiritualistPurchased)
            {
                DealDamage(1 + spiritualistLevel, "Spirit");
            }

            UpdateUI();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void UpdateUI()
    {
        healthText.text = $"{FormatNumber(currentHealth)}/{FormatNumber(maxHealth)}";
        xpText.text = $"{FormatNumber(totalXP)}";
        goldText.text = $"{FormatNumber(totalGold)}";
    }

    private string FormatNumber(float value)
    {
        if (value >= 10_000_000)
            return $"{(value / 1_000_000f):0.0}why are you still playing this game";
        if (value >= 1_000_000)
            return $"{(value / 1_000_000f):0.00}m";
        if (value >= 100_000)
            return $"{(value / 1_000f):0}k";
        if (value >= 10_000)
            return $"{(value / 1_000f):0.0}k";
        if (value >= 1_000) 
            return $"{(value / 1_000f):0.00}k";

        return value.ToString("0");
    }


    private void GiveRewards()
    {
        int goldGained = (maxHealth / 100) * (maxHealth / 100);
        totalGold += goldGained;
        UpdateUI();
        Debug.Log($"Gold gained: {goldGained}");

        if (killTimer < timeToKill)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSources[0].time = 0.1f;
            audioSources[0].Play();
            SetNextMonsterMaxHealth();
            AssignRandomAttribute();
        }

        ResetHealth();
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
        killTimer = 0f;
    }

    private void SetNextMonsterMaxHealth()
    {
        maxHealth = Mathf.RoundToInt(((maxHealth + 100) * 1.25f / 10)) * 10;
        Debug.Log($"Next monster's max health set to: {maxHealth}");
    }

    private void SetupWeaponButtons()
    {
        ConfigureWeaponButton(meleeButton, meleeWeapon);
        ConfigureWeaponButton(magicButton, magicWeapon);
        ConfigureWeaponButton(spiritButton, spiritWeapon);
    }

    private void ConfigureWeaponButton(Button button, Weapon weapon)
    {
        bool hasWeapon = weapon != null && weapon.damage > 0;
        button.gameObject.SetActive(hasWeapon);
        button.onClick.AddListener(() => ToggleWeapon(weapon));
    }

    private void ToggleWeapon(Weapon weapon)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSources[2].time = 0.2f;
        audioSources[2].Play();
        currentWeapon = (currentWeapon == weapon) ? null : weapon;
        UpdateWeaponButtonSprites();
    }

    private void UpdateWeaponButtonSprites()
    {
        meleeButton.image.sprite = (currentWeapon == meleeWeapon) ? equippedSprite : unequippedSprite;
        magicButton.image.sprite = (currentWeapon == magicWeapon) ? equippedSprite : unequippedSprite;
        spiritButton.image.sprite = (currentWeapon == spiritWeapon) ? equippedSprite : unequippedSprite;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("MaxHealth", maxHealth);
        PlayerPrefs.SetInt("TotalXP", totalXP);
        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.SetInt("BurstClickerUnlocked", burstClickerUnlocked ? 1 : 0);
        PlayerPrefs.SetInt("WeaknessCursePurchased", weaknessCursePurchased ? 1 : 0);
        PlayerPrefs.SetInt("MagePurchased", magePurchased ? 1 : 0);
        PlayerPrefs.SetInt("WarriorPurchased", warriorPurchased ? 1 : 0);
        PlayerPrefs.SetInt("SpiritualistPurchased", spiritualistPurchased ? 1 : 0);
        PlayerPrefs.SetInt("MageLevel", mageLevel);
        PlayerPrefs.SetInt("WarriorLevel", warriorLevel);
        PlayerPrefs.SetInt("SpiritualistLevel", spiritualistLevel);
        PlayerPrefs.SetInt("ClickLevel", clickLevel);

        SaveWeapon("MeleeWeapon", meleeWeapon);
        SaveWeapon("MagicWeapon", magicWeapon);
        SaveWeapon("SpiritWeapon", spiritWeapon);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("MaxHealth"))
        {
            maxHealth = PlayerPrefs.GetInt("MaxHealth");
            totalXP = PlayerPrefs.GetInt("TotalXP");
            totalGold = PlayerPrefs.GetInt("TotalGold");
            burstClickerUnlocked = PlayerPrefs.GetInt("BurstClickerUnlocked") == 1;
            weaknessCursePurchased = PlayerPrefs.GetInt("WeaknessCursePurchased") == 1;
            magePurchased = PlayerPrefs.GetInt("MagePurchased") == 1;
            warriorPurchased = PlayerPrefs.GetInt("WarriorPurchased") == 1;
            spiritualistPurchased = PlayerPrefs.GetInt("SpiritualistPurchased") == 1;
            mageLevel = PlayerPrefs.GetInt("MageLevel");
            warriorLevel = PlayerPrefs.GetInt("WarriorLevel");
            spiritualistLevel = PlayerPrefs.GetInt("SpiritualistLevel");
            clickLevel = PlayerPrefs.GetInt("ClickLevel");

            meleeWeapon = LoadWeapon("MeleeWeapon");
            magicWeapon = LoadWeapon("MagicWeapon");
            spiritWeapon = LoadWeapon("SpiritWeapon");
        }
    }
    private void SaveWeapon(string keyPrefix, Weapon weapon)
    {
        PlayerPrefs.SetInt($"{keyPrefix}_Rarity", weapon.rarity);
        PlayerPrefs.SetInt($"{keyPrefix}_Damage", weapon.damage);
    }

    private Weapon LoadWeapon(string keyPrefix)
    {
        int rarity = PlayerPrefs.GetInt($"{keyPrefix}_Rarity", 0);
        int damage = PlayerPrefs.GetInt($"{keyPrefix}_Damage", 0);
        return new Weapon(rarity, damage);
    }

    public void ResetStats()
    {
        PlayerPrefs.SetInt("MaxHealth", 100);
        PlayerPrefs.SetInt("TotalXP", 0);
        PlayerPrefs.SetInt("TotalGold", 0);
        PlayerPrefs.SetInt("BurstClickerUnlocked", 0);
        PlayerPrefs.SetInt("WeaknessCursePurchased", 0);
        PlayerPrefs.SetInt("MagePurchased", 0);
        PlayerPrefs.SetInt("WarriorPurchased", 0);
        PlayerPrefs.SetInt("SpiritualistPurchased", 0);
        PlayerPrefs.SetInt("MageLevel", 0);
        PlayerPrefs.SetInt("WarriorLevel", 0);
        PlayerPrefs.SetInt("SpiritualistLevel", 0);
        PlayerPrefs.SetInt("ClickLevel", 0);

        SaveWeapon("MeleeWeapon", meleeWeapon);
        SaveWeapon("MagicWeapon", magicWeapon);
        SaveWeapon("SpiritWeapon", spiritWeapon);

        PlayerPrefs.SetInt("MeleeWeapon_Rarity", 0);
        PlayerPrefs.SetInt("MeleeWeapon_Damage", 0);
        PlayerPrefs.SetInt("MagicWeapon_Rarity", 0);
        PlayerPrefs.SetInt("MagicWeapon_Damage", 0);
        PlayerPrefs.SetInt("SpiritWeapon_Rarity", 0);
        PlayerPrefs.SetInt("SpiritWeapon_Damage", 0);

        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
