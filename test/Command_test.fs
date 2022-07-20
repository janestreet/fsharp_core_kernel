module Core_kernel.Test.Command

open NUnit.Framework
open Core_kernel.Command
open System
open System.IO

module Req_test = type t = { length : int }
module Opt_test = type t = string
module No_arg_test = type t = bool
module Test = type t = string

let run_test (expected_output : string) (param: string Param.t) (args: string list) =
    let x, _ = Param.parse param args
    Assert.AreEqual(expected_output, x)


[<Test>]
[<Category("Command_tests")>]
let ``flags`` () = 
    let create_arg_string req opt no_arg = 
        let no_arg = 
            if no_arg then "true"
            else "false"
        match opt with 
        | Some opt -> req.ToString () + opt + no_arg
        | None -> req.ToString () + no_arg

    
    let (req_arg : Req_test.t Arg_type.t) = Arg_type.create (fun string -> { length = String.length string} )
    let (req_flag : Req_test.t Flag.t) = Flag.required (req_arg : Req_test.t Arg_type.t) "-req" "Testing required arg: returns a record of the length of the arg string"
    let (req_param: Req_test.t Param.t) = Param.flag (req_flag : Req_test.t Flag.t) 

    let (no_arg_flag : No_arg_test.t Flag.t) = Flag.no_arg "-no_arg" "Testing no arg: returns true if flag is present, false otherwise"
    let (no_arg_param: No_arg_test.t Param.t) = Param.flag (no_arg_flag : No_arg_test.t Flag.t) 

    let (opt_arg : Opt_test.t Arg_type.t) = Arg_type.create (fun string -> string)
    let (opt_flag : Opt_test.t option Flag.t) = Flag.optional (opt_arg : Opt_test.t Arg_type.t) "-opt" "Testing optional arg: returns the arg string"
    let (opt_param: Opt_test.t option Param.t) = Param.flag (opt_flag : Opt_test.t option Flag.t) 

    let x = Param.let_syntax {
        let! (req : Req_test.t) = (req_param : Req_test.t Param.t) 
        and! (no_arg : No_arg_test.t) = (no_arg_param : No_arg_test.t Param.t)
        and! (opt : Opt_test.t option) = (opt_param : Opt_test.t option Param.t) 
        
        return create_arg_string req opt no_arg }

    let required_args = "{ length = 4 }false"
    run_test required_args x ["-req"; "test"]

    let optional_args = "{ length = 4 }optfalse"
    run_test optional_args x ["-req"; "test"; "-opt"; "opt"]

    let no_args = "{ length = 4 }true"
    run_test no_args x ["-req"; "test"; "-no_arg"]

[<Test>]
[<Category("Command_tests")>]
let ``help_test`` () = 
    use string_writer = new StringWriter()
    Console.SetOut(string_writer)

    let (test_arg : Test.t Arg_type.t) = Arg_type.create (fun string -> string)
    let (test_flag : Test.t Flag.t) = Flag.required (test_arg : Test.t Arg_type.t) "-test" "Test flag doc"
    let (test_param : Test.t  Param.t) = Param.flag (test_flag : Test.t Flag.t) 

    let x = Param.let_syntax {
        let! (test : Test.t) = (test_param : Test.t Param.t)
        return printf "%A" test
    }

    Assert.AreEqual(0, (run x ["-help"]))
    let output = string_writer.ToString()
    let expected_output = "
possible flags:

-test                Test flag doc

"
    Assert.AreEqual(output, expected_output)

[<Test>]
[<Category("Command_tests")>]
let ``unknown_flag`` () = 
    use string_writer = new StringWriter()
    Console.SetOut(string_writer)

    let (test_arg : Test.t Arg_type.t) = Arg_type.create (fun string -> string)
    let (test_flag : Test.t option Flag.t) = Flag.optional (test_arg : Test.t Arg_type.t) "-test" "Test flag doc"
    let (test_param : Test.t option Param.t) = Param.flag (test_flag : Test.t option Flag.t) 

    let x = Param.let_syntax {
        let! (test : Test.t option) = (test_param : Test.t option Param.t)
        return printf "%A" test
    } 

    Assert.AreEqual(1, run x ["-unknown"])
    let output = string_writer.ToString() 
    let expected_output = "String\n  \"System.Exception: Unknown flag -unknown, refer to -help for possible flags"
    Assert.AreEqual(output[0..83], expected_output)

[<Test>]
[<Category("Command_tests")>]
let ``no_required_arg`` () = 
    use string_writer = new StringWriter()
    Console.SetOut(string_writer)

    let (test_arg : Test.t Arg_type.t) = Arg_type.create (fun string -> string)
    let (test_flag : Test.t Flag.t) = Flag.required (test_arg : Test.t Arg_type.t) "-test" "Test flag doc"
    let (test_param : Test.t Param.t) = Param.flag (test_flag : Test.t Flag.t) 

    let x = Param.let_syntax {
        let! (test : Test.t) = (test_param : Test.t Param.t)
        return printf "%A" test
    } 

    Assert.AreEqual(1, run x [])
    let output = string_writer.ToString() 
    let expected_output = "String\n  \"System.Exception: Required arg not supplied, refer to -help"
    Assert.AreEqual(output[0..68], expected_output)