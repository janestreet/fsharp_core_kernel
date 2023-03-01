module Core_kernel.Host_and_port

type t = { host : string; port : int }

let create host port = { host = host; port = port }

module Parser =
  open Integer.ActivePatterns

  let of_string_exn (str : string) =
    match str.Split ':' with
    | [| host; ParseInteger port |] -> { host = host; port = port }
    | (_ : string array) -> failwithf "not a host and port %s" str

let of_string_exn s = Parser.of_string_exn s

let arg_type = Command.Arg_type.create of_string_exn
