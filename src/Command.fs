module Core_kernel.Command

module Arg_type =
    type 'a t = { parse: string -> 'a }
    let create of_string = { parse = of_string }

module Flag =
    type 'a t =
        { read: string list -> 'a
          name: string
          doc: string
          required: bool
          no_arg: bool }

    let required (arg_type: 'a Arg_type.t) (name: string) (doc: string) =
        { read = (fun arg -> (arg_type.parse (List.exactlyOne arg))) // CR-soon Jade exactlyOne will change to Num_occurrences later for a more specific error message
          name = name
          doc = doc
          required = true
          no_arg = false }

    let optional (arg_type: 'a Arg_type.t) (name: string) (doc: string) =
        { read =
            (fun arg ->
                if List.length arg > 0 then
                    Some(arg_type.parse (List.exactlyOne arg))
                else
                    None)
          name = name
          doc = doc
          required = false
          no_arg = false }

    let no_arg (name: string) (doc: string) =
        { read = (fun flag -> not (List.isEmpty flag))
          name = name
          doc = doc
          required = false
          no_arg = true }

module Parser =
    type 'a t = string list -> 'a * string list
  
    let bind (x: 'a t) (f: 'a -> 'b t) =
        fun (args: string list) ->
            let parser_type, args = x args
            if not (List.isEmpty args) then 
                failwith ("Unknown flag " + args[0] + ", refer to -help for possible flags")
            (f parser_type) args

    let map (x: 'a t) (f: 'a -> 'b) =
        fun (args: string list) ->
            let parser_type, args = x args
            if not (List.isEmpty args) then 
                failwith ("Unknown flag " + args[0] + ", refer to -help for possible flags")
            f parser_type, args

    let both (x: 'a t) (y: 'b t) : ('a * 'b) t =
        fun (args: string list) ->
            let x_type, x_args = x args
            let y_type, y_args = y x_args
            (x_type, y_type), y_args

    let return_ (x: 'a) = fun (args: string list) -> x, args
    let zero () = fun (args: string list) -> (), args

module Param =
    type 'a t =
        { parser: 'a Parser.t
          flags: (string * string) list
          anons: string list }

    let parse (flag: 'a t) (args: string list) =
        flag.parser args 

    let flag (f: 'a Flag.t) =
        { parser =
            fun args ->
                if List.exists (fun flag -> f.name = flag) args
                   && not f.no_arg then
                    let flag_index = List.findIndex (fun flag -> f.name = flag) args
                    let args = List.removeAt flag_index args
                    f.read [ args[flag_index] ], List.removeAt flag_index args
                else if f.required = true && not f.no_arg then
                    failwith "Required arg not supplied, refer to -help"
                else if List.exists (fun flag -> f.name = flag) args then
                    let flag_index = List.findIndex (fun flag -> f.name = flag) args
                    f.read [args[0]], List.removeAt flag_index args
                else 
                    f.read [], args
          flags = [ (f.name, f.doc) ]
          anons = [] }


    let bind (x: 'a t) (f: 'a -> 'b t) =
        let f x =
            let f_param = f x
            f_param.parser
        { parser = Parser.bind x.parser f
          flags = x.flags
          anons = x.anons }

    let map (x: 'a t) (f: 'a -> 'b) =
        { parser = Parser.map x.parser f
          flags = x.flags
          anons = x.anons }

    let both (x: 'a t) (y: 'b t) : ('a * 'b) t =
        { parser = Parser.both x.parser y.parser
          flags = x.flags @ y.flags
          anons = x.anons @ y.anons }

    let return_ (x: 'a) =
        { parser = Parser.return_ x
          flags = []
          anons = [] }

    let zero () =
        { parser = Parser.zero ()
          flags = []
          anons = [] }

    [<Sealed>]
    type ResultBuilder() =
        member __.Bind(x: 'a t, f: 'a -> 'b t) = bind x f
        member __.BindReturn(x: 'a t, f: 'a -> 'b) = map x f
        member __.MergeSources(x: 'a t, y: 'b t) = both x y
        member __.Return(x: 'a) = return_ x
        member __.Zero() = zero ()

    let let_syntax = ResultBuilder()

let run_exn (param: unit Param.t) (args: string list) = 
    if List.contains "-help" args then 
        printfn ""
        printfn "possible flags:"
        printfn ""
        let possible_flags = param.flags
        for flag in possible_flags do
            let name, doc = flag
            printfn "%-*s %s" 20 name doc 
        printfn ""
        0
    else 
        let _, _ = Param.parse param args
        0

let run (param: unit Param.t) (args: string list) =
    match Or_error.try_with (fun () -> run_exn param args) with 
    | Ok exit_code -> exit_code
    | Error error -> 
        printfn "%A" error 
        1
     