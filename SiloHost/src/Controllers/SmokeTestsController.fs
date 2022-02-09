namespace SiloHost.Controllers

open System
open System.Diagnostics
open System.Threading.Tasks
open Grains
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Orleans

[<ApiController>]
[<Route("[controller]")>]
type SmokeTestsController(logger: ILogger<SmokeTestsController>, clusterClient: IClusterClient) =
    inherit ControllerBase()

    let operation () : Task<bool> =
        let grain =
            clusterClient.GetGrain<ISmokeTests>(Guid.NewGuid())

        let cancellationTokenSource = new GrainCancellationTokenSource()
        grain.SmokeTest1 cancellationTokenSource.Token

    [<HttpGet>]
    member _.Get() : Task<string> =
        task {
            let stopwatch = Stopwatch()
            stopwatch.Start()

            let! results =
                [| operation ()
                   operation ()
                   operation () |]
                |> Task.WhenAll

            stopwatch.Stop()
            return $"{results.[0]}, {results.[1]}, elapsed time: {stopwatch.ElapsedMilliseconds}ms"
        }