using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DailyMeals
{
    public abstract class Creature : MonoBehaviour, IRenew, IDisposable
    {
        [SerializeField] protected float _hp = 100f;
        [SerializeField] protected float _maxHp = 100f;
        [SerializeField, Tooltip("数值越大，消耗Hp越多")] protected float _respirationInstensity = 0.05f;
        [SerializeField] protected float _moneyAmount = 0;
        [SerializeField] protected string _fullName = "Creature Name";
        [SerializeField, Tooltip("数值越大，消耗Hp越慢")] protected int _respirationFrequency = 100;
        [Header("组件")]
        [SerializeField] protected Button interactButton;
        public event UnityAction<float> OnHpChanged;
        public event UnityAction OnStartOfFeeding;
        public event UnityAction OnEndOfFeeding;
        public event UnityAction OnDead;
        private bool _isDisposed;

        public string FullName { get => _fullName; }
        public float Hp
        {
            get => _hp;
            private set
            {
                if (value < 0) _hp = 0;
                else if (value > _maxHp) _hp = _maxHp;
                else { _hp = value; }
                OnHpChanged?.Invoke(value - _hp);
            }
        }
        public float MaxHp { get => _maxHp; }
        public float RespirationIntensity { get => _respirationInstensity; }
        public bool IsWorking { get; protected set; }

        protected Queue<Action> _queueTodo = new Queue<Action>();
        public Queue<Action> Todo
        {
            get => _queueTodo;
            private set => _queueTodo = value;
        }
        protected Animator _animator;
        protected int _idleSMId;
        protected int _moveSMId;
        protected int _dialogueSMId;
        protected int _deadSMId;
        protected int _workSMId;
        protected int _isIdleId;
        protected int _isMovingId;
        protected int _isInDialogueId;
        protected int _isWorkingId;
        protected int _workAnimChooseId;

        private CancellationTokenSource _breatheTokenSource;
        private CancellationTokenSource _takeFoodTokenSource;

        protected IState _currentState, _previousState;
        protected IState _idleState, _moveState, _workState, _dialogueState, _deadState;

        #region MonoBehaviour

        protected virtual void Awake()
        {

            if (!TryGetComponent(out _animator))
            {
                Debug.LogError($"{gameObject.name}没有Animator组件!");
                return;
            }
            CacheAnimStringToId();
            InitializeStates();
            AwakeState();

        }

        protected virtual void Start()
        {
            // Hp = MaxHp;
            interactButton.onClick.AddListener(Interact);
        }

        protected virtual void OnDestroy()
        {
            interactButton.onClick.RemoveAllListeners();
        }

        protected virtual void Update()
        {
            //For Test ---------------------Start
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartBreathe();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                StopBreathe();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                // StartTakeFood(,2);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                // StartTakeFood(new Pabulum(),2);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                // StopTakeFood();
            }
            //For Test ---------------------End

        }



        protected virtual void OnEnable()
        {
            GameManager.Singleton.PauseLevel += StopBreathe;
            GameManager.Singleton.ResumeLevel += StartBreathe;
            GameManager.Singleton.EndLevel += StopBreathe;
            ChangeState(_currentState);
        }

        protected virtual void OnDisable()
        {
            GameManager.Singleton.PauseLevel += StopBreathe;
            GameManager.Singleton.ResumeLevel -= StartBreathe;
            GameManager.Singleton.EndLevel -= StopBreathe;
        }
        protected virtual void OnApplicationQuit()
        {
            StopBreathe();
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                StopBreathe();
            }
            else
            {
                StartBreathe();
            }
        }
        #endregion


        #region Protected Methods

        /// <summary>
        /// 初始化Awake状态
        /// </summary>
        protected virtual void AwakeState()
        {
            _currentState = _idleState;
        }

        protected void Interact()
        {
            if (_queueTodo.Count > 0)
            {
                _queueTodo.Dequeue().Invoke();
            }
        }

        protected virtual void StartTakeFood(IFood food, float duration)
        {
            _takeFoodTokenSource = new CancellationTokenSource();
            TakeFoodAsync(_takeFoodTokenSource.Token, food, duration);
        }

        protected void StopTakeFood()
        {
            try
            {
                if (_takeFoodTokenSource != null && !_isDisposed)
                {
                    _takeFoodTokenSource.Cancel();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (_takeFoodTokenSource != null) _takeFoodTokenSource.Dispose();
            }
        }

        protected void StartBreathe()
        {
            _breatheTokenSource = new CancellationTokenSource();
            BreatheAsync(_breatheTokenSource.Token);
        }

        protected void StopBreathe()
        {
            try
            {
                if (!_isDisposed)
                    _breatheTokenSource.Cancel();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (_breatheTokenSource != null) _breatheTokenSource.Dispose();
            }

        }

        protected virtual void InitializeStates()
        {
            _idleState = new IdleState(_animator, _idleSMId, _isIdleId);
            _dialogueState = new DialogueState(_animator, _dialogueSMId, _isInDialogueId);
            _moveState = new MoveState(_animator, _moveSMId, _isMovingId);
            _deadState = new DeadState(_animator, _deadSMId);
            InitializeWorkState();
        }

        protected virtual void InitializeWorkState()
        {
            _workState = new WorkState(_animator, _workSMId, _isWorkingId, _workAnimChooseId, WorkChoose.Eat);
        }
        protected virtual void CacheAnimStringToId()
        {
            _idleSMId = Animator.StringToHash("Idle");
            _workSMId = Animator.StringToHash("Work");
            _dialogueSMId = Animator.StringToHash("Dialogue");
            _moveSMId = Animator.StringToHash("Move");
            _deadSMId = Animator.StringToHash("Dead");
            _isMovingId = Animator.StringToHash("IsMoving");
            _isInDialogueId = Animator.StringToHash("IsDialogue");
            _isIdleId = Animator.StringToHash("IsIdle");
            _isWorkingId = Animator.StringToHash("IsWorking");
            _workAnimChooseId = Animator.StringToHash("WorkAnimChoose");
        }
        #endregion


        #region Private Methods
        private void ChangeState(IState nextState)
        {
            if (_currentState != null)
            {
                _previousState = _currentState;
                _currentState.OnExit();
            }
            _currentState = nextState;
            nextState.OnEnter();
            Debug.Log($"{FullName} state is {_currentState} now!");
        }

        /// <summary>
        /// 返回到之前的状态
        /// </summary>
        private void RecoverState()
        {
            if (_previousState != null)
            {
                ChangeState(_previousState);
            }
            else
            {
                Debug.LogError("没有PreviousState哦！");
            }
        }
        private async void BreatheAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (Hp > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(_respirationFrequency, cancellationToken);
                    //FIXME:为了让C#的异步能同步于TimeScale缩放表现，做了Trick
                    Hp -= RespirationIntensity * GameManager.Singleton.GameTimeScale;
                    Debug.Log($"{FullName}剩余hp:{Hp}");
                }
                Debug.Log($"{FullName} is dead.");
                ChangeState(_deadState);
                OnDead?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
        private async void TakeFoodAsync(CancellationToken cancellationToken, IFood food, float duration)
        {
            Quaternion seatHolderOrientation = transform.rotation;
            try
            {
                OnStartOfFeeding?.Invoke();
                IsWorking = true;
                StopBreathe();
                //找空座位坐下
                ChangeState(_workState);
                GameManager.Singleton.FreeSeat.Sit(transform, out Seat seat, ref seatHolderOrientation);
                //FIXME:为了让C#的异步能同步于TimeScale缩放表现，做了Trick
                await Task.Delay(TimeSpan.FromSeconds(duration / GameManager.Singleton.GameTimeScale), cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    //进食完成后体力增加
                    Hp += food.Nutrition;
                    Debug.Log($"成功吃下{food.FoodName}了");
                    //退还座位
                    seat.Leave(transform);
                    RecoverState();
                    // ChangeState(_moveState);
                    StartBreathe();
                    IsWorking = false;
                    OnEndOfFeeding?.Invoke();
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        #endregion

        public virtual void Renew()
        {
            Hp = _maxHp;
            AwakeState();
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }

}
