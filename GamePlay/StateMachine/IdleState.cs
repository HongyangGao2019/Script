using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{
    public class IdleState : IState
    {
        private Animator _animator;
        private int _triggerId;
        private int _doingId;

        public IdleState(Animator animator,int triggerId,int doingId)
        {
            _animator = animator;
            _triggerId = triggerId;
            _doingId = doingId;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_triggerId);
            _animator.SetBool(_doingId, true);
            Debug.Log($"state Enter:Idle");
        }

        public void OnUpdate()
        {
            Debug.Log($"state Update:Idle");
        }

        public void OnFixedUpdate()
        {
            Debug.Log($"state FixedUpdate:Idle");
        }

        public void OnExit()
        {
            _animator.SetBool(_doingId, false);
            Debug.Log($"state Exit:Idle");
        }
    }

}
