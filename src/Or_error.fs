module Core_kernel.Or_error

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type 'a t = Result<'a, Error.t>

let combine_errors l =
  Result.mapError Error.of_list (Result.combine_errors l)

let tagf tag =
  Printf.ksprintf (fun tag -> fun t -> Result.mapError (Error.tag tag) t) tag

let bin_t bin_a =
  Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Or_error.T.bin_t
    bin_a

[<Sealed>]
type Error () =
  static member string
    (
      message : string,
      [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string,
      [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int
    ) =
    Result.Error(Error.Of.string (message, path, line))

  static member format
    (
      format,
      [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string,
      [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int
    ) =
    Printf.ksprintf (fun s -> Result.Error(Error.Of.string (s, path, line))) format

let try_with f =
  try
    Ok(f ())
  with
  | error -> Error.format "%A" error
