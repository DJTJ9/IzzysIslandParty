using System;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

namespace Helper
{
    [Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;

        [ShowIf("UseConstant")]
        public float ConstantValue;

        public SO_FloatVariable Variable;

        public float Value => UseConstant ? ConstantValue : Variable.Value;
    }
}