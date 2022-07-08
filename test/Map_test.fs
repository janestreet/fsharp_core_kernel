module Core_kernel.Test.Map

open NUnit.Framework
open Core_kernel

[<Test>]
let ``merge_skewed`` () =
  let t1 = [ "A", "T1-A"; "B", "T1-B" ] |> Map.ofList

  let t2 =
    [ "A", "T2-A"; "C", "T2-C"; "D", "T2-D" ]
    |> Map.ofList

  let combine key (_ : string) (_ : string) = "BOTH-" + key

  let t = Map.merge_skewed combine t1 t2

  Assert.AreEqual(t, Map.merge_skewed combine t2 t1)
  Assert.AreEqual(Map.toList t, [ "A", "BOTH-A"; "B", "T1-B"; "C", "T2-C"; "D", "T2-D" ])
