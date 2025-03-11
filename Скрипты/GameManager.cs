using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerMoney = 100; // ��������� ������
    public List<GameObject> monsters; // ������ ���� �������� � ����
    public List<GameObject> npcs; // ������ ���� NPC � ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // ������� ������������� ���� ���������, ��� ��� ������� ���� ������ �� ������������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        Debug.Log("������: " + playerMoney);
    }

    public void RemoveMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            Debug.Log("������: " + playerMoney);
        }
        else
        {
            Debug.Log("������������ �����!");
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
            Debug.Log("�������� ��������!");
        }
    }
}
