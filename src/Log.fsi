module Core_kernel.Log

open System.Runtime.CompilerServices
open System.Runtime.InteropServices


open System

/// [printfn] which additionally prepends the message with [DateTime.Now]
val printfn : Printf.TextWriterFormat<'a> -> 'a

[<Sealed>]
type Limited =
  class
    static member printfn_every_n :
      n : int
      * format : Printf.StringFormat<'a, unit>
      * [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string
      * [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
      'a
  end
