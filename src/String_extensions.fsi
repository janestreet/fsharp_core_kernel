module Core_kernel.String_extensions

open System
open System.Runtime.CompilerServices

[<Extension; Class>]
type String_extensions =
  [<Extension>]
  static member inline chop_prefix : this : string * prefix : string -> string option

  [<Extension>]
  static member inline rsplit2 : this : string * on : char -> (string * string) option
