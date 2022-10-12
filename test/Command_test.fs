module Core_kernel.Test.Command

open NUnit.Framework
open Core_kernel.Command
open System
open System.IO

module Req_test =
  type t = { length : int }

  let (req_arg : t Arg_type.t) =
    Arg_type.create (fun string -> { length = String.length string })

  let (req_flag : t Flag.t) =
    Flag.required
      (req_arg : t Arg_type.t)
      "-req"
      "Testing required arg: returns a record of the length of the arg string"
      [ "--r" ]

  let (req_param : t Param.t) = Param.flag (req_flag : t Flag.t)

module No_arg_test =
  type t = bool

  let (no_arg_flag : t Flag.t) =
    Flag.no_arg
      "-no_arg"
      "Testing no arg: returns true if flag is present, false otherwise"
      [ "--na"; "-no" ]

  let (no_arg_param : t Param.t) = Param.flag (no_arg_flag : t Flag.t)

module Opt_test =
  type t = string
  let (opt_arg : t Arg_type.t) = Arg_type.create (fun string -> string)

  let (opt_flag : t option Flag.t) =
    Flag.optional
      (opt_arg : t Arg_type.t)
      "-opt"
      "Testing optional arg: returns the arg string"
      [ "--o" ]

  let (opt_param : t option Param.t) = Param.flag (opt_flag : t option Flag.t)

module Test =
  type t = string
  let (test_arg : t Arg_type.t) = Arg_type.create (fun string -> string)

  let (test_flag : t Flag.t) =
    Flag.required (test_arg : t Arg_type.t) "-test" "Test flag doc" []

  let (test_param : t Param.t) = Param.flag (test_flag : t Flag.t)


let run_test
  (expected_output : (string * bool * string) list)
  (param : (string * No_arg_test.t * string) list Param.t)
  (args : string list)
  =
  let x, _ = Param.parse param args
  Assert.AreEqual(expected_output, x)

let test_printed_output
  (param : unit Param.t)
  (args : string list)
  (expected_exit_code : int)
  (expected_output : string list)
  (lines_of_output : int)
  =
  use string_writer = new StringWriter()
  Console.SetOut(string_writer)
  Assert.AreEqual(expected_exit_code, (run param args))

  let output =
    string_writer.ToString().Split("\n")
    |> Array.toList
    |> List.take lines_of_output

  Assert.AreEqual(expected_output, output)

[<Test>]
[<Category("Command_tests")>]
let ``flags`` () =
  let create_arg_string req opt no_arg =
    match opt with
    | Some opt -> [ req.ToString(), no_arg, opt ]
    | None -> [ req.ToString(), no_arg, "no opt" ]

  let req_param = Req_test.req_param
  let no_arg_param = No_arg_test.no_arg_param
  let opt_param = Opt_test.opt_param

  let x =
    Param.let_syntax {
      let! (req : Req_test.t) = (req_param : Req_test.t Param.t)
      and! (no_arg : No_arg_test.t) = (no_arg_param : No_arg_test.t Param.t)
      and! (opt : Opt_test.t option) = (opt_param : Opt_test.t option Param.t)

      return create_arg_string req opt no_arg
    }

  let required_args = [ "{ length = 4 }", false, "no opt" ]
  run_test required_args x [ "-req"; "test" ]

  let optional_args = [ "{ length = 4 }", false, "opt" ]
  run_test optional_args x [ "-req"; "test"; "-opt"; "opt" ]

  let no_args = [ "{ length = 4 }", true, "no opt" ]
  run_test no_args x [ "-req"; "test"; "-no_arg" ]

  let aliases = [ "{ length = 4 }", true, "opt" ]
  run_test aliases x [ "--r"; "test"; "-no"; "--o"; "opt" ]

[<Test>]
[<Category("Command_tests")>]
let ``help_test`` () =
  let test_param = Test.test_param

  let x =
    Param.let_syntax {
      let! (test : Test.t) = (test_param : Test.t Param.t)
      return printf "%A" test
    }

  let expected_output =
    [ ""; "possible flags:"; ""; "-test                Test flag doc" ]

  test_printed_output x [ "-help" ] 0 expected_output 4
  test_printed_output x [ "--h" ] 0 expected_output 4

[<Test>]
[<Category("Command_tests")>]
let ``unknown_flag`` () =

  let test_param = Opt_test.opt_param

  let x =
    Param.let_syntax {
      let! (test : Test.t option) = (test_param : Test.t option Param.t)
      return printf "%A" test
    }

  let expected_output =
    [ "String"
      "  \"System.Exception: Unknown flag -unknown, refer to -help for possible flags" ]

  test_printed_output x [ "-unknown" ] 1 expected_output 2

[<Test>]
[<Category("Command_tests")>]
let ``no_required_arg`` () =
  let test_param = Test.test_param

  let x =
    Param.let_syntax {
      let! (test : Test.t) = (test_param : Test.t Param.t)
      return printf "%A" test
    }

  let expected_output =
    [ "String"; "  \"System.Exception: Required flag -test not supplied, refer to -help" ]

  test_printed_output x [] 1 expected_output 2
