using System;
using UnityEngine;

namespace TestGame.Scripts
{
    public class EnvironmentMoving : MonoBehaviour
    {
        [SerializeField] private float _movingSpeed;

        private void LateUpdate()
        {
            var position = transform.position;
            transform.position = Vector3.MoveTowards(position,
                position + new Vector3(-_movingSpeed, 0, 0),
                _movingSpeed * Time.deltaTime);
        }
    }
}