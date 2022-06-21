using Orleans;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Orleans.TestFramework;

public class OrleansTestRunner : TestAssemblyRunner<IXunitTestCase>
{
    private readonly IClusterClient _clusterClient;
    private readonly OrleansTestFrameworkAttribute _orleansAssemblyMetadata;
    public OrleansTestRunner(
        ITestAssembly testAssembly, 
        IEnumerable<IXunitTestCase> testCases, 
        IMessageSink diagnosticMessageSink, 
        IMessageSink executionMessageSink, 
        ITestFrameworkExecutionOptions executionOptions,
        IClusterClient clusterClient,
        OrleansTestFrameworkAttribute orleansAssemblyMetadata)
            : base(
                testAssembly, 
                testCases, 
                diagnosticMessageSink, 
                executionMessageSink, 
                executionOptions)
    {
        _clusterClient = clusterClient;
        _orleansAssemblyMetadata = orleansAssemblyMetadata;
        DiagnosticMessageSink = diagnosticMessageSink;
    }

    protected override string GetTestFrameworkDisplayName()
    {
        return XunitTestFrameworkDiscoverer.DisplayName;
    }

    protected override async Task<RunSummary> RunTestCollectionsAsync(
        IMessageBus messageBus, 
        CancellationTokenSource cancellationTokenSource)
    {
        var summary = new RunSummary();
        const int allowedRetries = 30;
        var attemptedRetries = 0;

        //connect to the cluster, allowing 30 retries with 1 second interval
        await _clusterClient.Connect(exception =>
        {
            DiagnosticMessageSink.OnMessage(
                new DiagnosticMessage("Connection to Orleans CLuster failed. Retrying attempt #{0}...",
                attemptedRetries + 1));
            
            if (exception == null) return Task.FromResult(false);
            
            attemptedRetries++;

            if (attemptedRetries > allowedRetries) throw exception;
                
            Thread.Sleep(1000);
            return Task.FromResult(true);
        });
        
        //prepare the execution of the test collections
        var executions = OrderTestCollections()
            .Select(orderTestCollection => 
                CollectionAsyncExecutor(messageBus, cancellationTokenSource, orderTestCollection, summary)
                ).ToList();

        //execute the test collections in parallel
        await Task.WhenAll(executions);
        await _clusterClient.Close();
        
        return summary;
    }

    protected override Task<RunSummary> RunTestCollectionAsync(
        IMessageBus messageBus, 
        ITestCollection testCollection, 
        IEnumerable<IXunitTestCase> testCases,
        CancellationTokenSource cancellationTokenSource)
    {
        return new OrleansTestCollectionRunner(
            testCollection, 
            testCases, 
            messageBus, 
            DiagnosticMessageSink,
            TestCaseOrderer, 
            new ExceptionAggregator(Aggregator), 
            cancellationTokenSource,
            _clusterClient,
            _orleansAssemblyMetadata).RunAsync();
    }
    
    private async Task<RunSummary> CollectionAsyncExecutor(
        IMessageBus messageBus, 
        CancellationTokenSource cancellationTokenSource, 
        Tuple<ITestCollection, List<IXunitTestCase>> orderTestCollection,
        RunSummary summary)
    {
        var result = await RunTestCollectionAsync(
            messageBus, 
            orderTestCollection.Item1, 
            orderTestCollection.Item2,
            cancellationTokenSource);
        
        summary.Aggregate(result);
        return result;
    }
}