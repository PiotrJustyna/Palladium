namespace SiloHost.Controllers

open System
open System.Diagnostics
open System.Threading.Tasks
open Grains
open Microsoft.AspNetCore.Mvc
open Orleans

[<ApiController>]
[<Route("[controller]")>]
type SmokeTestsController(clusterClient: IClusterClient) =
    inherit ControllerBase()

    [<HttpGet>]
    member _.Get() : Task<string> =
        task {
            let stopwatch = Stopwatch()
            stopwatch.Start()

            let! results =
                [| clusterClient
                    .GetGrain<ISmokeTests>(Guid.NewGuid())
                       .SmokeTest1()
                   clusterClient
                       .GetGrain<ISmokeTests>(Guid.NewGuid())
                       .SmokeTest2()
                   clusterClient
                       .GetGrain<ISmokeTests>(Guid.NewGuid())
                       .SmokeTest1() |]
                |> Task.WhenAll

            stopwatch.Stop()
            return $"{results.[0]}, {results.[1]}, {results.[2]}, elapsed time: {stopwatch.ElapsedMilliseconds}ms"
        }