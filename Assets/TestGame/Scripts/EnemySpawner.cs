using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestGame.Scripts
{
    public class EnemySpawner : MonoBehaviour
    {
        private const string EnemySpawnPoint = "EnemySpawnPoint";
        private const string EnemiesRoot = "EnemiesRoot";
        private const string Player = "Player";

        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private float _spawnDelayAverage;

        private bool _isSpawnActive = true;

        private Transform _enemySpawnPoint;
        private GameObject _enemiesRoot;

        private PlayerController _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag(Player).GetComponent<PlayerController>();
            _enemySpawnPoint = GameObject.FindWithTag(EnemySpawnPoint).transform;
            _enemiesRoot = GameObject.FindWithTag(EnemiesRoot);
        }

        private void Start()
        {
            _player.SubscribeOnLoose(StopSpawn);
            _player.SubscribeOnWin(StopSpawn);
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (_isSpawnActive)
            {
                Instantiate(_enemyPrefab, _enemySpawnPoint.position, _enemyPrefab.transform.rotation, _enemiesRoot.transform);
                var spawnDelay = Random.Range(_spawnDelayAverage - _spawnDelayAverage / 2, _spawnDelayAverage + _spawnDelayAverage / 2);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        public void StopSpawn()
        {
            _isSpawnActive = false;
        }
    }
}