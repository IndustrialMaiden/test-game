using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace TestGame.Scripts
{
    public class BackgroundLoop : MonoBehaviour
    {
        [SerializeField] private GameObject[] _layers;
        [SerializeField] private GameObject _environmentParentPrefab;
        [SerializeField] private float _offset;
        
        private GameObject _environmentRoot;
        private Camera _mainCamera;
        private Vector2 _screenBounds;

        private List<GameObject> _parents;

        private void Start()
        {
            _mainCamera = Camera.main;
            _screenBounds = _mainCamera.ScreenToWorldPoint(
                    new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));

            _parents = new List<GameObject>();
            
            foreach (var layer in _layers)
            {
                ConstructBackground(layer);
            }
        }

        private void ConstructBackground(GameObject layer)
        {
            float levelWidth = layer.GetComponentInChildren<SpriteRenderer>().bounds.size.x - _offset;
            int childsNeeded = (int) Mathf.Ceil(-_screenBounds.x * 2 / levelWidth);

            GameObject parent = Instantiate(_environmentParentPrefab);
            parent.name += layer.name;
            
            _parents.Add(parent);

            GameObject clone = Instantiate(layer);
            for (int i = 0; i <= childsNeeded; i++)
            {
                GameObject c = Instantiate(clone, parent.transform);

                Vector3 layerPos = layer.transform.position;
                c.transform.position = new Vector3(levelWidth * i, layerPos.y, layerPos.z);
                c.name = layer.name + i;
            }
            
            Destroy(clone);
        }

        private void LateUpdate()
        {
            foreach (var parent in _parents)
            {
                RepositionLayers(parent);
            }
        }

        private void RepositionLayers(GameObject parent)
        {
            Transform[] childrens = parent.GetComponentsInChildren<Transform>();
                
            if (childrens.Length > 1)
            {
                GameObject firstChild = childrens[1].gameObject;
                GameObject lastChild = childrens[^1].gameObject;

                float halfObjectWidth = lastChild.GetComponent<SpriteRenderer>().bounds.extents.x - _offset;
                if (_mainCamera.transform.position.x - _screenBounds.x > lastChild.transform.position.x + halfObjectWidth)
                {
                    firstChild.transform.SetAsLastSibling();
                    var position = lastChild.transform.position;
                    firstChild.transform.position = new Vector3(
                        position.x + halfObjectWidth * 2, position.y, position.z);
                }
                else if(_mainCamera.transform.position.x + _screenBounds.x < firstChild.transform.position.x - halfObjectWidth)
                {
                    lastChild.transform.SetAsFirstSibling();
                    var position = firstChild.transform.position;
                    lastChild.transform.position = new Vector3(
                        position.x - halfObjectWidth * 2, position.y, position.z);
                }
            }
        }
    }
}
