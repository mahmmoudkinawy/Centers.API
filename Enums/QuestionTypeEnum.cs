namespace Centers.API.Enums;
public enum QuestionTypeEnum
{
    [EnumMember(Value = "Multiple Choice")]
    MultipleChoice,

    [EnumMember(Value = "True False")]
    TrueFalse,

    [EnumMember(Value = "Free Text")]
    FreeText
}