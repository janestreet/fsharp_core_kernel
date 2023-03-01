module Core_kernel.Test.String_extensions

open NUnit.Framework
open System
open Core_kernel
open String_extensions

[<Test>]
let ``chop_prefix on valid inputs`` () =
  let prefix = "prefix"
  let suffix = "suffix"
  let joined = prefix + suffix
  Assert.AreEqual(Some suffix, joined.chop_prefix (prefix))
  Assert.AreEqual(Some joined, joined.chop_prefix (""))
  Assert.AreEqual(None, joined.chop_prefix ("not-present"))
  Assert.AreEqual(Some "", "".chop_prefix (""))
  Assert.AreEqual(None, "".chop_prefix ("not-present"))

[<Test>]
let ``chop_prefix on invalid inputs`` () =
  Assert.Throws<System.ArgumentNullException> (fun () ->
    "some string".chop_prefix null
    |> (ignore : string option -> unit))
  |> ignore

[<Test>]
let ``rsplit2`` () =
  let string = "^A@B@C$"

  Assert.AreEqual(Some("^A@B", "C$"), string.rsplit2 ('@'))
  Assert.AreEqual(Some("^A@B@C", ""), string.rsplit2 ('$'))
  Assert.AreEqual(Some("", "A@B@C$"), string.rsplit2 ('^'))
  Assert.AreEqual(None, string.rsplit2 ('?'))
