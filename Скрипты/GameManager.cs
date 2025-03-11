using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerMoney = 100; // Начальные деньги
    public List<GameObject> monsters; // Список всех монстров в игре
    public List<GameObject> npcs; // Список всех NPC в игре

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Удалена инициализация пула лутбоксов, так как система лута больше не используется
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        Debug.Log("Деньги: " + playerMoney);
    }

    public void RemoveMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            Debug.Log("Деньги: " + playerMoney);
        }
        else
        {
            Debug.Log("Недостаточно денег!");
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            playerMoney = data.money;
            Debug.Log("Прогресс загружен!");
        }
    }
}
