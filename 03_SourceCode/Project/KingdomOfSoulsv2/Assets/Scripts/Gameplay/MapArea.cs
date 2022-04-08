using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> enemies;

    public Unit GetRandomEnemy()
    {
        var enemy = enemies[Random.Range(0, enemies.Count)];
        enemy.Init();
        return enemy;
    }
}
