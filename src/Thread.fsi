module Core_kernel.Thread

type t = System.Threading.Thread

val spawn : desc : string -> (unit -> unit) -> t
val spawn_and_ignore : desc : string -> (unit -> unit) -> unit
