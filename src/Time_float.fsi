module Core_kernel.Time_float

open Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Unix.Time_float

type t = Stable.V1.t

val to_datetime : t -> System.DateTime
val of_datetime : System.DateTime -> t

(* Excel uses OLE Automation Date format which is what is stored in the [double]. *)
val of_excel_time : oa_date : double -> t
val to_excel_time : t -> double

module Stable =
  module V1 =
    type t = Stable.V1.t
