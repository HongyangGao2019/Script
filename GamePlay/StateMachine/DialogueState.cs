using UnityEngine;

namespace DailyMeals
{
    public class DialogueState:IState
    {
        private Animator _animator;
        private int _triggerId;
        private int _doingId;

        public DialogueState(Animator animator,int triggerId,int doingId)
        {
            _animator = animator;
            _triggerId = triggerId;
            _doingId = doingId;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_triggerId);
            _animator.SetBool(_doingId, true);
            Debug.Log($"state Enter:Dialogue");
        }

        public void OnUpdate()
        {
            Debug.Log($"state Update:Dialogue");
        }

        public void OnFixedUpdate()
        {
            Debug.Log($"state FixedUpdate:Dialogue");
        }

        public void OnExit()
        {
            _animator.SetBool(_doingId, false);
            Debug.Log($"state Exit:Dialogue");
        }
    }
}