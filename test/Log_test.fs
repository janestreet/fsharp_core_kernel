module Core_kernel.Test.Log

open NUnit.Framework
open Core_kernel
open System
open System.IO

[<Test>]
let ``printfn_every_n even iterations`` () =
  use string_writer = new StringWriter()
  Console.SetOut(string_writer)

  for i in 0..10 do
    Log.Limited.printfn_every_n (2, "%d") i

  let output =
    string_writer.ToString()
    |> Log.For_testing.sanitize_timestamp

  let expected_output =
    List.init (10 + 1) id
    |> List.filter (fun i -> i % 2 = 0)
    |> List.map (sprintf "<TIMESTAMP>: %d\n")
    |> String.concat ""

  Assert.AreEqual(expected_output, output)

[<Test>]
let ``printfn_every_n based on path/line`` () =
  use string_writer = new StringWriter()
  Console.SetOut(string_writer)

  let first_message = "This message will be printed."

  let second_message =
    "This message will also be printed because limiting is done per function call"

  Log.Limited.printfn_every_n (2, "%s") first_message
  Log.Limited.printfn_every_n (2, "%s") second_message

  let output =
    string_writer.ToString()
    |> Log.For_testing.sanitize_timestamp

  let expected_output =
    sprintf "<TIMESTAMP>: %s\n<TIMESTAMP>: %s\n" first_message second_message

  Assert.AreEqual(expected_output, output)
