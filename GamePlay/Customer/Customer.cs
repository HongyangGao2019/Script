using System;
using System.Numerics;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace DailyMeals
{
    public class Customer : Creature, ICustomer
    {
        public float Bill { get; private set; }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartBreathe();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopBreathe();
            StopTakeFood();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void AwakeState()
        {
            _currentState = _moveState;
        }
        protected override void InitializeWorkState()
        {
            _workState = new WorkState(_animator, _workSMId, _isWorkingId, _workAnimChooseId, WorkChoose.Eat);
        }

        public void Selected()
        {
            transform.DOScale(1.4f, 0.3f);
        }

        public void UnSelected()
        {
            transform.DOScale(1, 0.3f);
        }
        public void OrderMeals(FoodMenu foodMenu, params int[] orderIndexes)
        {
            for (int i = 0; i < orderIndexes.Length; i++)
            {
                if ((_moneyAmount - Bill) < foodMenu.FoodList[i].Price)
                {
                    Debug.Log($"{FullName}买不起{foodMenu.FoodList[i].FoodName}了！");
                    continue;
                }
                Bill += foodMenu.FoodList[i].Price;
                StartTakeFood(foodMenu.FoodList[i], foodMenu.FoodList[i].EatDuration);
            }
            _moneyAmount -= Bill;
            Debug.Log($"Bill:{Bill}");
        }

        public void OrderMeal(Food food)
        {
            if ((_moneyAmount - Bill) < food.Price)
            {
                Debug.Log($"{FullName}买不起{food.FoodName}了！");
                return;
            }
            Instantiate(GameManager.Singleton.IntoStoreEffect, transform.position, quaternion.Euler(new float3(UnityEngine.Vector3.up)));
            Bill += food.Price;
            StartTakeFood(food, food.EatDuration);
            _moneyAmount -= Bill;
            GameManager.Singleton.AddMoney(Bill);
            Debug.Log($"Bill:{Bill}");
        }

        public void CheckOut(Boss boss)
        {
            try
            {
                boss.Todo.Enqueue(() =>
                {
                    boss.CheckCharge(this, Bill);
                    Debug.Log($"{(boss).FullName}已为{FullName} 结账啦！");
                });
                Debug.Log($"{(boss).FullName} 老板结账啦！");
                Bill = 0;
            }
            catch (Exception e)
            {
                Debug.Log($"Error:{e}");
            }
        }

        public override void Renew()
        {
            base.Renew();
        }
    }
}