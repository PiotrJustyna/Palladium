namespace SiloHost.Controllers

open System
open System.Diagnostics
open System.Threading.Tasks
open Grains
open Grains.TestRun
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Orleans

[<ApiController>]
[<Route("[controller]")>]
type SmokeTestsController(logger: ILogger<SmokeTestsController>, clusterClient: IClusterClient) =
    inherit ControllerBase()

    let operation (test: GrainCancellationToken -> Task<bool>) : Task<bool> =
        let cancellationTokenSource = new GrainCancellationTokenSource()
        test cancellationTokenSource.Token
    
    let operation1 : Task<bool> = clusterClient.GetGrain<ISmokeTests>(Guid.NewGuid()).SmokeTest1 |> operation

    let operation2 : Task<bool> = clusterClient.GetGrain<ISmokeTests>(Guid.NewGuid()).SmokeTest2 |> operation


    [<HttpGet>]
    member _.Get() : Task<string> =
        task {
            let stopwatch = Stopwatch()
            stopwatch.Start()
            
            let qwe : TestRun =
                { Id = ""
                  Name = "" }

            let! results =
                [| operation1
                   operation2
                   operation1 |]
                |> Task.WhenAll

            stopwatch.Stop()
            return $"{results.[0]}, {results.[1]}, {results.[2]}, elapsed time: {stopwatch.ElapsedMilliseconds}ms"
        }