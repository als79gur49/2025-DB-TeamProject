using System;

/// <summary>
/// 세션에서 선택한 스킬 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class SkillChoiceModel
{
    public int ChoiceID;
    public int SessionID;
    public int SkillID;
    public int ChoiceOrder; // 몇 번째로 선택했는지
    public int PlayerLevel; // 선택 당시 플레이어 레벨
    public DateTime ChosenAt;

    // 추가 정보 (조인 결과용)
    public string SkillName;
    public string SkillType;

    public SkillChoiceModel()
    {
        ChoiceID = 0;
        SessionID = 0;
        SkillID = 0;
        ChoiceOrder = 0;
        PlayerLevel = 1;
        ChosenAt = DateTime.Now;
        SkillName = "";
        SkillType = "";
    }

    public SkillChoiceModel(int sessionID, int skillID, int choiceOrder, int playerLevel)
    {
        ChoiceID = 0;
        SessionID = sessionID;
        SkillID = skillID;
        ChoiceOrder = choiceOrder;
        PlayerLevel = playerLevel;
        ChosenAt = DateTime.Now;
        SkillName = "";
        SkillType = "";
    }
}
