using System;
using System.Collections;
using UnityEngine;

namespace TestGame.Scripts
{
    public class BackgroundChanging : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            StartCoroutine(ChangingBackground());
        }

        private IEnumerator ChangingBackground()
        {
            while (_spriteRenderer.color.a > 0)
            {
                var spriteRendererColor = _spriteRenderer.color;
                spriteRendererColor.a -= 0.01f;
                _spriteRenderer.color = spriteRendererColor;
                yield return new WaitForSeconds(Constants.BackgroundChangingSpeed);
            }
        }

    }
}