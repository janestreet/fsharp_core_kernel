module Core_kernel.Time_ns

open System
open Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Unix.Time_ns

module Stable =
  module V1 =
    type t = Stable.V1.t

type t = Stable.V1.t

let epoch = DateTime(1970, 1, 1, 0, 0, 0, 0)

let to_datetime t =
  (* [Time_ns] ticks are per nanosecond, [Datetime] ticks are per 100 nanoseconds. *)
  epoch.AddTicks(t / 100L).ToLocalTime()

let of_datetime (datetime : DateTime) =
  datetime.ToUniversalTime().Subtract(epoch).Ticks
  * 100L

module Span =
  type t = Stable.Span.V2.t

  let of_sec secs = int64 (secs * 1e9)
  let to_sec (t : t) = float t / 1e9

  let to_time_span (t : t) = TimeSpan.FromTicks(t / 100L)

let now () = of_datetime DateTime.Now

let diff (t1 : t) (t2 : t) = t1 - t2
