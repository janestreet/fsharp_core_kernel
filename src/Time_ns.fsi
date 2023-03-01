module Core_kernel.Time_ns

open Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Unix.Time_ns

type t = Stable.V1.t

val to_datetime : t -> System.DateTime

val of_datetime : System.DateTime -> t

module Span =
  type t = Stable.Span.V2.t

  val of_sec : float -> t
  val to_sec : t -> float
  val to_time_span : t -> System.TimeSpan

val now : unit -> t

val diff : t -> t -> Span.t

module Stable =
  module V1 =
    type t = Stable.V1.t
