namespace Grains

open System.Threading.Tasks
open Library.TestInterfaces
open Orleans
open Xunit

module EntryXUnit =
    [<assembly: Xunit.TestFramework("LN.TestFramework.OrleansTestFramework", "LN.TestFramework")>]
    do()

//type IAsynchronousTests =
//    inherit IGrainWithGuidKey
//    abstract member Test1 : unit -> Task<bool>
//    abstract member Test2 : unit -> Task<bool>
//    abstract member Test3 : unit -> Task<bool>

type AsynchronousTests() =
    inherit Grain()

    interface IAsynchronousTests with
    
        [<Fact>]
        member this.Test1() : Task<bool> =
            task {
                do! Task.Delay(100)
                Assert.True(true)
                return true
            }

        member this.Test2() : Task<bool> =
            task {
                do! Task.Delay(200)
                return true
            }

        member this.Test3() : Task<bool> =
            task {
                do! Task.Delay(300)
                return true
            }
    