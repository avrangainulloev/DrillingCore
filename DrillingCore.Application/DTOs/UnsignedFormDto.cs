namespace DrillingCore.Application.DTOs;

public class UnsignedFormDto
{
    public int FormId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = "";
    public int FormTypeId { get; set; }
    public string FormTypeName { get; set; } = "";
    public string CreatorName { get; set; } = "";
    public string OtherComments { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}
