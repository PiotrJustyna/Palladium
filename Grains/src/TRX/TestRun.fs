module Grains.TestRun

open System.Xml.Serialization

[<XmlRoot("Times", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")>]
type Times = {
      [<XmlAttribute(AttributeName = "creation")>]
      Creation: string
      [<XmlAttribute(AttributeName = "queueing")>]
      Queuing: string
      [<XmlAttribute(AttributeName = "start")>]
      Start: string
      [<XmlAttribute(AttributeName = "finish")>]
      Finish: string      
}

type TestSettings = {
      [<XmlAttribute(AttributeName = "id")>]
      Id: string
      [<XmlAttribute(AttributeName = "name")>]
      Name: string    
}

[<XmlRoot("UnitTestResult")>]
type UnitTestResult = {
      [<XmlAttribute(AttributeName = "executionId")>]
      ExecutionId : string
      [<XmlAttribute(AttributeName = "testId")>]
      TestId : string
      [<XmlAttribute(AttributeName = "testName")>]
      TestName : string
      [<XmlAttribute(AttributeName = "computerName")>]
      ComputerName : string
      [<XmlAttribute(AttributeName = "duration")>]
      Duration : string
      [<XmlAttribute(AttributeName = "startTime")>]
      StartTime : string
      [<XmlAttribute(AttributeName = "endTime")>]
      EndTime : string
      [<XmlAttribute(AttributeName = "testType")>]
      TestType : string
      [<XmlAttribute(AttributeName = "outcome")>]
      Outcome : string
      [<XmlAttribute(AttributeName = "testListId")>]
      TestListId : string
      [<XmlAttribute(AttributeName = "relativeResultsDirectory")>]
      RelativeResultsDirectory : string
}

[<XmlRoot("Execution")>]
type Execution = {
      [<XmlAttribute(AttributeName = "id")>]
      Id : string
}

[<XmlRoot("TestMethod")>]
type TestMethod = {
      [<XmlAttribute(AttributeName = "codeBase")>]
      CodeBase : string
      [<XmlAttribute(AttributeName = "className")>]
      ClassName : string
      [<XmlAttribute(AttributeName = "name")>]
      Name : string
      [<XmlAttribute(AttributeName = "adapterTypeName")>]
      AdapterTypeName : string
}

[<XmlRoot("UnitTest")>]
type UnitTestDefinition = {
     [<XmlAttribute(AttributeName = "id")>]
     Id : string
     [<XmlAttribute(AttributeName = "name")>]
     Name : string
     [<XmlAttribute(AttributeName = "storage")>]
     Storage : string
     Execution : Execution
     TestMethod : string     
}

[<XmlRoot("TestDefinitions")>]
type TestDefinitions = {
      [<XmlAttribute(AttributeName = "UnitTest")>]
      UnitTests : List<UnitTestDefinition>      
}

[<XmlRoot("Results")>]
type Results = {    
      UnitTestResults: List<UnitTestResult>    
}

[<XmlRoot("TestEntry")>]
type TestEntry = {
      [<XmlAttribute(AttributeName = "testId")>]
      TestId : string
      [<XmlAttribute(AttributeName = "executionId")>]
      ExecutionId : string
      [<XmlAttribute(AttributeName = "testListId")>]
      TestListId : string
}

[<XmlRoot("TestList")>]
type TestList = {
      [<XmlAttribute(AttributeName = "id")>]
      Id : string
      [<XmlAttribute(AttributeName = "name")>]
      Name : string
}

[<XmlRoot("Counters")>]
type Counters = {
      [<XmlAttribute(AttributeName = "total")>]
      Total : string
      [<XmlAttribute(AttributeName = "executed")>]
      Executed : string
      [<XmlAttribute(AttributeName = "passed")>]
      Passed : string
      [<XmlAttribute(AttributeName = "failed")>]
      Failed : string
      [<XmlAttribute(AttributeName = "error")>]
      Error : string
      [<XmlAttribute(AttributeName = "timeout")>]
      Timeout : string
      [<XmlAttribute(AttributeName = "aborted")>]
      Aborted : string
      [<XmlAttribute(AttributeName = "inconclusive")>]
      Inconclusive : string
      [<XmlAttribute(AttributeName = "passedButRunAborted")>]
      PassedButRunAborted : string
      [<XmlAttribute(AttributeName = "notRunnable")>]
      NotRunnable : string
      [<XmlAttribute(AttributeName = "notExecuted")>]
      NotExecuted : string
      [<XmlAttribute(AttributeName = "disconnected")>]
      Disconnected : string
      [<XmlAttribute(AttributeName = "warning")>]
      Warning : string
      [<XmlAttribute(AttributeName = "completed")>]
      Completed : string
      [<XmlAttribute(AttributeName = "inProgress")>]
      InProgress : string
      [<XmlAttribute(AttributeName = "pending")>]
      Pending : string
}

[<XmlRoot("Output")>]
type Output = {
      [<XmlAttribute(AttributeName = "StdOut")>]
      StdOut : string
}

[<XmlRoot("ResultSummary")>]
type ResultSummary = {
      Counters : Counters
      Output : Output
      [<XmlAttribute(AttributeName = "outcome")>]
      Outcome : string
}

[<XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")>]
type TestRun = {
      [<XmlAttribute(AttributeName = "id")>]
      Id: string
      [<XmlAttribute(AttributeName = "name")>]
      Name: string
      Times: Times
      TestSettings: TestSettings
      TestResults : Results
      TestDefinitions : TestDefinitions
      TestEntries : TestEntry
      TestLists : List<TestList>
      ResultSummary : ResultSummary
}
