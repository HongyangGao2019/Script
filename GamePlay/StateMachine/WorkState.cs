using UnityEngine;

namespace DailyMeals
{
    //与Animator里对应
    public enum WorkChoose
    {
        Cook=0,
        Eat=1,
    }
    public class WorkState:IState
    {
        private Animator _animator;
        private int _triggerId;
        private int _doingId;
        private int _workAnimChooseId;
        private WorkChoose _workAnim;

        public WorkState(Animator animator,int triggerId,int doingId,int workAnimChooseId,WorkChoose workAnim)
        {
            _animator = animator;
            _triggerId = triggerId;
            _doingId = doingId;
            _workAnimChooseId = workAnimChooseId;
            _workAnim = workAnim;
        }
        public void OnEnter()
        {
            _animator.SetTrigger(_triggerId);
            _animator.SetBool(_doingId, true);
            _animator.SetInteger(_workAnimChooseId,(int)_workAnim);
            Debug.Log($"state Enter:Work");
        }

        public void OnUpdate()
        {
            Debug.Log($"state Update:Work");
        }

        public void OnFixedUpdate()
        {
            Debug.Log($"state FixedUpdate:Work");
        }

        public void OnExit()
        {
            _animator.SetBool(_doingId, false);
            Debug.Log($"state Exit:Work");
        }
    }
}