using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("������ �����, ������� ����� ���������� �� ���� ����� ������")]
    public GameObject enemyPrefab;
    [Tooltip("����� (� ��������) �� ����������� ����� ����� ��� ������")]
    public float respawnTime = 15f;

    // ������ �� �������� �����, ���������� ���� ������ ������
    private GameObject currentEnemy;

    void Start()
    {
        SpawnEnemy();
    }

    // ����� �������� �����
    public void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            currentEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            // ��������� ���������-������ ��� ����� � ���� ������ ������
            EnemySpawnMarker marker = currentEnemy.AddComponent<EnemySpawnMarker>();
            marker.spawnPoint = this;
        }
    }

    // �����, ���������� �� EnemySpawnMarker ��� ������ �����
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
