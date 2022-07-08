module Core_kernel.Hostname

type t

val current : unit -> t

val to_string : t -> string
val of_string : string -> t
