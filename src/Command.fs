module Core_kernel.Command

module Arg_type =
  type 'a t = { parse : string -> 'a }
  let create of_string = { parse = of_string }

module Flag =
  module Info =
    type t =
      { name : string
        doc : string
        aliases : string list
        no_arg : bool }

  type 'a t =
    { read : string option -> 'a
      info : Info.t }

  let required
    (arg_type : 'a Arg_type.t)
    (name : string)
    (doc : string)
    (aliases : string list)
    =
    { read =
        (function
        | Some arg -> (arg_type.parse arg)
        | None ->
          failwith (
            "Required flag "
            + name
            + " not supplied, refer to -help"
          ))
      info =
        { name = name
          doc = doc
          aliases = aliases
          no_arg = false } }

  let optional
    (arg_type : 'a Arg_type.t)
    (name : string)
    (doc : string)
    (aliases : string list)
    =
    { read =
        (function
        | Some arg -> Some(arg_type.parse arg)
        | None -> None)
      info =
        { name = name
          doc = doc
          aliases = aliases
          no_arg = false } }

  let no_arg (name : string) (doc : string) (aliases : string list) =
    { read =
        (function
        | Some _ -> true
        | None -> false)
      info =
        { name = name
          doc = doc
          aliases = aliases
          no_arg = true } }

module Parser =
  type 'a t = string list -> 'a * string list

  let error_message flag =
    "Unknown flag "
    + flag
    + ", refer to -help for possible flags"

  let bind (x : 'a t) (f : 'a -> 'b t) =
    fun (args : string list) ->
      let parser_type, args = x args

      if not (List.isEmpty args) then
        failwith (error_message args[0])

      (f parser_type) args

  let map (x : 'a t) (f : 'a -> 'b) =
    fun (args : string list) ->
      let parser_type, args = x args

      if not (List.isEmpty args) then
        failwith (error_message args[0])

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

  let rec check_aliases (args : string list) (aliases : string list) =
    match aliases with
    | head :: tail ->
      match List.tryFindIndex (fun flag -> head = flag) args with
      | Some index -> Some(index)
      | None -> check_aliases args tail
    | [] -> None

  let flag (f : 'a Flag.t) =
    { parser =
        fun args ->
          if f.info.no_arg then
            match List.tryFindIndex (fun flag -> f.info.name = flag) args with
            | Some index -> f.read (Some(args[index])), List.removeAt index args
            | None ->
              match (check_aliases args f.info.aliases) with
              | Some index -> f.read (Some(args[index])), List.removeAt index args
              | None -> f.read None, args
          else
            match List.tryFindIndex (fun flag -> f.info.name = flag) args with
            | Some index ->
              let args = List.removeAt index args
              f.read (Some(args[index])), List.removeAt index args
            | None ->
              match (check_aliases args f.info.aliases) with
              | Some index ->
                let args = List.removeAt index args
                f.read (Some(args[index])), List.removeAt index args
              | None -> f.read None, args
      flags = [ f.info ] }


  let bind (x : 'a t) (f : 'a -> 'b t) =
    let f x =
      let f_param = f x
      f_param.parser

    { parser = Parser.bind x.parser f
      flags = x.flags }

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
    member __.Bind(x : 'a t, f : 'a -> 'b t) = bind x f
    member __.BindReturn(x : 'a t, f : 'a -> 'b) = map x f
    member __.MergeSources(x : 'a t, y : 'b t) = both x y
    member __.Return(x : 'a) = return_ x
    member __.Zero() = zero ()

  let let_syntax = ResultBuilder()

let run_exn (param : unit Param.t) (args : string list) =
  if List.contains "-help" args
     || List.contains "--h" args then
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
    (* Since unit Param.t always returns a unit and there should be no remaining arguments in the string list,
           we can ignore what is returned here *)
    let (_ : unit), (_ : string list) = Param.parse param args
    ()

let run (param : unit Param.t) (args : string list) =
  match Or_error.try_with (fun () -> run_exn param args) with
  | Ok _ -> 0
  | Error error ->
    printfn "%A" error
    1
