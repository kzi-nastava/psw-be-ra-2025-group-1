using Explorer.BuildingBlocks.Tests;

namespace Explorer.ProjectAutopsy.Tests;

public class BaseProjectAutopsyIntegrationTest : BaseWebIntegrationTest<ProjectAutopsyTestFactory>
{
    public BaseProjectAutopsyIntegrationTest(ProjectAutopsyTestFactory factory) : base(factory) { }
}
