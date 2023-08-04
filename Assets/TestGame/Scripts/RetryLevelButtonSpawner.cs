using System;
using UnityEngine;
using UnityEngine.Events;

namespace TestGame.Scripts
{
    public class RetryLevelButtonSpawner : MonoBehaviour
    {
        private const string Player = "Player";
        
        [SerializeField] private GameObject _buttonPrefab;
        
        private PlayerController _player;

        private void Awake()
        {
            _player = GameObject.FindWithTag(Player).GetComponent<PlayerController>();
        }

        private void Start()
        {
            _player.SubscribeOnLoose(CreateButton);
            _player.SubscribeOnWin(CreateButton);
        }

        private void CreateButton()
        {
            Instantiate(_buttonPrefab, transform);
        }
    }
}