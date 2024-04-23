using UnityEngine;

namespace DailyMeals
{
    public class MoveState:IState
    {
        private Animator _animator;
        private int _triggerId;
        private int _doingId;
        private Vector3 _targetPos;

        public MoveState(Animator animator,int triggerId,int doingId)
        {
            _animator = animator;
            _triggerId = triggerId;
            _doingId = doingId;
        }
        
        public void Init(Vector3 targetPos)
        {
            _targetPos=targetPos;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_triggerId);
            _animator.SetBool(_doingId, true);
            Debug.Log($"state Enter:Move");
        }

        public void OnUpdate()
        {
            Debug.Log($"state Update:Move");
        }

        public void OnFixedUpdate()
        {
            Debug.Log($"state FixedUpdate:Move");
        }

        public void OnExit()
        {
            _animator.SetBool(_doingId, false);
            Debug.Log($"state Exit:Move");
        }


    }
}