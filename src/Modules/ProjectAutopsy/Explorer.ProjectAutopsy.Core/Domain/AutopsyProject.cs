using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.ProjectAutopsy.Core.Domain;

/// <summary>
/// Represents a project being analyzed by Project Autopsy.
/// This is separate from the main Explorer project entities.
/// </summary>
public class AutopsyProject : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    
    // External integrations
    public string? JiraProjectKey { get; private set; }
    public string? GitHubRepo { get; private set; } // format: "owner/repo"
    
    // Analysis settings
    public int AnalysisWindowDays { get; private set; }
    public bool IsActive { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastAnalysisAt { get; private set; }

    public AutopsyProject() { } // EF Core

    public AutopsyProject(string name, string? description = null)
    {
        Name = name;
        Description = description;
        AnalysisWindowDays = 30;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfigureJira(string projectKey)
    {
        JiraProjectKey = projectKey;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfigureGitHub(string repo)
    {
        GitHubRepo = repo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAnalysisWindow(int days)
    {
        if (days < 7 || days > 90)
            throw new ArgumentException("Analysis window must be between 7 and 90 days");
        
        AnalysisWindowDays = days;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAnalyzed()
    {
        LastAnalysisAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() 
    { 
        IsActive = true; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() 
    { 
        IsActive = false; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
