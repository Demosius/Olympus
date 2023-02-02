using Uranus.Staff;

namespace Uranus.Users.Models;

public enum EDevIssueType
{
    Other,
    Query,
    Bug,
    Improvement,
}

public class DevMessage : Message
{
    public EDevIssueType IssueType { get; set; }
    public EProject Project { get; set; }
    public string Subject { get; set; }
    public string Tags { get; set; }

    public DevMessage()
    {
        IssueType = EDevIssueType.Other;
        Project = EProject.None;
        Subject = string.Empty;
        Tags = string.Empty;
    }
}