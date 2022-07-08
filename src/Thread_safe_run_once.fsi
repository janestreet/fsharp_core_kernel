module Core_kernel.Thread_safe_run_once

type t

val create : unit -> t

val run_if_first_time : t -> f : (unit -> unit) -> unit

val ran : t -> bool
