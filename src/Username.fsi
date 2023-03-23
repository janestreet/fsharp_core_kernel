module Core_kernel.Username

type t

val to_string : t -> string

val current : unit -> t
