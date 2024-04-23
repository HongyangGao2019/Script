using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DailyMeals
{
    public enum PoolType
    {
        Stack, LinkedList
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private Boss[] _bossesPrefab;
        [SerializeField] private Customer[] _customersPrefab;
        [SerializeField] private FoodMenu[] _foodMenus;
        [SerializeField] private Seat _seat_prefab;
        [SerializeField] private BowlShow _bowlShow_prefab;
        [SerializeField] private ParticleSystem[] _intoStoreEffectPrefab;
        [SerializeField] private int _maxBossCount = 4;
        [SerializeField] private int _maxCustomerCount = 10;

        [Header("Gameplay")]
        [SerializeField] private int _goldCoin = 0;
        [SerializeField] private float _money = 0;
        [Header("Settings")]
        [SerializeField] private float _gameTimeScale = 1;

        public event UnityAction InitLevel;
        public event UnityAction StartLevel;
        public event UnityAction PauseLevel;
        public event UnityAction ResumeLevel;
        public event UnityAction EndLevel;
        public event UnityAction ForceEndLevel;
        public event UnityAction<int> OnGoldCoinCountChanged;
        public event UnityAction<float> OnMoneyCountChanged;



        private List<Boss> _livingBosses = new List<Boss>();
        private List<Customer> _livingCustomers = new List<Customer>();
        private List<Seat> _freeSeats = new List<Seat>();
        private Dictionary<Seat, BowlShow> _bowlShows = new Dictionary<Seat, BowlShow>();


        public static GameManager Singleton { get; private set; }
        public float GameTimeScale { get => _gameTimeScale; private set => _gameTimeScale = value; }
        public List<Boss> LivingBosses
        {
            get => _livingBosses;
            private set { _livingBosses = value; }
        }
        public List<Customer> LivingCustomers
        {
            get => _livingCustomers;
            private set { _livingCustomers = value; }
        }
        public List<Seat> FreeSeats
        {
            get => _freeSeats;
            private set { _freeSeats = value; }
        }

        public Dictionary<Seat, BowlShow> BowlShows
        {
            get => _bowlShows;
            private set { _bowlShows = value; }
        }
        public Seat FreeSeat
        {
            get
            {
                if (_freeSeats == null) return null;
                return _freeSeats[Random.Range(0, _freeSeats.Count - 1)];
            }
        }

        public FoodMenu[] FoodMenus => _foodMenus;
        public Boss BossPrefab => _bossesPrefab[Random.Range(0, _bossesPrefab.Length - 1)];
        public ParticleSystem IntoStoreEffect => _intoStoreEffectPrefab[Random.Range(0, _intoStoreEffectPrefab.Length - 1)];
        public Customer CustomerPrefab => _customersPrefab[Random.Range(0, _customersPrefab.Length - 1)];
        public Seat SeatPrefab => _seat_prefab;
        public BowlShow BowlShowPrefab => _bowlShow_prefab;
        public int MaxBossCount => _maxBossCount;
        public int MaxCustomerCount => _maxCustomerCount;
        public int GoldCoin
        {
            get => _goldCoin;
            private set
            {
                if (value <= 0) return;
                _goldCoin = value;
                OnGoldCoinCountChanged?.Invoke(value);
            }
        }
        public float Money
        {
            get => _money;
            private set
            {
                if (value <= 0) return;
                _money = value;
                OnMoneyCountChanged?.Invoke(value);
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Singleton = this;
        }

        private void Update()
        {
            Time.timeScale = _gameTimeScale;
        }

        public void InitLevelGame()
        {

            InitLevel?.Invoke();
        }
        public void StartLevelGame()
        {
            StartLevel?.Invoke();
        }

        public void PauseLevelGame()
        {
            PauseLevel?.Invoke();
        }
        public void ResumeLevelGame()
        {
            ResumeLevel?.Invoke();
        }
        public void ForceEndLevelGame()
        {
            _livingBosses.Clear();
            _livingCustomers.Clear();
            _freeSeats.Clear();
            _bowlShows.Clear();
            ForceEndLevel?.Invoke();
        }
        public void AddGoldCoin(int count)
        {
            GoldCoin += count;
        }
        public void SetGoldCoin(int count)
        {
            GoldCoin = count;
        }
        public void AddMoney(float count)
        {
            Money += count;
        }
        public void SetMoney(float count)
        {
            Money = count;
        }

        public void RegisterBoss(Boss boss)
        {
            LivingBosses.Add(boss);
        }
        public void RegisterCustomer(Customer customer)
        {
            LivingCustomers.Add(customer);
        }

        public void RegisterFreeSeat(Seat seat)
        {
            _freeSeats.Add(seat);
        }

        public void UnregisterFreeSeat(Seat seat)
        {
            _freeSeats.Remove(seat);
        }
        public void UnregisterCreature(Creature creature)
        {
            if (creature is Boss boss)
            {
                LivingBosses.Remove(boss);
                Destroy(creature.gameObject);
                if (LivingBosses.Count <= 0)
                {
                    EndLevel?.Invoke();
                }
            }
            else if (creature is Customer customer)
            {
                LivingCustomers.Remove(customer);
                SpawnManager.Singleton.CustomerPool.Release(customer);
            }
        }

        public void UnregisterBowlShow(Seat seat)
        {
            _bowlShows.Remove(seat);
        }

        public void RegisterBowlShow(Seat seat, BowlShow bowlShow)
        {
            _bowlShows.Add(seat, bowlShow);
        }

        #region Private Methods


        #endregion

    }

}
