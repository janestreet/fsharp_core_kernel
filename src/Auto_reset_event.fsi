module Core_kernel.Auto_reset_event

open System.Threading

type t = AutoResetEvent

val wait_one : t -> unit

val wait_n : t -> n : int -> unit

val set : t -> unit
