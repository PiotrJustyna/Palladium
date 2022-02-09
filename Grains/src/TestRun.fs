module Grains.TestRun

open System.Xml.Serialization

[<XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")>]
type TestRun =
    { [<XmlAttribute(AttributeName = "id")>]
      Id: string
      [<XmlAttribute(AttributeName = "name")>]
      Name: string }