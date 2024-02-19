using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damagable : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxShield;
    [SerializeField] private float currentShield;

    [SerializeField] private Color HealthColor;
    [SerializeField] private Color ShieldColor;
    [SerializeField] private Color BackHealthColor;

    private float remainingDamage;

    [Header("Loot")]
    private int ItemInLoot;
    [SerializeField] private int MaxLootItem;
    private ItemInfo[] ItemIntoToLoot;

    [Header("UI")]
    [SerializeField] private Image HeatlhUI;
    [SerializeField] private Image HealtUnderUI;

    private enum ObjectType
    {
        Humanoid,
        Item,
        Player
    }

    [SerializeField] private ObjectType type;

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        if (type == ObjectType.Player)
        {
            UpdateUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(5f);
        }
    }


    public void TakeDamage(float damage)
    {
        if (currentShield > 0)
        {
            remainingDamage = Mathf.Max(damage - currentShield, 0f);
            currentShield = Mathf.Max(currentShield - damage, 0f);
            currentHealth -= remainingDamage;
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            // Jeœli zdrowie spad³o do zera lub poni¿ej, wykonaj odpowiednie czynnoœci (np. zniszcz obiekt)
            Death();
        }

        if (type == ObjectType.Player)
        {
            UpdateUI();
        }
    }


    // CA£E UI DO OSOBNEGO SKRYPTU

    private void UpdateUI()
    {
        if (currentShield > 0)
        {
            HeatlhUI.fillAmount = currentShield / maxShield;
            HeatlhUI.color = ShieldColor;
            HealtUnderUI.color = HealthColor;
        }
        else
        {
            HeatlhUI.fillAmount = currentHealth / maxHealth;
            HeatlhUI.color = HealthColor;
            HealtUnderUI.color = BackHealthColor;
        }
    }

    private void Death()
    {
        switch (type)
        {
            case ObjectType.Humanoid:
                {
                    ItemInLoot = Random.Range(0, MaxLootItem);


                    break;
                }
            case ObjectType.Item:
                {

                    break;
                }
            case ObjectType.Player:
                {

                    break;
                }
        }
    }
}
