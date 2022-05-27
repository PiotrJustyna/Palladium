namespace Library.TestInterfaces

open System.Threading.Tasks
open Orleans

type IAsynchronousTests =
    inherit IGrainWithGuidKey
    abstract member Test1: unit -> Task<bool>
    abstract member Test2: unit -> Task<bool>
    abstract member Test3: unit -> Task<bool>
