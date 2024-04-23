using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace DailyMeals
{
    public class CreatureUI : MonoBehaviour
    {
        [SerializeField] private Image _hpImage;
        [SerializeField] private Material _hpMaterial;
        private Creature _creature;
        private int _hpId;
        private Vector3 _uiOrientation = new Vector3(0, 0, -100000);

        private void Awake()
        {
            _creature = GetComponentInParent<Creature>();
            _hpImage.material = new Material(_hpMaterial);
            _hpId = Shader.PropertyToID("_Hp");
        }

        private void Start()
        {
            _creature.OnHpChanged += HpChanged;

        }

        private void Update()
        {

            transform.LookAt(_uiOrientation);
        }

        private void OnDestroy()
        {
            _creature.OnHpChanged -= HpChanged;

        }

        private void HpChanged(float value)
        {
            _hpImage.material.SetFloat(_hpId, _creature.Hp / _creature.MaxHp);
        }
    }
}
