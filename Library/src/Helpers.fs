namespace Library

open System.Linq
open System
open System.Threading.Tasks
open Library.Models.Tests

module Helpers =

    let generateString = fun () -> Guid.NewGuid().ToString()

    let runTests (tests: (string * (unit -> Task<bool>)) []) : Task<TestRunResult> [] =
        tests
            .Select(fun test ->
                task {
                    let startTimestampUtc = DateTime.UtcNow

                    let! result = snd test ()

                    let endTimestampUtc = DateTime.UtcNow

                    let testId = generateString ()

                    let executionId = generateString ()

                    return
                        { Outcome = result
                          StartTimestampUtc = startTimestampUtc.ToString("O")
                          EndTimestampUtc = endTimestampUtc.ToString("O")
                          Duration =
                              (endTimestampUtc - startTimestampUtc)
                                  .ToString("g")
                          TestId = testId
                          ExecutionId = executionId
                          TestName = fst test }
                })
            .ToArray()