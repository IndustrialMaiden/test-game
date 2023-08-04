using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private const string Player = "Player";
    
    [SerializeField] private float _runningSpeedAverage;
    
    [Space] [Header("Animations")] 
    [SerializeField] private SkeletonAnimation _skeletonAnimation;
    [SerializeField] private AnimationReferenceAsset _idle, _run, _angry, _win;

    [Space] [Header("Particles")] 
    [SerializeField] private ParticleSystem _explosion;
    
    private string[] skinsNames = {"blue", "orange", "teal"};
    
    
    private IEnumerator _currentRoutine;

    private Rigidbody2D _rigidbody;

    private PlayerController _player;

    public void OnLoose()
    {
        StartState(LooseState());
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _player = GameObject.FindWithTag(Player).GetComponent<PlayerController>();

        _player.SubscribeOnLoose(OnPlayerLoose);
        _player.SubscribeOnWin(OnPlayerWin);
        
        _skeletonAnimation.skeleton.SetSkin(skinsNames[Random.Range(0, skinsNames.Length)]);

        _runningSpeedAverage = Random.Range(_runningSpeedAverage - 10f, _runningSpeedAverage + 10f);
        
        StartState(RunningState());
    }

    private void OnPlayerLoose()
    {
        StartState(WinState());
    }
    
    private void OnPlayerWin()
    {
        StartState(LooseState());
    }

    private void FixedUpdate()
    {
        MovingToFinish();
    }

    private void MovingToFinish()
    {
        _rigidbody.velocity = new Vector2(-_runningSpeedAverage * Time.deltaTime, _rigidbody.velocity.y);
    }
    
    private void StartState(IEnumerator coroutine)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }

        _currentRoutine = coroutine;
        StartCoroutine(coroutine);
    }
    
    private IEnumerator RunningState()
    {
        SetAnimation(_run, true);

        while (_currentRoutine == RunningState())
        {
            yield return null;
        }
    }
    
    private IEnumerator LooseState()
    {
        yield return new WaitForSeconds(0.5f);
        _runningSpeedAverage = 0;
        _explosion.gameObject.SetActive(true);
        SetAnimation(_angry, false);
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(_angry.Animation.Duration);

        Destroy(gameObject);
    }
    
    private IEnumerator WinState()
    {
        _runningSpeedAverage = 0;
        SetAnimation(_win, true);
        
        while (_currentRoutine == WinState())
        {
            yield return null;
        }
    }

    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale = 1f)
    {
        _skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }
}
