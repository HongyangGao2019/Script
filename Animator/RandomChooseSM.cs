using System;
using UnityEngine;
using Random = System.Random;


public class RandomChooseSM : StateMachineBehaviour
{
    [Header("从1下标开始")]
    [SerializeField] private int optionalCount;
    [SerializeField] private string animIdName;
    private int randomChoose;
    private int animId;
    private Random _rand;

    private void Awake()
    {
        animId = Animator.StringToHash(animIdName);
        Debug.Log("Hello Animator");
        _rand = new Random();
    }
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        randomChoose=_rand.Next(1, optionalCount);
        // randomChoose = r.NextInt(1, optionalCount);
        animator.SetInteger(animId,randomChoose);
        // Debug.Log($"{animator.gameObject.name} random Choose:{randomChoose}");
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        
    }
}
