namespace Grains

open System.Threading.Tasks
open Orleans

type ISmokeTests =
    inherit IGrainWithGuidKey
    abstract member SmokeTest1 : unit -> Task<bool>
    abstract member SmokeTest2 : unit -> Task<bool>

type SmokeTests() =
    inherit Grain()

    interface ISmokeTests with
        member this.SmokeTest1() : Task<bool> =
            task {
                do! Task.Delay(2000)
                return true
            }

        member this.SmokeTest2() : Task<bool> =
            task {
                do! Task.Delay(100)
                return false
            }