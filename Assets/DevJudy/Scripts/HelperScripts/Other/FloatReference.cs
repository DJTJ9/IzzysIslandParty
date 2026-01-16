using System;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    [ShowIf("UseConstant")]
    public float ConstantValue;
    public SO_FloatVariable Variable;

    public float Value => UseConstant ? ConstantValue : Variable.Value;
}