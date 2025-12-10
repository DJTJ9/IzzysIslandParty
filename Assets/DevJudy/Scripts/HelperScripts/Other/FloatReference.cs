using System;
using UnityEngine.UIElements;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public SO_FloatVariable Variable;

    public float Value => UseConstant ? ConstantValue : Variable.Value;
}