module Grains.Garbaggio

open System.Collections.Generic
open System.Linq
open System
open System.Text

let UnreservedChars =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~"

let IsReservedCharacter (x: char) : bool = UnreservedChars.Contains x |> not

let UrlEncode (value: string) : string =
    let builder = StringBuilder()

    String.iter
        (fun x ->
            match IsReservedCharacter x with
            | false -> builder.Append x |> ignore
            | true ->
                builder.AppendFormat("%{0:X2}", int x)
                |> ignore)
        value

    builder.ToString()

let FlattenDictionaryToQueryParameterString (queryItems: Dictionary<string, string>) : string =
    let orderedItems =
        queryItems
            .OrderByDescending(fun x -> x.Key)
            .Select(fun x -> $"{x.Key}={x.Value |> UrlEncode}")
            .ToArray()

    String.Join("&", orderedItems)