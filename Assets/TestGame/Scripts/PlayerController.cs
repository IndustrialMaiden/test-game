using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private const string Finish = "Finish";
    private const string Enemy = "Enemy";
    private const string SFX = "SFX";

    [Header("Player Speed")]
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runningSpeed;

    [Space] [Header("Animations")] 
    [SerializeField] private SkeletonAnimation _skeletonAnimation;
    [SerializeField] private AnimationReferenceAsset _idle, _walk, _run, _shot, _failedShot, _loose;

    [Space] [Header("Audio")] 
    [SerializeField] private AudioClip _shootSfx;

    [Space] [Header("Delays")]
    [SerializeField] private float _idleDelay;
    [SerializeField] private float _walkDelay;

    [Space] [Header("Particles")] 
    [SerializeField] private GameObject _muzzle;
    [SerializeField] private Transform _muzzlePosition;

    private float _currentSpeed;

    public bool IsWin { get; private set; }
    public bool IsLoose { get; private set; }
    
    private IEnumerator _currentRoutine;

    private Rigidbody2D _rigidbody;

    private AudioSource _sfxSource;

    private UnityEvent _onWin, _onLoose;

    public void OnSuccessShot()
    {
        StartState(ShootingState());
    }

    public void OnFailedShot()
    {
        StartState(FailedShotState());
    }

    public void SubscribeOnLoose(UnityAction call)
    {
        _onLoose.AddListener(call);
    }
    public void SubscribeOnWin(UnityAction call)
    {
        _onWin.AddListener(call);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _sfxSource = GameObject.FindWithTag(SFX).GetComponent<AudioSource>();
        
        _onLoose = new UnityEvent();
        _onWin = new UnityEvent();
    }

    private void Start()
    {
        StartState(IdleState());
    }

    private void FixedUpdate()
    {
        MovingToFinish();
    }

    private void MovingToFinish()
    {
        _rigidbody.velocity = new Vector2(_currentSpeed * Time.deltaTime, _rigidbody.velocity.y);
    }

    private void StartState(IEnumerator coroutine)
    {
        if (IsWin || IsLoose) return;
        
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }

        _currentRoutine = coroutine;
        StartCoroutine(coroutine);
    }

    private IEnumerator IdleState()
    {
        _currentSpeed = 0;
        SetAnimation(_idle, true);
        yield return new WaitForSeconds(_idleDelay);
        StartState(WalkingState());
    }
    
    private IEnumerator WalkingState()
    {
        _currentSpeed = _walkingSpeed;
        SetAnimation(_walk, true);
        yield return new WaitForSeconds(_walkDelay);
        StartState(RunningState());
    }
    
    private IEnumerator RunningState()
    {
        _currentSpeed = _runningSpeed;
        SetAnimation(_run, true);

        while (_currentRoutine == RunningState())
        {
            yield return null;
        }
    }
    
    private IEnumerator ShootingState()
    {
        _currentSpeed = 0;
        SetAnimation(_shot, false);
        _sfxSource.PlayOneShot(_shootSfx);
        
        yield return new WaitForSeconds(_shot.Animation.Duration * 0.25f);
        
        var muzzle = Instantiate(_muzzle, _muzzlePosition.position, _muzzle.transform.rotation, _muzzlePosition);
        Destroy(muzzle, muzzle.GetComponent<ParticleSystem>().main.duration);
        
        yield return new WaitForSeconds(_shot.Animation.Duration * 0.75f);
        
        StartState(RunningState());
    }
    
    private IEnumerator FailedShotState()
    {
        _currentSpeed = 0;
        SetAnimation(_failedShot, false);
        yield return new WaitForSeconds(_failedShot.Animation.Duration);
        StartState(RunningState());
    }
    
    private IEnumerator LooseState()
    {
        _currentSpeed = 0;
        SetAnimation(_loose, false);
        IsLoose = true;
        _onLoose?.Invoke();

        while (_currentRoutine == LooseState())
        {
            yield return null;
        }
    }
    
    private IEnumerator WinState()
    {
        _currentSpeed = _runningSpeed;
        SetAnimation(_run, true);
        IsWin = true;
        _onWin?.Invoke();
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;

        yield return new WaitForSeconds(9f);
        Destroy(gameObject);
    }

    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale = 1f)
    {
        _skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag.Equals(Finish))
            StartState(WinState());
        if (collider.gameObject.tag.Equals(Enemy))
            StartState(LooseState());
    }

    private void OnDestroy()
    {
        _onWin.RemoveAllListeners();
        _onLoose.RemoveAllListeners();
    }
}
