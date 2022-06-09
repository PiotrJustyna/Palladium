namespace Grains

open System.Threading.Tasks
open Library.TestInterfaces
open Orleans
open Xunit
open Xunit.Orleans.TestFramework

module EntryXUnit =
    [<assembly: TestFramework("Xunit.Orleans.TestFramework.OrleansTestFramework", "Xunit.Orleans.TestFramework")>]
    do()
    
module EntryOrleansTest =
    [<assembly: OrleansTestFramework(typeof<OrleansTestAttribute>)>]
    do()

//basic test (fact)
type AsynchronousTests() =
    inherit Grain()

    interface IAsynchronousTests with
    
        [<Fact>]
        member this.Test1() : Task<bool> =
            task {
                do! Task.Delay(500)
                Assert.True(true)
                return true
            }

        [<Fact>]
        member this.Test2() : Task<bool> =
            task {
                do! Task.Delay(700)
                return true
            }

        [<Fact>]
        member this.Test3() : Task<bool> =
            task {
                do! Task.Delay(900)
                return true
            }

//second basic test (fact) - failures, skipped tests
type AsynchronousTestsBeta() =
    inherit Grain()
    
    interface IAsynchronousTestsBeta with
    
        [<Fact(Skip = "I don't want to do this.... :(")>]
        member this.Test4() : Task<bool> =
            task {
                do! Task.Delay(600)
                Assert.True(true)
                return true
            }

        [<Fact>]
        member this.Test5() : Task<bool> =
            task {
                do! Task.Delay(800)
                return false
            }

        [<Fact>]
        member this.Test6() : Task<bool> =
            task {
                do! Task.Delay(1000)
                return false
            }

//class fixture to be injected in the test class
type TestClassFixture() =
    member this.GetDatabaseConnectionString() : Task<string> =
        task{
            return "myConnectionString"
        }

//test class with injected class fixture (resolved with Orleans DI)
type AsynchronousTestFixture(_fixture : TestClassFixture) =
    inherit Grain()
    
    interface IClassFixture<TestClassFixture>
    interface IAsynchronousTestFixture with
    
        [<Fact>]
        member this.TestWithFixture() : Task<bool> =
            task {
                do! Task.Delay(500)
                let! connectionString = _fixture.GetDatabaseConnectionString()
                
                Assert.NotEqual<string>(connectionString, "")
                return connectionString <> ""
            }

//theory class with parametrised test with inline data
type AsynchronousTestTheory() =
    inherit Grain()
    
    interface IAsynchronousTestTheory with
    
        [<Theory>]
        [<InlineData(1, 2, 3)>]
        [<InlineData(2, 3, 5)>]
        [<InlineData(1, 1, 2)>]
        member this.TestTheory(number1:int, number2:int, expectedResult:int) : Task<bool> =
            task {
                do! Task.Delay(500)
                let result = number1 + number2 = expectedResult
                
                Assert.True(result)
                return result          
            } 

    