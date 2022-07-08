module Core_kernel.Test.Time

open NUnit.Framework
open System
open Core_kernel

let time = DateTime(1970, 2, 2, 2, 2, 2)

[<Test>]
let ``Time_float roundtrips`` () =
  Assert.AreEqual(
    time,
    time
    |> Time_float.of_datetime
    |> Time_float.to_datetime
  )

[<Test>]
let ``Time_ns roundtrips`` () =
  Assert.AreEqual(time, time |> Time_ns.of_datetime |> Time_ns.to_datetime)
