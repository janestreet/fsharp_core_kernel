module Core_kernel.Test.Host_and_port

open NUnit.Framework
open System
open Core_kernel


[<Test>]
let ``of_string_exn equal`` () =
  Assert.AreEqual(
    Host_and_port.of_string_exn "host:50",
    ({ host = "host"; port = 50 } : Host_and_port.t)
  )

[<Test>]
let ``of_string_exn raises`` () =
  Assert.Throws<Exception> (fun () ->
    Host_and_port.of_string_exn "host:port"
    |> (ignore : Host_and_port.t -> unit))
  |> ignore
