module Core_kernel.Thread

open System.Threading

// Copied from [catalog_addin_network_connection/src/Threading.fs], simplified the
// logging printout, and no longer return the thread since we always ignore it.

type t = Thread

let spawn (desc : string) f =
  let f () =
    try
      f ()
    with
    | :? System.Threading.ThreadAbortException -> ()
    | e -> System.Console.WriteLine("thread {0} died with exception {1}", desc, e)

  let th = new Thread(f)
  th.Start()
  th

let spawn_and_ignore desc f = spawn desc f |> (ignore : t -> unit)

let blocking_section sync f =
  Monitor.Exit sync

  try
    let res = f ()
    Monitor.Enter sync
    res
  with
  | exn ->
    Monitor.Enter sync
    raise exn
