module Core_kernel.Error

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type t = Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.t

val to_string : t -> string
val of_list : t list -> t
val bin_t : t Bin_prot.Type_class.t
val raise : t -> 'a
val tag : string -> t -> t
val tagf : Printf.StringFormat<'a, (t -> t)> -> 'a

/// This [Of] class exists so that we can collect line numbers and file paths and
/// append them to error messages.
[<Sealed>]
type Of =
  class
    static member string :
      message : string
      * [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string
      * [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
      t

    static member format :
      format : Printf.StringFormat<'a, t>
      * [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string
      * [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
      'a
  end
