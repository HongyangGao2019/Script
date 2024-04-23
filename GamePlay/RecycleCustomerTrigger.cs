using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

namespace DailyMeals
{
    public class RecycleCustomerTrigger : MonoBehaviourGizmos
    {
        [SerializeField] private Color gizmoColor;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Customer"))
            {
                Customer customer = other.GetComponent<Customer>();
                GameManager.Singleton.UnregisterCreature(customer);
            }
        }

        public override void DrawGizmos(){
            Draw.SolidBox(transform.position, new float3(1,10,20), gizmoColor);
        }
    }
}
