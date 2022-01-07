module Tests

open System.Threading
open Xunit

[<Fact>]
let ``My test 1`` () =
    Thread.Sleep(1000)
    Assert.True(true)

[<Fact>]
let ``My test 2`` () =
    Thread.Sleep(1000)
    Assert.True(true)
