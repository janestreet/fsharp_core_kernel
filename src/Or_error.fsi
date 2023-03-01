module Core_kernel.Or_error

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type 'a t = Result<'a, Error.t>

val combine_errors : 'a t list -> 'a list t
val try_with : (unit -> 'a) -> 'a t
val tagf : Printf.StringFormat<'a, ('b t -> 'b t)> -> 'a
val bin_t : 'a Bin_prot.Type_class.t -> 'a t Bin_prot.Type_class.t

/// This [Error] class exists so that we can collect line numbers and file paths and
/// append them to error messages.
[<Sealed>]
type Error =
  class
    static member string :
      message : string *
      [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string *
      [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
        _ t

    static member format :
      format : Printf.StringFormat<'a, 'b t> *
      [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string *
      [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
        'a
  end
