module Tests

open Grains
open Xunit

[<Fact>]
let ``My test`` () =
    let result = Garbaggio.UrlEncode("Piotr%")
    Assert.Equal("Piotr", result)