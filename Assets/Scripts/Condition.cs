using UnityEngine;

[System.Serializable]
public class Condition
{
    public ConditionType Type {  get; }
    public bool Resolved {  get; private set; }

    public Condition(ConditionType type)
    {
        Type = type;
    }
}
