using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    private string itemName;
    private string itemDescription;
    private int itemCost;
    private string currencyType;
    private bool isInfinitePurchase;
    private ShopUIManager shopUIManager;

    private MonsterClick monsterClick;

    public void Initialize(string name, string description, Sprite itemSprite, int cost, string currency, bool isInfinite, ShopUIManager manager)
    {
        monsterClick = FindObjectOfType<MonsterClick>();
        
        cost = GetLeveledPrice(itemName, cost, isInfinite);

        itemName = name;
        itemNameText.text = itemName;
        itemDescription = description;
        itemImage.sprite = itemSprite;
        itemCost = cost;
        currencyType = currency;
        isInfinitePurchase = isInfinite;
        shopUIManager = manager;

        GetComponent<Button>().onClick.AddListener(() => OnItemSelected());
    }

    private int GetLeveledPrice(string itemName, int cost, bool isInfinite)
    {
        if (isInfinite)
        {
            switch (itemName)
            {
                case "Upgrade Click":
                    cost = 100 * (1 + monsterClick.clickLevel);
                    break;
                case "Upgrade Warrior":
                    cost = 50 * (1 + monsterClick.warriorLevel);
                    break;
                case "Upgrade Mage":
                    cost = 50 * (1 + monsterClick.mageLevel);
                    break;
                case "Upgrade Spiritualist":
                    cost = 50 * (1 + monsterClick.spiritualistLevel);
                    break;
                default:
                    break;
            }

        }
        return cost;
    }

    private void OnItemSelected()
    {
        shopUIManager.UpdateItemDetails(itemName, itemDescription, itemCost, currencyType);
        shopUIManager.SetBuyButtonAction(() => OnBuyButtonClicked());
    }

    private void OnBuyButtonClicked()
    {
        if (currencyType == "Gold" && MonsterClick.totalGold >= itemCost)
        {
            MonsterClick.totalGold -= itemCost;
            Debug.Log($"{itemName} bought for {itemCost} Gold");
            PerformPurchase(itemName);
        }
        else if (currencyType == "XP" && MonsterClick.totalXP >= itemCost)
        {
            MonsterClick.totalXP -= itemCost;
            Debug.Log($"{itemName} bought for {itemCost} XP");
            PerformPurchase(itemName);
            itemCost = GetLeveledPrice(itemName, itemCost, isInfinitePurchase);
            shopUIManager.UpdateItemDetails(itemName, itemDescription, itemCost, currencyType);
        }
        else if (currencyType == "Money")
        {
            Debug.Log($"{itemName} bought for {itemCost}$");
            PerformPurchase(itemName);
        }
        else
        {
            Debug.Log("Not enough currency");
        }
    }

    private void PerformPurchase(string itemName)
    {
        shopUIManager.PurchaseSound();
        switch (itemName)
        {
            case "Burst Clicker":
                monsterClick.burstClickerUnlocked = true; // Set the relevant variable in MonsterClick
                break;
            case "Weakness Curse":
                monsterClick.weaknessCursePurchased = true; // Set the relevant variable in MonsterClick
                monsterClick.AssignRandomAttribute();
                break;
            case "Mage":
                shopUIManager.CreateShopItem("Upgrade Mage", "Increases base damage of mage + 1", shopUIManager.mageSprite, 50, "XP", true);
                monsterClick.magePurchased = true; // Set the relevant variable in MonsterClick
                break;
            case "Warrior":
                shopUIManager.CreateShopItem("Upgrade Warrior", "Increases base damage of warrior + 1", shopUIManager.warriorSprite, 50, "XP", true);
                monsterClick.warriorPurchased = true; // Set the relevant variable in MonsterClick
                break;
            case "Spiritualist":
                shopUIManager.CreateShopItem("Upgrade Spiritualist", "Increases base damage of spiritualist + 1", shopUIManager.spiritualistSprite, 50, "XP", true);
                monsterClick.spiritualistPurchased = true; // Set the relevant variable in MonsterClick
                break;
            case "Discounted T1 Chest":
                BuyChest(1); // Call the T1 chest method
                break;
            case "T1 Chest":
                BuyChest(1); // Call the T1 chest method
                break;
            case "T2 Chest":
                BuyChest(2); // Call the T1 chest method
                break;
            case "T3 Chest":
                BuyChest(3); // Call the T1 chest method
                break;
            case "Upgrade Click":
                monsterClick.clickLevel += 1;
                break;
            case "Upgrade Warrior":
                monsterClick.warriorLevel += 1;
                break;
            case "Upgrade Mage":
                monsterClick.mageLevel += 1;
                break;
            case "Upgrade Spiritualist":
                monsterClick.spiritualistLevel += 1;
                break;
            case "Gold Pack":
                MonsterClick.totalGold += (monsterClick.maxHealth/100) * (monsterClick.maxHealth/100)*10;
                break;
            case "XP Pack":
                MonsterClick.totalXP += monsterClick.maxHealth * 10;
                break;
            default:
                Debug.Log("Unknown item");
                break;
        }

        monsterClick.UpdateUI();

        if (!isInfinitePurchase)
        {
            Destroy(gameObject);
        }

    }

    public void BuyChest(int tier)
    {
        int rarity = GetChestRarity(tier);
        int newWeaponDamage = 1 + Mathf.FloorToInt((monsterClick.maxHealth / 200) * rarity);
        
        Debug.Log("a");
        switch ((MonsterClick.MonsterAttribute)Random.Range(0, 3))
        {
            case MonsterClick.MonsterAttribute.Melee:
                if (newWeaponDamage > monsterClick.meleeWeapon.damage)
                {
                    Debug.Log("melee");
                    monsterClick.meleeWeapon.rarity = rarity;
                    monsterClick.meleeWeapon.damage = newWeaponDamage;
                    monsterClick.meleeButton.gameObject.SetActive(true);
                }
                break;
            case MonsterClick.MonsterAttribute.Magic:
                if (newWeaponDamage > monsterClick.magicWeapon.damage)
                {
                    Debug.Log("magic");
                    monsterClick.magicWeapon.rarity = rarity;
                    monsterClick.magicWeapon.damage = newWeaponDamage;
                    monsterClick.magicButton.gameObject.SetActive(true);
                }
                break;
            case MonsterClick.MonsterAttribute.Spirit:
                if (newWeaponDamage > monsterClick.spiritWeapon.damage)
                {
                    Debug.Log("spirit");
                    monsterClick.spiritWeapon.rarity = rarity;
                    monsterClick.spiritWeapon.damage = newWeaponDamage;
                    monsterClick.spiritButton.gameObject.SetActive(true);
                }
                break;
        }

    }

    private int GetChestRarity(int rarity)
    {
        float roll = Random.Range(0f, 100f);
        if (roll < 0.1f * (rarity*rarity*rarity)) return 4; // Legendary
        else if (roll < 4.9f * (rarity * rarity * rarity)) return 3; // Exotic
        else if (roll < 14.5f * (rarity * rarity * rarity)) return 2; // Rare
        else if (roll < 34.5f * (rarity * rarity * rarity)) return 1; // Uncommon
        else return 0; // Common
    }
}
