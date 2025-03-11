using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("Префаб врага, который будет появляться на этой точке спавна")]
    public GameObject enemyPrefab;
    [Tooltip("Время (в секундах) до возрождения врага после его смерти")]
    public float respawnTime = 15f;

    // Ссылка на текущего врага, созданного этой точкой спавна
    private GameObject currentEnemy;

    void Start()
    {
        SpawnEnemy();
    }

    // Метод создания врага
    public void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            currentEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            // Добавляем компонент-маркер для связи с этой точкой спавна
            EnemySpawnMarker marker = currentEnemy.AddComponent<EnemySpawnMarker>();
            marker.spawnPoint = this;
        }
    }

    // Метод, вызываемый из EnemySpawnMarker при смерти врага
    public void OnEnemyDeath()
    {
        currentEnemy = null;
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnEnemy();
    }
}
