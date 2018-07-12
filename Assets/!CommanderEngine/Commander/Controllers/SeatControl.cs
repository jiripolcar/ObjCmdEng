using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommanderEngine
{
    [RequireComponent(typeof(ObjectIdentifier))]
    public class SeatControl : MonoBehaviour
    {

        [SerializeField] private GameObject _constraintSit;
        [SerializeField] private GameObject _constraintStandUp;
        [SerializeField] private bool standUpConstraintForRightFoot = true;

        [SerializeField] private CharacterCommander _owner;
        [SerializeField] private CharacterCommander previousOwner;

        public bool HasOwner { get { return _owner || previousOwner; } }

        public CharacterCommander Owner
        {
            get { return _owner; }
            set
            {
                if (_owner)
                    if (_owner != value)
                    {
                        _owner.SeatState = null;
                        if (value)
                            value.SeatState = this;
                    }
                _owner = value;
                if (value)
                    previousOwner = value;
            }
        }

        public GameObject ConstraintSit {  get { return _constraintSit ? _constraintSit : gameObject; } }
        public GameObject ConstraintStandUp { get { return _constraintStandUp ? _constraintStandUp : gameObject; } }
        public bool RightFoot { get { return standUpConstraintForRightFoot; } }

    }
}