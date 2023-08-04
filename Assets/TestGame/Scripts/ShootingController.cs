using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    private const string Player = "Player";
    private const string Enemy = "Enemy";

    private PlayerController _player;

    private void Start()
    {
        _player = GameObject.FindWithTag(Player).GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_player.IsWin || _player.IsLoose) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.transform.tag.Equals(Enemy))
            {
                hit.transform.GetComponent<EnemyController>().OnLoose();
                _player.OnSuccessShot();
            }
            else
            {
                _player.OnFailedShot();
            }
        }
    }
}
