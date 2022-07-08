module Core_kernel.Test.System_information

open NUnit.Framework
open Core_kernel

[<Test>]
let ``Requesting groups does not crash and returns non-empty`` () =
  let groups = User_groups.of_current_user () |> Result.ok_exn
  List.isEmpty groups |> Assert.False
