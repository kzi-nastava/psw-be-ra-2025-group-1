namespace Explorer.Stakeholders.API.Dtos;

public class ChangeProblemStatusDto
{
    public ProblemStatus Status { get; set; }
    public string? Comment { get; set; }
}
