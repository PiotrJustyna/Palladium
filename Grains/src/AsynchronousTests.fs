namespace Grains

open System.Threading.Tasks
open Orleans

type IAsynchronousTests =
    inherit IGrainWithGuidKey
    abstract member Test1 : unit -> Task<bool>
    abstract member Test2 : unit -> Task<bool>
    abstract member Test3 : unit -> Task<bool>

type AsynchronousTests() =
    inherit Grain()

    interface IAsynchronousTests with
        member this.Test1() : Task<bool> =
            task {
                do! Task.Delay(100)
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