namespace SiloHost.Controllers

open System
open System.IO
open System.Linq
open System.Threading.Tasks
open System.Xml.Serialization
open Grains
open Library
open Library.Models.Tests
open Microsoft.AspNetCore.Mvc
open Orleans

[<ApiController>]
[<Route("[controller]")>]
type AsynchronousTestsController(clusterClient: IClusterClient) =
    inherit ControllerBase()

    let generateString = fun () -> Guid.NewGuid().ToString()

    [<HttpGet>]
    member _.Get() : Task<string> =
        let testListId = generateString ()

        let testId = generateString ()

        let testExecutionId = generateString ()

        let testClassName = "Grains.AsynchronousTests"

        let testMethodName = "name of grain method"

        let testName = testClassName + testMethodName

        let testDll = "AsynchronousTests.dll"

        let testType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"

        let computerName = generateString ()

        let zero = "0"

        task {
            let testsCreationTime = DateTime.UtcNow.ToString("O")

            let testFunctions =
                [| ("Test1",
                    clusterClient
                        .GetGrain<IAsynchronousTests>(
                            Guid.NewGuid()
                        )
                        .Test1)
                   ("Test2",
                    clusterClient
                        .GetGrain<IAsynchronousTests>(
                            Guid.NewGuid()
                        )
                        .Test2)
                   ("Test3",
                    clusterClient
                        .GetGrain<IAsynchronousTests>(
                            Guid.NewGuid()
                        )
                        .Test3) |]

            let testsQueuedTime = DateTime.UtcNow.ToString("O")

            let! results = Helpers.runTests testFunctions |> Task.WhenAll

            let testsFinishedTime = DateTime.UtcNow.ToString("O")

            let testRun =
                { Id = generateString ()
                  Name = generateString ()
                  Times =
                      { Creation = testsCreationTime
                        Queuing = testsQueuedTime
                        Start = testsQueuedTime
                        Finish = testsFinishedTime }
                  TestSettings =
                      { Id = generateString ()
                        Name = generateString () }
                  Results =
                      results
                          .Select(fun x ->
                              { ExecutionId = x.ExecutionId
                                TestId = x.TestId
                                TestName = x.TestName
                                ComputerName = computerName
                                Duration = x.Duration
                                StartTime = x.StartTimestampUtc
                                EndTime = x.EndTimestampUtc
                                TestType = testType
                                Outcome =
                                    match x.Outcome with
                                    | false -> "Failed"
                                    | _ -> "Passed"
                                TestListId = testListId
                                RelativeResultsDirectory = x.ExecutionId })
                          .ToArray()
                  TestDefinitions =
                      results
                          .Select(fun x ->
                              { Id = x.TestId
                                Name = x.TestName
                                Storage = testDll
                                Execution = { Id = x.ExecutionId }
                                TestMethod =
                                    { CodeBase = testDll
                                      ClassName = testClassName
                                      Name = x.TestName
                                      AdapterTypeName = "orleans" } })
                          .ToArray()
                  TestEntries =
                      results
                          .Select(fun x ->
                              { TestId = x.TestId
                                ExecutionId = x.ExecutionId
                                TestListId = testListId })
                          .ToArray()
                  TestLists =
                      [| { Id = testListId
                           Name = "All Loaded Results" } |]
                  ResultSummary =
                      { Counters =
                            { Total = results.Count().ToString()
                              Executed = results.Count().ToString()
                              Passed =
                                  results
                                      .Count(fun x -> x.Outcome = true)
                                      .ToString()
                              Failed =
                                  results
                                      .Count(fun x -> x.Outcome = false)
                                      .ToString()
                              Error = zero // todo: capture exceptions independently
                              Timeout = zero // todo: capture timeouts independently
                              Aborted = zero
                              Inconclusive = zero
                              PassedButRunAborted = zero
                              NotRunnable = zero
                              NotExecuted = zero
                              Disconnected = zero
                              Warning = zero
                              Completed = zero
                              InProgress = zero
                              Pending = zero }
                        Output = { StdOut = String.Empty }
                        Outcome = "Complete" } }

            let serializerNamespaces = XmlSerializerNamespaces()
            serializerNamespaces.Add("", "")

            let writer = new StringWriter()

            let serializer = XmlSerializer(typeof<TestRun>)
            serializer.Serialize(writer, testRun, serializerNamespaces)

            return writer.ToString()
        }