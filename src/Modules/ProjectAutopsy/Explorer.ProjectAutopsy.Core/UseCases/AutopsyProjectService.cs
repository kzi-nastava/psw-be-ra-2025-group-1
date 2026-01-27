using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.API.Public;
using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;

namespace Explorer.ProjectAutopsy.Core.UseCases;

public class AutopsyProjectService : IAutopsyProjectService
{
    private readonly IAutopsyProjectRepository _repository;
    private readonly IMapper _mapper;

    public AutopsyProjectService(IAutopsyProjectRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public AutopsyProjectDto Create(CreateAutopsyProjectDto dto)
    {
        // Check for duplicate name
        var existing = _repository.GetByName(dto.Name);
        if (existing != null)
            throw new ArgumentException($"Project with name '{dto.Name}' already exists");

        var project = new AutopsyProject(dto.Name, dto.Description);

        if (!string.IsNullOrEmpty(dto.JiraProjectKey))
            project.ConfigureJira(dto.JiraProjectKey);

        if (!string.IsNullOrEmpty(dto.GitHubRepo))
            project.ConfigureGitHub(dto.GitHubRepo);

        if (dto.AnalysisWindowDays > 0)
            project.SetAnalysisWindow(dto.AnalysisWindowDays);

        var created = _repository.Create(project);
        return _mapper.Map<AutopsyProjectDto>(created);
    }

    public AutopsyProjectDto Update(long id, UpdateAutopsyProjectDto dto)
    {
        var project = _repository.Get(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {id} not found");

        // Check for duplicate name if name is being changed
        if (!string.IsNullOrEmpty(dto.Name) && dto.Name != project.Name)
        {
            var existing = _repository.GetByName(dto.Name);
            if (existing != null && existing.Id != id)
                throw new ArgumentException($"Project with name '{dto.Name}' already exists");
        }

        if (!string.IsNullOrEmpty(dto.Name))
            project.Update(dto.Name, dto.Description ?? project.Description);

        if (dto.Description != null)
            project.Update(project.Name, dto.Description);

        if (!string.IsNullOrEmpty(dto.JiraProjectKey))
            project.ConfigureJira(dto.JiraProjectKey);

        if (!string.IsNullOrEmpty(dto.GitHubRepo))
            project.ConfigureGitHub(dto.GitHubRepo);

        if (dto.AnalysisWindowDays.HasValue)
            project.SetAnalysisWindow(dto.AnalysisWindowDays.Value);

        if (dto.IsActive.HasValue)
        {
            if (dto.IsActive.Value)
                project.Activate();
            else
                project.Deactivate();
        }

        var updated = _repository.Update(project);
        return _mapper.Map<AutopsyProjectDto>(updated);
    }

    public void Delete(long id)
    {
        var project = _repository.Get(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {id} not found");

        _repository.Delete(id);
    }

    public AutopsyProjectDto Get(long id)
    {
        var project = _repository.Get(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {id} not found");

        return _mapper.Map<AutopsyProjectDto>(project);
    }

    public List<AutopsyProjectDto> GetAll()
    {
        var projects = _repository.GetPaged(0, int.MaxValue).Results;
        return projects.Select(p => _mapper.Map<AutopsyProjectDto>(p)).ToList();
    }

    public AutopsyProjectDto ConfigureGitHub(long id, string repo)
    {
        var project = _repository.Get(id);
        if (project == null)
            throw new KeyNotFoundException($"Project with id {id} not found");

        project.ConfigureGitHub(repo);
        var updated = _repository.Update(project);

        return _mapper.Map<AutopsyProjectDto>(updated);
    }
}
