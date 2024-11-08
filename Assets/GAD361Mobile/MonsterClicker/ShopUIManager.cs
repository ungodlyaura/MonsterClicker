using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopUIManager : MonoBehaviour
{
    private MonsterClick monsterClick;
    public GameObject shopPanel;
    public Button openShopButton;
    public Button closeShopButton;

    public Button shopTabButton;
    public Button upgradeTabButton;
    public Button extraTabButton;

    public GameObject shopTabPanel;
    public GameObject upgradeTabPanel;
    public GameObject extraTabPanel;

    public GameObject shopItemPrefab;
    public Transform shopContent;
    public Transform upgradeContent;
    public Transform extraContent;

    public TextMeshProUGUI itemDetailText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI xpText;

    public Sprite burstClickSprite;
    public Sprite curseWeakSprite;
    public Sprite idlePlaySprite;
    public Sprite chestT1Sprite;
    public Sprite chestT2Sprite;
    public Sprite chestT3Sprite;
    public Sprite warriorSprite;
    public Sprite mageSprite;
    public Sprite spiritualistSprite;
    public Sprite mouseSprite;
    public Sprite goldSprite;
    public Sprite xpSprite;

    public Button openSettingsButton;
    public Button closeSettingsButton;
    public Button resetStatsButton;
    public GameObject settingsPanel;
    public Button closeGameButton;
    

    public void Setup()
    {
        monsterClick = FindObjectOfType<MonsterClick>();
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);

        openShopButton.onClick.AddListener(OpenShop);
        closeShopButton.onClick.AddListener(CloseShop);
        shopTabButton.onClick.AddListener(() => ShowTab(shopTabPanel));
        upgradeTabButton.onClick.AddListener(() => ShowTab(upgradeTabPanel));
        extraTabButton.onClick.AddListener(() => ShowTab(extraTabPanel));

        openSettingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        closeGameButton.onClick.AddListener(ExitGame);
        resetStatsButton.onClick.AddListener(monsterClick.ResetStats);

        ShowTab(shopTabPanel);
        InitializeShopItems();
    }

    void ExitGame() {
        monsterClick.Save();
        Application.Quit(); 
    }

    private void InitializeShopItems()
    {
        CreateShopItem("Discounted T1 Chest", "A one-time purchase chest for random Tier 1 crate", chestT1Sprite, 5, "Gold", false);
        CreateShopItem("Warrior", "Deals melee damage every second", warriorSprite, 10, "Gold", false);
        CreateShopItem("Mage", "Deals magic damage every second", mageSprite, 10, "Gold", false);
        CreateShopItem("Spiritualist", "Deals spirit damage every second", spiritualistSprite, 10, "Gold", false);
        CreateShopItem("Burst Clicker", "Exponential increase to damage depending on click speed", burstClickSprite, 50, "Gold", false);
        CreateShopItem("Weakness Curse", "Enemies now have a weakness, match that weakness for extra damage", curseWeakSprite, 75, "Gold", false);
        CreateShopItem("Idle Play", "Game auto plays generating XP and Gold while not playing", idlePlaySprite, 100, "Gold", false);
        CreateShopItem("T1 Chest", "Contains a random Tier 1 chest", chestT1Sprite, 100, "Gold", true);
        CreateShopItem("T2 Chest", "Contains a random Tier 2 chest", chestT2Sprite, 1000, "Gold", true);
        CreateShopItem("T3 Chest", "Contains a random Tier 3 chest", chestT3Sprite, 100000, "Gold", true);

        CreateShopItem("Upgrade Click", "Increases base damage of clicking + 1, adds onto weapon damage", mouseSprite, 100, "XP", true);
        if (monsterClick.warriorPurchased)
        { 
            CreateShopItem("Upgrade Warrior", "Increases base damage of warrior + 1", warriorSprite, 50, "XP", true); 
        }
        if (monsterClick.magePurchased)
        {
            CreateShopItem("Upgrade Mage", "Increases base damage of mage + 1", mageSprite, 50, "XP", true);
        }
        if (monsterClick.spiritualistPurchased)
        {
            CreateShopItem("Upgrade Spiritualist", "Increases base damage of spiritualist + 1", spiritualistSprite, 50, "XP", true);
        }

        CreateShopItem("Gold Pack", "Gives Gold based on current monsters health (health*health)*50", goldSprite, 3, "Money", true);
        CreateShopItem("XP Pack", "Gives XP based on current monsters health (health*50)", xpSprite, 3, "Money", true);
    }

    public void CreateShopItem(string itemName, string description, Sprite itemSprite, int cost, string currency, bool isInfinitePurchase)
    {
        GameObject newItem = null;
        ShopItem shopItem = null;

        switch (currency)
        {
            case "Gold":
                newItem = Instantiate(shopItemPrefab, shopContent);
                shopItem = newItem.GetComponent<ShopItem>();
                break;

            case "XP":
                newItem = Instantiate(shopItemPrefab, upgradeContent);
                shopItem = newItem.GetComponent<ShopItem>();
                break;

            case "Money":
                newItem = Instantiate(shopItemPrefab, extraContent);
                shopItem = newItem.GetComponent<ShopItem>();
                break;

            default:
                Debug.LogWarning("Invalid currency type specified");
                return;
        }

        if (newItem != null && shopItem != null)
        {
            shopItem.Initialize(itemName, description, itemSprite, cost, currency, isInfinitePurchase, this);
        }
    }


    public void UpdateItemDetails(string itemName, string description, int cost, string currency)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSources[0].Play();
        itemDetailText.text = $"{itemName} - {description}";
        costText.text = $"{cost} {currency}";
    }

    public void SetBuyButtonAction(System.Action buyAction)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => buyAction());
    }

    private void OpenShop()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        shopPanel.SetActive(true);
    }

    private void CloseShop()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        shopPanel.SetActive(false);
    }

    private void OpenSettings()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        settingsPanel.SetActive(false);
    }

    private void ShowTab(GameObject tabPanel)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        shopTabPanel.SetActive(false);
        upgradeTabPanel.SetActive(false);
        extraTabPanel.SetActive(false);
        tabPanel.SetActive(true);
    }

    public void PurchaseSound()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSources[1].time = 0.2f;
        audioSources[1].Play();
    }
}
