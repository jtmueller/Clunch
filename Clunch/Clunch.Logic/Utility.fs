[<AutoOpen>]
module Clunch.Utility

open System

let inline isNull x = Object.ReferenceEquals(x, null)
let inline isNotNull x = isNull x |> not

