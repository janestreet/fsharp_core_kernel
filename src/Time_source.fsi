module Core_kernel.Time_source

type t =
  abstract member now : unit -> System.DateTime
  abstract member sleep_for : System.TimeSpan -> unit

type time_source = t

module Wall_clock =
  type t =
    new : unit -> t
    interface time_source

module Controllable =
  type t =
    new : System.DateTime -> t
    member advance_immediately_to : System.DateTime -> unit
    member advance_immediately_by : System.TimeSpan -> unit

    /// If a test advances time with the intent of waking up a sleeping thread,
    /// it must first wait for a sleep in order to avoid a race whereby it advances
    /// time before the thread starts sleeping.
    member advance_after_sleep_to : System.DateTime -> unit
    member advance_after_sleep_by : System.TimeSpan -> unit

    interface System.IDisposable
    interface time_source

module Constant =
  type t =
    new : System.DateTime -> t
    interface time_source

  val dotnet_epoch : t
