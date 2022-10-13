module Core_kernel.Command

module Arg_type =
  type 'a t = { parse : string -> 'a }
  let create of_string = { parse = of_string }

module Flag =
  module Info =
    type t =
      { name : string
        doc : string
        no_arg : bool }

  type 'a t =
    { read : string option -> 'a
      info : Info.t }

  let required (arg_type : 'a Arg_type.t) (name : string) (doc : string) =
    { read =
        (function
        | Some arg -> (arg_type.parse arg)
        | None -> failwithf $"Required flag {name} not supplied, refer to -help")
      info =
        { name = name
          doc = doc
          no_arg = false } }

  let optional (arg_type : 'a Arg_type.t) (name : string) (doc : string) =
    { read = (fun x -> Option.map arg_type.parse x)
      info =
        { name = name
          doc = doc
          no_arg = false } }

  let no_arg (name : string) (doc : string) =
    { read = Option.isSome
      info =
        { name = name
          doc = doc
          no_arg = true } }

module Parser =
  type 'a t = string list -> 'a * string list

  let error_message flag =
    $"Unknown flag {flag}, refer to -help for possible flags"

  let map (x : 'a t) (f : 'a -> 'b) =
    fun (args : string list) ->
      let parser_type, args = x args

      if not (List.isEmpty args) then
        failwith (error_message args.[0])

      f parser_type, args

  let both (x : 'a t) (y : 'b t) : ('a * 'b) t =
    fun (args : string list) ->
      let x_type, x_args = x args
      let y_type, y_args = y x_args
      (x_type, y_type), y_args

  let return_ (x : 'a) = fun (args : string list) -> x, args
  let zero () = fun (args : string list) -> (), args

module Param =
  type 'a t =
    { parser : 'a Parser.t
      flags : Flag.Info.t list }

  let parse (t : 'a t) (args : string list) = t.parser args

  let flag (f : 'a Flag.t) =
    { parser =
        fun args ->
          if f.info.no_arg then
            match List.tryFindIndex (fun flag -> f.info.name = flag) args with
            | Some index -> f.read (Some(args.[index])), List.removeAt index args
            | None -> f.read None, args
          else
            match List.tryFindIndex (fun flag -> f.info.name = flag) args with
            | Some index ->
              let args = List.removeAt index args
              f.read (Some(args.[index])), List.removeAt index args
            | None -> f.read None, args
      flags = [ f.info ] }

  let map (x : 'a t) (f : 'a -> 'b) =
    { parser = Parser.map x.parser f
      flags = x.flags }

  let both (x : 'a t) (y : 'b t) : ('a * 'b) t =
    { parser = Parser.both x.parser y.parser
      flags = x.flags @ y.flags }

  let return_ (x : 'a) =
    { parser = Parser.return_ x
      flags = [] }

  let zero () = { parser = Parser.zero (); flags = [] }

  [<Sealed>]
  type ResultBuilder () =
    member __.BindReturn(x : 'a t, f : 'a -> 'b) = map x f
    member __.MergeSources(x : 'a t, y : 'b t) = both x y
    member __.Return(x : 'a) = return_ x
    member __.Zero() = zero ()

  let let_syntax = ResultBuilder()

let run_exn (param : unit Param.t) (args : string list) =
  if List.contains "-help" args then
    printfn ""
    printfn "possible flags:"
    printfn ""
    let possible_flags = param.flags

    for flag in possible_flags do
      let name = flag.name
      let doc = flag.doc
      printfn "%-*s %s" 20 name doc

    printfn ""
  else
    // We ignore the rest of the string instead of asserting that it's empty because we do
    // the assertion in [Parser.map]. This allows us to halt the execution of the program
    // (at this point in the code the function would have already ran so it's too late to
    // error out)
    let (), (_ : string list) = Param.parse param args
    ()

let run (param : unit Param.t) (args : string list) =
  match Or_error.try_with (fun () -> run_exn param args) with
  | Ok _ -> 0
  | Error error ->
    printfn "%A" error
    1
