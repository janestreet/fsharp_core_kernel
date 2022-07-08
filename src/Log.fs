module Core_kernel.Log

open System
open System.Collections.Concurrent

let printfn format =
  // Example: 2022-03-02 13:03:14.765-05:00:
  printf "%s: " (DateTime.Now.ToString "yyyy-MM-dd HH:mm:ss.fffK")
  printfn format

let global_counter : ConcurrentDictionary<string * int, int> = ConcurrentDictionary()

[<Sealed>]
type Limited () =
  static member printfn_every_n(n : int, format, path : string, line : int) =
    let i =
      Concurrent_dictionary.update
        (path, line)
        (function
        | None -> 0
        | Some i -> i + 1)
        global_counter

    Printf.ksprintf
      (fun message ->
        if i % n = 0 then
          printfn "%s" message
        else
          ())
      format
