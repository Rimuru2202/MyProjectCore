using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private List<SpawnPoint> spawnPoints;

    void Awake()
    {
        Instance = this;
        spawnPoints = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).ToList();
    }

    // Получить точку спавна по индексу
    public SpawnPoint GetSpawnPointByIndex(int index)
    {
        return spawnPoints.FirstOrDefault(sp => sp.spawnIndex == index);
    }

    // Получить ближайшую точку спавна от заданной позиции
    public SpawnPoint GetNearestSpawnPoint(Vector3 position)
    {
        SpawnPoint nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var sp in spawnPoints)
        {
            float dist = Vector3.Distance(position, sp.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = sp;
            }
        }
        return nearest;
    }
}
