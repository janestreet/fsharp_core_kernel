module Core_kernel.Test.Pipe

open NUnit.Framework
open System
open Core_kernel

let assert_ok_read_result expected actual =
  let expected = Pipe.Read_result.Ok expected
  Assert.AreEqual(expected, actual)

let assert_ok_read_now_result expected actual =
  let expected = Pipe.Read_now_result.Ok expected
  Assert.AreEqual(expected, actual)

[<Test>]
let ``Pipe.read, Pipe.write`` () =
  let reader, writer = Pipe.create ()

  task {
    do! Pipe.write writer 1

    let! result = Pipe.read reader
    assert_ok_read_result 1 result

    do! Pipe.write writer 2
    Pipe.close writer

    let! result = Pipe.read reader
    assert_ok_read_result 2 result

    let! result = Pipe.read reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)
  }

[<Test>]
let ``Pipe.write_without_pushback`` () =
  let reader, writer = Pipe.create ()

  task {
    Pipe.write_without_pushback writer 1

    let! result = Pipe.read reader
    assert_ok_read_result 1 result

    Pipe.write_without_pushback writer 2
    Pipe.close writer

    let! result = Pipe.read reader
    assert_ok_read_result 2 result

    let! result = Pipe.read reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)

    let _ : Exception =
      Assert.Throws<Exception>(fun () -> Pipe.write_without_pushback writer 3)

    Assert.AreEqual((), Pipe.write_without_pushback_if_open writer 3)

    let! result = Pipe.read reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)
  }

[<Test>]
let ``Pipe.read_now`` () =
  let reader, writer = Pipe.create ()

  task {
    let result = Pipe.read_now reader

    Assert.AreEqual(
      (Pipe.Read_now_result.Nothing_available : int Pipe.Read_now_result.t),
      result
    )

    do! Pipe.write writer 1

    let result = Pipe.read_now reader

    assert_ok_read_now_result 1 result

    do! Pipe.write writer 2
    Pipe.close writer

    let result = Pipe.read_now reader
    assert_ok_read_now_result 2 result

    let result = Pipe.read_now reader
    Assert.AreEqual((Pipe.Read_now_result.Eof : int Pipe.Read_now_result.t), result)
  }

[<Test>]
let ``Pipe.is_closed, close, closed`` () =
  let reader, writer = Pipe.create ()

  Assert.False(Pipe.is_closed reader)
  Assert.False(Pipe.is_closed writer)

  task {
    Pipe.close reader
    do! Pipe.closed reader

    Assert.True(Pipe.is_closed reader)
    Assert.True(Pipe.is_closed writer)

    let reader, writer = Pipe.create ()
    Pipe.close writer
    do! Pipe.closed reader

    Assert.True(Pipe.is_closed reader)
    Assert.True(Pipe.is_closed writer)
  }

[<Test>]
let ``Pipe.iter`` () =
  let reader, writer = Pipe.create ()
  let values = [ 1..10 ]

  task {
    // await in order
    for value in values do
      do! Pipe.write writer value

    Pipe.close writer
    let mutable result = []

    do! Pipe.iter reader (fun value -> (task { result <- List.append result [ value ] }))

    Assert.AreEqual(result, values)

    let! result = Pipe.read reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)
  }

[<Test>]
let ``Pipe.iter_async`` () =
  let reader, writer = Pipe.create ()
  let values = [ 1..10 ]

  async {
    // await in order
    for value in values do
      do! Pipe.write_async writer value

    Pipe.close writer
    let mutable result = []

    do!
      Pipe.iter_async reader (fun value ->
        (async { result <- List.append result [ value ] }))

    Assert.AreEqual(result, values)

    let! result = Pipe.read_async reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)
  }

[<Test>]
let ``Pipe.map`` () =
  let reader, writer = Pipe.create ()
  let values = [ 1..10 ]
  let mult_by_10 i = i * 10

  task {
    let mapped_reader = Pipe.map reader mult_by_10

    do! Pipe.write writer 1
    let value = (Pipe.read mapped_reader).Result

    assert_ok_read_result 10 value

    // await in order
    for value in values do
      do! Pipe.write writer value

    Pipe.close writer
    let mutable result = []

    do!
      Pipe.iter mapped_reader (fun value ->
        (task { result <- List.append result [ value ] }))

    let expected = List.map mult_by_10 values
    Assert.AreEqual(result, expected)

    let! result = Pipe.read mapped_reader
    Assert.AreEqual((Pipe.Read_result.Eof : int Pipe.Read_result.t), result)

    Assert.IsTrue(Pipe.is_closed mapped_reader)
  }
