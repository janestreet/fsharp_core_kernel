module Core_kernel.Timing_wheel

open System

module Action =
  type t

/// A [Timing_wheel.t] is a thread on which you can schedule future actions. The thread
/// sleeps until a new action is added or until the closest scheduled action is due.
type t

val create : unit -> t
val schedule_action : t -> delay : TimeSpan -> action : (unit -> unit) -> Action.t

/// Note that there is no guarantee that when you run this function, the action
/// hasn't already started.
val try_cancel : t -> Action.t -> unit

val close : t -> unit
