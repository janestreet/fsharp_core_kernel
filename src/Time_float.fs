module Core_kernel.Time_float

open System
open Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Unix.Time_float

module Stable =
  module V1 =
    type t = Stable.V1.t

type t = Stable.V1.t

let epoch = DateTime(1970, 1, 1, 0, 0, 0, 0)

let to_datetime t =
  (* We use [DateTime.AddTicks] instead of [DateTime.AddSeconds] because [AddSeconds]
    round the result to the nearest millisecond. *)
  epoch.AddTicks(int64 (t * 1E7)).ToLocalTime()

let of_datetime (datetime : DateTime) =
  let elapsed = datetime.ToUniversalTime() - epoch
  elapsed.TotalSeconds

let of_excel_time oa_date = DateTime.FromOADate(oa_date) |> of_datetime

let to_excel_time t =
  let datetime = to_datetime t
  datetime.ToOADate()
