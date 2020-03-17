using UnityEngine;

public class CustomLabelAttribute : PropertyAttribute
{
    public readonly string Value;

    public CustomLabelAttribute(string value)
    {
        Value = value;
    }
}