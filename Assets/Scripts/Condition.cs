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

    // Resolution
    public bool TryResolve(EquipmentType equipment)
    {
        if (Matches(equipment))
        {
            Resolved = true;
            return true;
        }
        return false;
    }

    public bool Matches(EquipmentType equipment)
    {
        return (
            equipment == EquipmentType.Extracator && Type == ConditionType.Trapped ||
            equipment == EquipmentType.Water && Type == ConditionType.OnFire ||
            equipment == EquipmentType.RubberGloves && Type == ConditionType.Electrocuted
            );
    }
}
