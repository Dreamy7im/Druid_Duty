using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private SkillCurve fightSkill;
    [SerializeField] private SkillCurve alchemySkill;
    [SerializeField] private SkillCurve BuildingSkill;

    [SerializeField] private int KnowledgePoint;

    private float expNeeded;
    private SkillCurve skillCurve;

    public int ReturnKnowPoint()
    {
        return KnowledgePoint;
    }

    public float ReturnExpNeeded(int statsIndex)
    {
        skillCurve = GetSkillCurve(statsIndex);
        expNeeded = skillCurve.curve.Evaluate(skillCurve.Level);

        return expNeeded;

    }

    public void AddExperience(int statsIndex, float experienceToAdd)
    {
        skillCurve = GetSkillCurve(statsIndex);

        if (skillCurve == null)
        {
            Debug.LogWarning("Invalid stats index provided.");
            return;
        }

        skillCurve.CurrentExperience += experienceToAdd;

        expNeeded = skillCurve.curve.Evaluate(skillCurve.Level);
        if (skillCurve.CurrentExperience >= expNeeded)
        {
            KnowledgePoint++;
            skillCurve.Level++;
        }
    }

    private SkillCurve GetSkillCurve(int statsIndex)
    {
        switch (statsIndex)
        {
            case 0:
                return fightSkill;
            case 1:
                return alchemySkill;
            case 2:
                return BuildingSkill;
            default:
                return null;
        }
    }
}

[System.Serializable]
public class SkillCurve
{
    public AnimationCurve curve;
    public float CurrentExperience;
    public int Level;
}
