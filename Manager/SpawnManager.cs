using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace DailyMeals
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Boss")]
        [SerializeField] private Transform _bosses_spawnParent;
        [SerializeField] private Transform _newbossSpawnPos;
        [SerializeField] private Transform[] _bossSpawnPos;
        
        [Header("Customer")]
        [SerializeField] private Transform _customers_spawnParent;
        [SerializeField] private Transform[] _customerSpawnPos;
        [SerializeField] private float _minCustomerSpawnDelay=1f;
        [SerializeField] private float _maxCustomerSpawnDelay=3f;
        
        [Header("FoodMenu")]
        [SerializeField] private RectTransform _foodItems_spawnParent;
        [SerializeField] private FoodItem _foodItem_prefab;
        [SerializeField] private float _respawnFoodMenuDelayMinutes=3f;
        
        [Header("Seat")] 
        [SerializeField] private Transform _seats_spawnParent;
        [SerializeField] private Transform[] _seats_transform;
        
        [Header("Table")] 
        [SerializeField] private Transform _food_spawnParent;
        [SerializeField] private Transform[] food_transform;
        
        [Header("Parabola Draw")]
        [SerializeField] private Transform _parabola_spawnparent;
        [SerializeField] private GameObject _parabolaPrefab;
        [SerializeField] private float _parabolaFormulaFactor=0.1f;
        [SerializeField] private float _parabolaHeightOffset=0.1f;
        [SerializeField] private int _parabolaCount=4;
        [SerializeField] private float _parabolaGradientSize=0.2f;
        [SerializeField] private float _parabolaSizeOffset=0.2f;

        [Header("Customer ObjectPool设置")] 
        [SerializeField] private int _defaultCustomerCapacity = 10;
        [SerializeField] private bool _customerCollectionCheck = true;
        [SerializeField] private int  _maxCustomerPoolSize=30;
        [SerializeField] private PoolType _customerPoolType = PoolType.Stack;
        
        private int _currentCustomerSpawnPosIndex;
        private int _preventCustomerSpawnPosIndex;
        private CancellationTokenSource _spawnCustomerCancellationTokenSource;
        private CancellationTokenSource _spawnFoodMenuCancellationTokenSource;

        public static SpawnManager Singleton { get; private set; }
                
        private IObjectPool<Customer> _customerPool;
        
        private IObjectPool<GameObject> _parabolaPool;

        public IObjectPool<GameObject> ParabolaPool
        {
            get
            {
                if (_parabolaPool == null)
                {
                    _parabolaPool = new ObjectPool<GameObject>(CreateParabola, OnTakeFromParabolaPool,
                        OnReturnedToParabolaPool, OnDestroyParabolaPoolObjet, true, _parabolaCount*2+1,_parabolaCount*2+1);
                }

                return _parabolaPool;
            }
        }

        public IObjectPool<Customer> CustomerPool
        {
            get
            {
                if (_customerPool == null)
                {
                    if (_customerPoolType == PoolType.Stack)
                    {
                        _customerPool = new ObjectPool<Customer>(CreateCustomer,OnTakeFromCustomerPool,OnReturnedToCustomerPool,
                            OnDestroyCustomerPoolObjet,_customerCollectionCheck,_defaultCustomerCapacity,_maxCustomerPoolSize);
                    }
                    else if(_customerPoolType==PoolType.LinkedList)
                    {
                        _customerPool = new LinkedPool<Customer>(CreateCustomer,OnTakeFromCustomerPool,OnReturnedToCustomerPool,
                            OnDestroyCustomerPoolObjet,_customerCollectionCheck,_maxCustomerPoolSize);
                    }
                }
                return _customerPool;
            }
        }
        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            GameManager.Singleton.InitLevel += FirstSpawnBosses;
            GameManager.Singleton.InitLevel += StartSpawnCustomerWave ;
            GameManager.Singleton.InitLevel += StartSpawnFoodMenu  ;
            GameManager.Singleton.InitLevel += SpawnSeats ;

            GameManager.Singleton.EndLevel += ClearFoodItemsContainer ;
            GameManager.Singleton.EndLevel += ClearBosses ;
            GameManager.Singleton.EndLevel += ClearCustomers ;
            GameManager.Singleton.EndLevel += ClearSeats ;

            GameManager.Singleton.ResumeLevel += StartSpawnCustomerWave ;
            GameManager.Singleton.ResumeLevel += StartSpawnFoodMenu ;
            GameManager.Singleton.PauseLevel += StopSpawnCustomerWave ;
            GameManager.Singleton.PauseLevel += StopSpawnFoodMenu ;
        }
        public  Vector3 StartPos { get; set; }
        public Transform EndTransform { get; set; }
        public bool IsDragging { get; set; }


        private bool _drew=false;
        private List<GameObject> _parabolaList=new List<GameObject>();
        private void DrawParabola()
        {
            if (IsDragging)
            {
                if(EndTransform==null)return;
                Vector3 delta=(EndTransform.position - StartPos) / (_parabolaCount*2+1);
                if (!_drew)
                {
                    for (int i = 0; i < (_parabolaCount*2+1); i++)
                    {
                        Vector3 pos = StartPos + new Vector3(delta.x, 0, delta.z) * i;
                        pos.y = -_parabolaFormulaFactor * (i - _parabolaCount) * (i - _parabolaCount) +
                                _parabolaFormulaFactor * (_parabolaCount) * (_parabolaCount) + _parabolaHeightOffset;
                        GameObject parabolaGo = ParabolaPool.Get();
                        parabolaGo.transform.localScale = Vector3.one-Vector3.one*(_parabolaGradientSize*(Mathf.Abs(i - _parabolaCount))-_parabolaSizeOffset);//让抛物线球产生渐变大小
                        parabolaGo.transform.position = pos;
                        _parabolaList.Add(parabolaGo);
                    }
                    _drew = true;
                }
                else
                {
                    for (int i = 0; i < (_parabolaCount*2+1); i++)
                    {
                        Vector3 pos = StartPos + new Vector3(delta.x, 0, delta.z) * i;
                        pos.y = -_parabolaFormulaFactor * (i - _parabolaCount) * (i - _parabolaCount) +
                                _parabolaFormulaFactor * (_parabolaCount) * (_parabolaCount) + _parabolaHeightOffset;
                        _parabolaList[i].transform.position = pos;
                    }
                }

            }
            else
            {
                if (_drew)
                {
                    _drew = false;
                    for (int i = _parabolaList.Count-1; i >=0; i--)
                    {
                        ParabolaPool.Release(_parabolaList[i]);
                        _parabolaList.Remove(_parabolaList[i]);
                    }

                }

            }
        }
        private void Update()
        {
            DrawParabola();
        }

        private void OnDestroy()
        {
            GameManager.Singleton.InitLevel -= FirstSpawnBosses ;
            GameManager.Singleton.InitLevel -= StartSpawnCustomerWave ;
            GameManager.Singleton.InitLevel -= StartSpawnFoodMenu ;
            GameManager.Singleton.InitLevel -= SpawnSeats;

            GameManager.Singleton.EndLevel -= ClearFoodItemsContainer ;
            GameManager.Singleton.EndLevel -= ClearBosses ;
            GameManager.Singleton.EndLevel -= ClearCustomers ;
            GameManager.Singleton.EndLevel -= ClearSeats ;

            GameManager.Singleton.ResumeLevel -= StartSpawnCustomerWave ; 
            GameManager.Singleton.ResumeLevel -= StartSpawnFoodMenu ; 
            GameManager.Singleton.PauseLevel -= StopSpawnCustomerWave ;
            GameManager.Singleton.PauseLevel -= StopSpawnFoodMenu ;
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                StopSpawnCustomerWave();
                StopSpawnFoodMenu();
            }
            else
            {
                StartSpawnCustomerWave();
                StartSpawnFoodMenu();
            }
        }

        private void OnApplicationQuit()
        {
            StopSpawnFoodMenu();
            StopSpawnCustomerWave();
        }
        /// <summary>
        /// 第一次生成，初始化游戏
        /// </summary>
        private void FirstSpawnBosses()
        {
            for (int i = 0; i < GameManager.Singleton.MaxBossCount; i++)
            {
                GameManager.Singleton.RegisterBoss(Instantiate(GameManager.Singleton.BossPrefab,_bossSpawnPos[i].position,_bossSpawnPos[i].rotation,_bosses_spawnParent));
            }
        }

        private void StartSpawnCustomerWave()
        {
            _spawnCustomerCancellationTokenSource = new CancellationTokenSource();
            CustomerSpawnAsync(_spawnCustomerCancellationTokenSource.Token);
        }
        

        private void StopSpawnCustomerWave()
        {
            try
            {
                _spawnCustomerCancellationTokenSource.Cancel();

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _spawnCustomerCancellationTokenSource.Dispose();
            }
        }

        private void StartSpawnFoodMenu()
        {
            _spawnFoodMenuCancellationTokenSource = new CancellationTokenSource(); 
            FoodMenuRespawnAsync(_spawnFoodMenuCancellationTokenSource.Token);
        }
        private void StopSpawnFoodMenu()
        {
            try
            {
                _spawnFoodMenuCancellationTokenSource.Cancel();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _spawnFoodMenuCancellationTokenSource.Dispose();
            }
        }

        private async void CustomerSpawnAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromSeconds(Random.Range(_minCustomerSpawnDelay, _maxCustomerSpawnDelay)), cancellationToken);
                    GameManager.Singleton.RegisterCustomer(SpawnCustomer());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                _spawnCustomerCancellationTokenSource.Dispose();
            }
        }

        private async void FoodMenuRespawnAsync(CancellationToken cancellationToken)
        {
            try
            {
                int i = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    RespawnFoodMenu(i);
                    await Task.Delay(TimeSpan.FromMinutes(_respawnFoodMenuDelayMinutes), cancellationToken); 
                    i=(++i)%GameManager.Singleton.FoodMenus.Length;
                    ClearFoodItemsContainer();
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        /// <summary>
        /// 生成Boss by Random
        /// </summary>
        public Boss SpawnBoss()
        {
            return Instantiate(GameManager.Singleton.BossPrefab, _newbossSpawnPos.position,_newbossSpawnPos.rotation,_bosses_spawnParent);
        }
        
        /// <summary>
        /// 生成Boss
        /// </summary>
        /// <param name="boss">boss的预制体</param>
        public Boss SpawnBoss(Boss boss)
        {
            return Instantiate(boss, _newbossSpawnPos.position,_newbossSpawnPos.rotation,_bosses_spawnParent);
        }

        /// <summary>
        /// 生成Customer by random
        /// </summary>
        public Customer SpawnCustomer()
        {
            do
            {
                _currentCustomerSpawnPosIndex = Random.Range(0, _customerSpawnPos.Length - 1);
            } while (_currentCustomerSpawnPosIndex == _preventCustomerSpawnPosIndex);

            _preventCustomerSpawnPosIndex = _currentCustomerSpawnPosIndex;
            return CustomerPool.Get();
            return Instantiate(GameManager.Singleton.CustomerPrefab, _customerSpawnPos[_currentCustomerSpawnPosIndex]);
        }
        
        /// <summary>
        /// 生成Customer
        /// </summary>
        /// <param name="customer">customer的预制体</param>
        public Customer SpawnCustomer(Customer customer)
        {
            do
            {
                _currentCustomerSpawnPosIndex = Random.Range(0, _customerSpawnPos.Length - 1);
            } while (_currentCustomerSpawnPosIndex == _preventCustomerSpawnPosIndex);

            _preventCustomerSpawnPosIndex = _currentCustomerSpawnPosIndex;
            return Instantiate(customer, _customerSpawnPos[_currentCustomerSpawnPosIndex].position,_customerSpawnPos[_currentCustomerSpawnPosIndex].rotation,_customers_spawnParent);
        }

        private void ClearFoodItemsContainer()
        {
            for (int i=_foodItems_spawnParent.childCount-1;i>=0;i--)
            {
                Destroy(_foodItems_spawnParent.GetChild(i).gameObject);
            }
        }
        private void ClearBosses()
        {
            for (int i=_bosses_spawnParent.childCount-1;i>=0;i--)
            {
                Destroy(_bosses_spawnParent.GetChild(i).gameObject);
            }
        }        
        private void ClearCustomers()
        {
            for (int i=_customers_spawnParent.childCount-1;i>=0;i--)
            {
                CustomerPool.Release(_customers_spawnParent.GetChild(i).GetComponent<Customer>());
            }
        }
        private void ClearSeats()
        {
            for (int i=_seats_spawnParent.childCount-1;i>=0;i--)
            {
                Seat freeSeat = _seats_spawnParent.GetChild(i).GetComponent<Seat>();
                GameManager.Singleton.UnregisterFreeSeat(freeSeat);
                GameManager.Singleton.UnregisterBowlShow(freeSeat);
                Destroy(freeSeat.gameObject);
                Destroy(GameManager.Singleton.BowlShows[freeSeat].gameObject);
            }
        }
        
        private void RespawnFoodMenu(int foodMenuIndex)
        {
            foreach (Food food in GameManager.Singleton.FoodMenus[foodMenuIndex].FoodList)
            {
                Instantiate(_foodItem_prefab, _foodItems_spawnParent).SetUp(food);
            }
        }

        private void SpawnSeats()
        {
            for (int i =0; i <_seats_transform.Length; i++)
            {
                Seat seat=Instantiate(GameManager.Singleton.SeatPrefab, _seats_transform[i].position,
                    _seats_transform[i].rotation, _seats_spawnParent);
                BowlShow bowlShow = Instantiate(GameManager.Singleton.BowlShowPrefab, food_transform[i].position,
                    food_transform[i].rotation, _food_spawnParent);
                GameManager.Singleton.RegisterFreeSeat(seat);
                GameManager.Singleton.RegisterBowlShow(seat, bowlShow);
            }
        }
        
        private Customer CreateCustomer()
        {
            do
            {
                _currentCustomerSpawnPosIndex = Random.Range(0, _customerSpawnPos.Length - 1);
            } while (_currentCustomerSpawnPosIndex == _preventCustomerSpawnPosIndex);

            _preventCustomerSpawnPosIndex = _currentCustomerSpawnPosIndex;
            Customer customer=Instantiate(GameManager.Singleton.CustomerPrefab,_customerSpawnPos[_currentCustomerSpawnPosIndex].position,_customerSpawnPos[_currentCustomerSpawnPosIndex].rotation,_customers_spawnParent) ;
            return customer;
        }

        private void OnReturnedToCustomerPool(Customer customer)
        {
            customer.gameObject.SetActive(false);
        }

        private void OnTakeFromCustomerPool(Customer customer)
        {
            Transform customerSpawnTransform = _customerSpawnPos[Random.Range(0, _customerSpawnPos.Length - 1)];
            Transform customerTransform = customer.transform;
            customerTransform.position = customerSpawnTransform.position;
            customerTransform.rotation = customerSpawnTransform.rotation;
            customer.Renew();
            customer.gameObject.SetActive(true);

        }

        private void OnDestroyCustomerPoolObjet(Customer customer)
        {
            Destroy(customer.gameObject);
        }
        private GameObject CreateParabola()
        {
            return Instantiate(_parabolaPrefab,_parabola_spawnparent) ;
        }

        private void OnReturnedToParabolaPool(GameObject go)
        {
            go.gameObject.SetActive(false);
        }

        private void OnTakeFromParabolaPool(GameObject go)
        {

            go.gameObject.SetActive(true);

        }

        private void OnDestroyParabolaPoolObjet(GameObject go)
        {
            Destroy(go.gameObject);
        }
    }
}

