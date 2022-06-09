namespace Xunit.Orleans.TestFramework;

[AttributeUsage(AttributeTargets.Assembly)]
public class OrleansTestFrameworkAttribute : Attribute
{
    /// <summary>
    /// The type of the local custom attribute used to mark the Orleans test interfaces.
    /// </summary>
    public Type OrleansTestInterfaceDiscovererType { get; }

    /// <summary>
    /// Whether the Orleans silo/cluster will be stopped after the tests execution.
    /// </summary>
    public bool StopOrleansClusterAfterTestExecution { get; }

    /// <summary>
    /// Initializes an instance of OrleansTestFrameworkAttribute
    /// </summary>
    /// <param name="orleansTestInterfaceDiscovererType">The type of the local custom attribute used to mark the Orleans test interfaces.</param>
    /// <param name="stopOrleansClusterAfterTestExecution">Whether the Orleans silo/cluster will be stopped after the tests execution.</param>
    public OrleansTestFrameworkAttribute(
        Type orleansTestInterfaceDiscovererType,
        bool stopOrleansClusterAfterTestExecution = true)
    {
        OrleansTestInterfaceDiscovererType = orleansTestInterfaceDiscovererType;
        StopOrleansClusterAfterTestExecution = stopOrleansClusterAfterTestExecution;
    }
}