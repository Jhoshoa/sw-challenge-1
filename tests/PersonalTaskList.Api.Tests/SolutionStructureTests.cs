using System.Reflection;

namespace PersonalTaskList.Api.Tests;

public class SolutionStructureTests
{
    [Fact]
    public void ApiAssemblyReferenceIsAvailable()
    {
        var assembly = Assembly.Load("PersonalTaskList.Api");

        Assert.Equal("PersonalTaskList.Api", assembly.GetName().Name);
    }
}
