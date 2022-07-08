module Core_kernel.Thread

open System.Threading

// Copied from [catalog_addin_network_connection/src/Threading.fs], simplified the
// logging printout, and no longer return the thread since we always ignore it.

type t = Thread

let spawn desc f =
  let f () =
    try
      f ()
    with
    | :? System.Threading.ThreadAbortException -> ()
    | e -> printf "thread %s died with exception %O" desc e

  let th = new Thread(f)
  th.Start()
  th

let spawn_and_ignore desc f = spawn desc f |> (ignore : t -> unit)
