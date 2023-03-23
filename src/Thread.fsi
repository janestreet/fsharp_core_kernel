module Core_kernel.Thread

type t = System.Threading.Thread

val spawn : desc : string -> (unit -> unit) -> t
val spawn_and_ignore : desc : string -> (unit -> unit) -> unit

/// [blocking_section sync f] Unlock [sync], run [f], and then lock [sync] again. If [f]
/// raises an exception it will be raised after [sync] is re-locked. Useful if you want to
/// let others run while you do IO.
val blocking_section : 'a -> (unit -> 'b) -> 'b
