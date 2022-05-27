using System.Reflection;
using Library.TestInterfaces;
using Orleans;
using Orleans.Configuration;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LN.TestFramework;

public sealed class OrleansTestFramework : XunitTestFramework
{
    public OrleansTestFramework(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        return new CustomTestExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }

    private sealed class CustomTestExecutor : XunitTestFrameworkExecutor
    {
        public CustomTestExecutor(AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dummy-test-cluster";
                    options.ServiceId = "dummy-test-service";
                })
                .UseLocalhostClustering(3001, "dummy-test-service", "dummy-test-cluster")
                .Build();

            await client.Connect();

            //foreach (var test in testCases)
            //{
                // var methodInfo = test.Method.ToRuntimeMethod();
                // var interfaceType = methodInfo.DeclaringType?.GetInterfaces().FirstOrDefault(i => i.Namespace.StartsWith("Grains"));
                var grain = client.GetGrain<IAsynchronousTests>(Guid.NewGuid());
                var result = await grain.Test1();


                //var grain = client.GetGrain<IAsynchronousTests>(Guid.NewGuid());
                //var result = await grain.Test1();
                //grain.InvokeOneWay();
            //}
        }
    }
}