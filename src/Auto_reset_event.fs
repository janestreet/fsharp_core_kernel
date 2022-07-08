module Core_kernel.Auto_reset_event

open System.Threading

type t = AutoResetEvent

let wait_one (t : t) = ignore (t.WaitOne() : bool)

let rec wait_n (t : t) n =
  if n > 0 then
    wait_one t
    wait_n t (n - 1)

let set (t : t) = ignore (t.Set() : bool)
