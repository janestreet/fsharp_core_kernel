module Core_kernel.Test.Error

open NUnit.Framework
open Core_kernel

let sanitize (str : string) =
  str
    .Replace(__SOURCE_DIRECTORY__, "src-dir")
    .Replace(__SOURCE_FILE__, "src-file")

let assert_error_matches error pattern =
  Assert.That(error |> Error.to_string |> sanitize, Does.Match(pattern : string))

[<Test>]
let ``Source code position is inserted`` () =
  assert_error_matches (Error.Of.string "sample") """sample \(src-dir/src-file:[0-9]+\)"""
