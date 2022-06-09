using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Xunit.Orleans.TestFramework;

public static class OrleansTestEnvironmentVariables
{
    public static string ClusterName => Environment.GetEnvironmentVariable("CLUSTERNAME") ?? "test-cluster";
    
    public static string ServiceName => Environment.GetEnvironmentVariable("SERVICENAME") ?? "test-service";
    
    public static string SiloHostProjectPath
    {
        get
        {
            var rootDirectory = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Parent?.Parent?.Parent;
            var defaultSiloHostPath = Path.Combine(rootDirectory!.FullName, "SiloHost/SiloHost.fsproj");
            
            return Environment.GetEnvironmentVariable("SILOHOSTPROJECTPATH") ?? defaultSiloHostPath;
        }
    }

    public static int TestApiPort => 
        !int.TryParse(Environment.GetEnvironmentVariable("TESTSAPIPORT"), out var apiPort) 
            ? 5001 
            : apiPort;

    public static int DashboardPort =>
        !int.TryParse(Environment.GetEnvironmentVariable("DASHBOARDPORT"), out var dashboardPort)
            ? 8081
            : dashboardPort;
    
    public static int PrimaryPort =>
        !int.TryParse(Environment.GetEnvironmentVariable("PRIMARYPORT"), out var primaryPort)
            ? 2001
            : primaryPort;
    
    public static int SiloPort =>
        !int.TryParse(Environment.GetEnvironmentVariable("SILOPORT"), out var siloPort)
            ? 2001
            : siloPort;
    
    public static int GatewayPort =>
        !int.TryParse(Environment.GetEnvironmentVariable("GATEWAYPORT"), out var gatewayPort)
            ? 3001
            : gatewayPort;
    
    public static IPAddress AdvertisedIpAddress =>
        !IPAddress.TryParse(Environment.GetEnvironmentVariable("ADVERTISEDIP"), out var ipAddress)
            ? GetIpAddress()
            : ipAddress;

    private static IPAddress GetIpAddress()
    {
        var ipAddresses = new List<IPAddress>();
        
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.OperationalStatus.Equals(OperationalStatus.Up));

        foreach (var ni in networkInterfaces)
        {
            ipAddresses.AddRange(ni.GetIPProperties().UnicastAddresses
                .Select(address => address.Address));
        }

        return ipAddresses.First(ip =>
            ip.AddressFamily.Equals(AddressFamily.InterNetwork) && !IPAddress.IsLoopback(ip));
    }
}