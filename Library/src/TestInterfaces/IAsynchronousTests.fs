namespace Library.TestInterfaces

open System.Threading.Tasks
open Orleans

type OrleansTestAttribute() = inherit System.Attribute()

[<OrleansTest>]
type IAsynchronousTests =
    inherit IGrainWithGuidKey
    abstract member Test1: unit -> Task<bool>
    abstract member Test2: unit -> Task<bool>
    abstract member Test3: unit -> Task<bool>
    
[<OrleansTest>]
type IAsynchronousTestsBeta =
    inherit IGrainWithGuidKey
    abstract member Test4: unit -> Task<bool>
    abstract member Test5: unit -> Task<bool>
    abstract member Test6: unit -> Task<bool>

[<OrleansTest>]
type IAsynchronousTestFixture =
    inherit IGrainWithGuidKey
    
    abstract member TestWithFixture: unit -> Task<bool>

[<OrleansTest>]
type IAsynchronousTestTheory =
    inherit IGrainWithGuidKey
    
    abstract member TestTheory: number1:int * number2:int * expectedResult:int -> Task<bool>
    
    
