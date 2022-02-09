namespace Grains

open System.Threading.Tasks
open Orleans

type ISmokeTests =
    inherit IGrainWithGuidKey
    abstract member SmokeTest1 : cancellationToken: GrainCancellationToken -> Task<bool>
    abstract member SmokeTest2 : cancellationToken: GrainCancellationToken -> Task<bool>

type SmokeTests() =
    inherit Grain()

    interface ISmokeTests with
        member this.SmokeTest1(_: GrainCancellationToken) : Task<bool> =
            task {
                do! Task.Delay(2000)
                return true
            }

        member this.SmokeTest2(_: GrainCancellationToken) : Task<bool> =
            task {
                do! Task.Delay(100)
                return false
            }