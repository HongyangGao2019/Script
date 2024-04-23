using UnityEngine;

namespace DailyMeals
{
    public class DeadState:IState
    {
        private Animator _animator;
        private int _triggerId;
        // private int _doingId;

        public DeadState(Animator animator,int triggerId)
        {
            _animator = animator;
            _triggerId = triggerId;

        }
        public void OnEnter()
        {
            _animator.SetTrigger(_triggerId);
            // _animator.SetBool(_doingId, true);
            Debug.Log($"state Enter:Dead");
        }

        public void OnUpdate()
        {
            Debug.Log($"state Update:Dead");
        }

        public void OnFixedUpdate()
        {
            Debug.Log($"state FixedUpdate:Dead");
        }

        public void OnExit()
        {
            // _animator.ResetTrigger(_triggerId);
            // _animator.SetBool(_doingId, false);
            Debug.Log($"state Exit:Dead");
        }
    }
}