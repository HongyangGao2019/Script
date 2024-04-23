using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DailyMeals
{
    public class Boss:Creature,IBoss
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        protected override void Start()
        {
            base.Start();
            GameManager.Singleton.StartLevel += StartBreathe;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Singleton.StartLevel -= StartBreathe;
        }

        protected override void Update(){
            base.Update();
        }
        protected override void InitializeWorkState()
        {
            _workState = new WorkState(_animator, _workSMId,_isWorkingId,_workAnimChooseId,WorkChoose.Cook);
        }
        private void Check()
        {
            Debug.Log($"FullName:{FullName} is interact!");
        }
        public void CheckCharge(Customer customer,float bill)
        {
            _moneyAmount += customer.Bill;
        }
    }
}