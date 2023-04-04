module Core_kernel.Waithandle_extensions

open System.Threading
open System.Runtime.CompilerServices

[<Extension>]
type Waithandle_extensions =
  [<Extension>]
  static member inline wait_for_signal(this : WaitHandle) =
    async { this.WaitOne() |> ignore<bool> }
    |> Async.StartAsTask
