using System.Diagnostics;
using Orleans;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Orleans.TestFramework;

public class OrleansTestCollectionRunner : TestCollectionRunner<IXunitTestCase>
{
    private readonly IMessageSink _diagnosticMessageSink;
    private readonly IClusterClient _clusterClient;
    private readonly OrleansTestFrameworkAttribute _orleansAssemblyMetadata;

    public OrleansTestCollectionRunner(
        ITestCollection testCollection, 
        IEnumerable<IXunitTestCase> testCases, 
        IMessageBus messageBus,
        IMessageSink diagnosticMessageSink,
        ITestCaseOrderer testCaseOrderer, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource, 
        IClusterClient clusterClient,
        OrleansTestFrameworkAttribute orleansAssemblyMetadata) 
            : base(
                testCollection, 
                testCases, 
                messageBus, 
                testCaseOrderer, 
                aggregator, 
                cancellationTokenSource)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
        _clusterClient = clusterClient;
        _orleansAssemblyMetadata = orleansAssemblyMetadata;
    }

    protected override async Task<RunSummary> RunTestClassAsync(
        ITestClass testClass, 
        IReflectionTypeInfo @class, 
        IEnumerable<IXunitTestCase> xunitTestCases)
    {
        var collectionResults = new List<(bool, decimal)>();
        var testCases = xunitTestCases.ToList();
        
        var interfaceReflectedType = @class.Interfaces
            .FirstOrDefault(i => i.ToRuntimeType().GetCustomAttributes(false)
                .Any(a => a.GetType() == _orleansAssemblyMetadata.OrleansTestInterfaceDiscovererType))
            .ToRuntimeType();
        
        if (interfaceReflectedType == null) 
            throw new Exception($@"Could not find the Test (grain) interface. 
            Make sure the interface is marked with the { _orleansAssemblyMetadata.OrleansTestInterfaceDiscovererType.FullName } test attribute.");

        var grain = _clusterClient.GetGrain(interfaceReflectedType, Guid.NewGuid());

        //invoke the collection's methods
        var collectionStopWatch = Stopwatch.StartNew();

        var testExecutionCollection = testCases
            .Select(xUnitTest => RunTestAsync(
                interfaceReflectedType, 
                xUnitTest, 
                grain, 
                collectionResults, 
                testCases)
            ).ToList();

        //execute the collection's methods in parallel
        await Task.WhenAll(testExecutionCollection);
        collectionStopWatch.Stop();
        
        var collectionExecutionTime = (decimal)collectionStopWatch.ElapsedMilliseconds / 1000;
        var collectionFailedTests = collectionResults.Count(r => r.Item1 == false);
        var collectionSkippedTests = testCases.Count(c => !string.IsNullOrEmpty(c.SkipReason));

        //create the collection output
        MessageBus.QueueMessage(new TestClassFinished(
            testCases, 
            testClass, 
            collectionExecutionTime, 
            testCases.Count, 
            collectionFailedTests, 
            collectionSkippedTests));

        //return the run summary
        return new RunSummary
        {
            Failed = collectionFailedTests,
            Skipped = collectionSkippedTests,
            Time = collectionExecutionTime,
            Total = collectionResults.Count
        };
    }

    private async Task<bool> RunTestAsync(
        Type interfaceReflectedType, 
        IXunitTestCase testCase,
        IGrain grain,
        ICollection<(bool, decimal)> collectionResult,
        List<IXunitTestCase> testCases)
    {
        var testCaseNameParts = testCase.Method.ToRuntimeMethod().Name.Split('.');
        var grainInterfaceInvocationMethod = interfaceReflectedType.GetMethod(testCaseNameParts[^1])!;
        
        MessageBus.QueueMessage(new TestCaseStarting(testCase));
        MessageBus.QueueMessage(new TestMethodStarting(testCases, testCase.TestMethod));
        
        //invoke the method on the grain instance
        var sw = Stopwatch.StartNew();
        var testResult = await (Task<bool>) grainInterfaceInvocationMethod.Invoke(grain, testCase.TestMethodArguments)!;
        sw.Stop();
        
        var executionElapsedTime = (decimal)sw.ElapsedMilliseconds / 1000;
        var testSkipped = string.IsNullOrEmpty(testCase.SkipReason) ? 0 : 1;

        MessageBus.QueueMessage(
            new TestMethodFinished(
                testCases,
                testCase.TestMethod,
                executionElapsedTime,
                1,
                testResult ? 0 : 1,
                testSkipped)
        );

        MessageBus.QueueMessage(
            new TestCaseFinished(
                testCase, 
                executionElapsedTime, 
                1, 
                testResult ? 0: 1, 
                testSkipped)
        );

        //log diagnostic message
        _diagnosticMessageSink.OnMessage(
            new DiagnosticMessage("The test {0} executed in {1} seconds. Test {2} with the following parameters: {3}",  
                testCase.Method.Name, 
                executionElapsedTime, 
                testResult ? "passed" : "failed",
                testCase.TestMethodArguments == null || testCase.TestMethodArguments.Length == 0 
                    ? "nothing" 
                    : string.Join(',', testCase.TestMethodArguments)));
        
        //add method invocation result to the result collection
        collectionResult.Add((testResult, executionElapsedTime));

        return testResult;
    }
}