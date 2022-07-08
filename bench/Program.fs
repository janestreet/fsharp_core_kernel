open System
open Core_kernel

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

let primitive_value = ("a thing", ("hi", 5))

type Test () =

  [<Benchmark>]
  member this.sprintf() =
    let str = sprintf "with a format %A" primitive_value
    str |> (ignore : string -> unit)

  [<Benchmark>]
  member this.Error_Of_string() =
    let error = Error.Of.string "a static string"
    error |> (ignore : Error.t -> unit)


  [<Benchmark>]
  member this.Error_Of_format() =
    let error = Error.Of.format "with a format %A" primitive_value
    error |> (ignore : Error.t -> unit)

[<EntryPoint>]
let main argv =
  let _ = BenchmarkRunner.Run<Test>()
  0
