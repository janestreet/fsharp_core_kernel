module Core_kernel.Host_and_port

type t = { host : string; port : int }

val create : string -> int -> t
val of_string_exn : string -> t

val arg_type : t Command.Arg_type.t
