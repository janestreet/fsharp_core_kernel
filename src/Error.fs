module Core_kernel.Error

open System.Diagnostics
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module Error = Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T

type t = Error.t

let bin_t = Error.bin_t

let to_string t =
  match t with
  | Error.String x -> x
  | (_ : t) -> sprintf "%A" t

let of_list ts =
  let trunc_after_unused_for_now = None
  t.Of_list(trunc_after_unused_for_now, ts)

let create_string x = Error.String x

let create_with_path_and_line (message : string) (path : string) (line : int) =
  message
  + " ("
  + path
  + ":"
  + line.ToString()
  + ")"
  |> create_string

let tag tag t = Error.Tag_t(tag, t)

let tagf tag =
  Printf.ksprintf (fun tag -> fun t -> Error.Tag_t(tag, t)) tag

[<Sealed>]
type Of () =
  static member string(message : string, path : string, line : int) =
    create_with_path_and_line message path line

  static member format(format : Printf.StringFormat<'a, t>, path : string, line : int) =
    Printf.ksprintf (fun x -> create_with_path_and_line x path line) format

let create x = create_string x
let createf format = Printf.ksprintf create_string format
let raise t = raise (System.Exception(to_string t))
