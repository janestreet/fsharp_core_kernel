module Core_kernel.Thread_safe_run_once

open System.Threading

type t = T of ref<int>

let create () = T(ref 0)

let run_if_first_time (T t) f =
  if 0 = Interlocked.Exchange(t, 1) then
    f ()

let ran (T t) = 1 = t.Value
