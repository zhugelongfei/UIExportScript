using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class UIDataAttribute : Attribute
{
    private readonly string fieldName;

    public UIDataAttribute(string fieldName)
    {
        this.fieldName = fieldName;
    }

    public string FieldName
    {
        get { return fieldName; }
    }
}