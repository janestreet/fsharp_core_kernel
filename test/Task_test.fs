module Core_kernel.Test.Task_test

open NUnit.Framework
open Core_kernel

[<Test>]
let ``Repeat_until_finished repeats`` () =
  let mutable counter = 0

  let increase () =
    counter <- counter + 1
    counter

  let finished = "Finished!"

  let f state =
    task {
      if state < 5 then
        return Repeat(increase ())
      else
        return Finished finished
    }

  Assert.AreEqual((Task.repeat_until_finished counter f).Result, finished)
  Assert.AreEqual(counter, 5)

[<Test>]
let ``Task.Or_error.let_syntax binds correctly`` () =
  let first_error = task { return Or_error.Error.string "First error" }
  let second_error = task { return Or_error.Error.string "Second error" }

  let error_value =
    Task.Or_error.let_syntax {
      do! first_error
      do! second_error
      return ()
    }

  match error_value.Result with
  | Error err -> Error.assert_error_matches err "First error"
  | Ok () -> Assert.Fail()

  let return_ok = task { return Ok "Foo" }
  let ok_value = Task.Or_error.let_syntax { return! return_ok }

  match ok_value.Result with
  | Error err -> Assert.That(err, Is.Null)
  | Ok ok -> Assert.AreEqual(ok, "Foo")

  let raise_exn = task { return failwith "Raised" }

  let raised_value =
    Task.Or_error.let_syntax {
      try
        return! raise_exn
      with
      | ex -> return! task { return Or_error.Error.string $"Caught: {ex}" }
    }

  match raised_value.Result with
  | Error err -> Error.assert_error_matches err "Caught: System.Exception: Raised"
  | Ok () -> Assert.Fail()

  let raised_value =
    Task.Or_error.let_syntax {
      return!
        Task.Or_error.try_with (fun () -> first_error)
        |> Task.Or_error.tag "Caught"
    }

  match raised_value.Result with
  | Error err ->
    Assert.That(Error.to_string err, Contains.Substring("(Caught First error"))
  | Ok () -> Assert.Fail()
