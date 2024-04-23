using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{

    [RequireComponent(typeof(ParticleSystem))]
    public class PsController : MonoBehaviour
    {
        private ParticleSystem system;

        void Start()
        {
            system = GetComponent<ParticleSystem>();
            var main = system.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        void OnParticleSystemStopped()
        {
            Destroy(this.gameObject);
        }
    }
}