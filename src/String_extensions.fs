module Core_kernel.String_extensions

open System
open System.Runtime.CompilerServices

[<Extension>]
type String_extensions =
  [<Extension>]
  static member inline chop_prefix(this : string, prefix : string) =
    if this.StartsWith(prefix) then
      this.Substring(prefix.Length) |> Some
    else
      None

  [<Extension>]
  static member inline rsplit2(this : string, on : char) =
    match this.LastIndexOf(on) with
    | -1 -> None
    | rindex -> Some(this.Substring(0, rindex), this.Substring(rindex + 1))
