module Core_kernel.Waithandle_extensions

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

[<Extension; Class>]
type Waithandle_extensions =
  [<Extension>]
  static member inline wait_for_signal : this : WaitHandle -> unit Task
