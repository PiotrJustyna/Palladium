using System.Diagnostics;
using System.Reflection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Orleans.TestFramework;

public class OrleansTestFramework : XunitTestFramework
{
    private Process _siloBashProcess = null!;
    private OrleansTestFrameworkAttribute _orleansAssemblyMetadata = null!;
    private Assembly _grainsAssembly = null!;
    
    public OrleansTestFramework(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        GetGrainsAssemblyMetadata(assemblyName);
        _siloBashProcess = StartSilo();
        
        return new OrleansTestExecutor(
            assemblyName, 
            SourceInformationProvider, 
            DiagnosticMessageSink,
            _siloBashProcess,
            _orleansAssemblyMetadata,
            _grainsAssembly);
    }

    private void GetGrainsAssemblyMetadata(AssemblyName assemblyName)
    {
        _grainsAssembly = Assembly.Load(assemblyName);
        
        var assemblyOrleansAttribute =
            _grainsAssembly.GetCustomAttributes(typeof(OrleansTestFrameworkAttribute)).FirstOrDefault();

        _orleansAssemblyMetadata = (OrleansTestFrameworkAttribute)assemblyOrleansAttribute! 
                                  ?? throw new Exception("Could not load the Orleans Test assembly.");
    }
    
    private Process? GetHostProcess()
    {
        //Assumption is the process runs with the name of the assembly/exe
        var processName = Path.GetFileNameWithoutExtension(OrleansTestEnvironmentVariables.SiloHostProjectPath);
        var hostProcess = Process.GetProcessesByName(processName);
            
        return hostProcess.FirstOrDefault();
    }

    private Process StartSilo()
    {
        var runningHostProcess = GetHostProcess();

        if (runningHostProcess != null)
            return runningHostProcess;
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = @"/bin/bash",
            Arguments = @$"-c ""dotnet run --project {OrleansTestEnvironmentVariables.SiloHostProjectPath}""",
            UseShellExecute = false,
            Environment =
            {
                ["CLUSTERNAME"] = OrleansTestEnvironmentVariables.ClusterName,
                ["SERVICENAME"] = OrleansTestEnvironmentVariables.ServiceName,
                ["TESTSAPIPORT"] = OrleansTestEnvironmentVariables.TestApiPort.ToString(),
                ["DASHBOARDPORT"] = OrleansTestEnvironmentVariables.DashboardPort.ToString(),
                ["PRIMARYPORT"] = OrleansTestEnvironmentVariables.PrimaryPort.ToString(),
                ["SILOPORT"] = OrleansTestEnvironmentVariables.SiloPort.ToString(),
                ["GATEWAYPORT"] = OrleansTestEnvironmentVariables.GatewayPort.ToString(),
                ["ADVERTISEDIP"] = OrleansTestEnvironmentVariables.AdvertisedIpAddress.ToString()
            }
        };

        var process = new Process
        {
            StartInfo = processStartInfo
        };

        process.Start();

        DiagnosticMessageSink.OnMessage(new DiagnosticMessage("Orleans Silo starting at {0}...", process.StartTime));

        return process;
    }

    private sealed class OrleansTestExecutor : XunitTestFrameworkExecutor
    {
        private readonly IClusterClient _clusterClient;
        private readonly Process _siloBashProcess;
        private readonly OrleansTestFrameworkAttribute _orleansAssemblyMetadata;
        private readonly Assembly _grainsAssembly;

        public OrleansTestExecutor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider, 
            IMessageSink diagnosticMessageSink,
            Process siloBashProcess,
            OrleansTestFrameworkAttribute orleansAssemblyMetadata,
            Assembly grainsAssembly)
                : base(
                    assemblyName, 
                    sourceInformationProvider, 
                    diagnosticMessageSink)
        {
            _siloBashProcess = siloBashProcess;
            _orleansAssemblyMetadata = orleansAssemblyMetadata;
            _grainsAssembly = grainsAssembly;
            _clusterClient = CreateOrleansClient();
        }

        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink, 
            ITestFrameworkExecutionOptions executionOptions)
        {
            using var orleansRunner = new OrleansTestRunner(
                TestAssembly, 
                testCases, 
                DiagnosticMessageSink, 
                executionMessageSink, 
                executionOptions,
                _clusterClient,
                _orleansAssemblyMetadata);
            
            await orleansRunner.RunAsync();
            
            if(_orleansAssemblyMetadata.StopOrleansClusterAfterTestExecution)
                StopSilo();
        }
        
        private IClusterClient CreateOrleansClient()
        {
            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = OrleansTestEnvironmentVariables.ClusterName;
                    options.ServiceId = OrleansTestEnvironmentVariables.ServiceName;
                })
                .UseLocalhostClustering(
                    OrleansTestEnvironmentVariables.GatewayPort, 
                    OrleansTestEnvironmentVariables.ServiceName,
                    OrleansTestEnvironmentVariables.ClusterName)
                .ConfigureApplicationParts(manager =>
                    manager.AddApplicationPart(_grainsAssembly)
                        .WithReferences()
                        .WithCodeGeneration()
                    )
                .Build();

            return client;
        }

        private void StopSilo()
        {
            //Assumption is the process runs with the name of the assembly/exe
            var processName = Path.GetFileNameWithoutExtension(OrleansTestEnvironmentVariables.SiloHostProjectPath);
            var hostProcessesByName = Process.GetProcessesByName(processName);

            foreach (var process in hostProcessesByName)
            {
                process.Kill();
            }
            
            _siloBashProcess.Kill(true);
        }
    }
}