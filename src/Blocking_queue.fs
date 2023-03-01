module Core_kernel.Blocking_queue

open System.Threading
open System.Collections.Concurrent

let option_some_if_tuple (success, result) = if success then Some result else None

// By putting the [out] param as the last item, the F# compiler desugars the
// out param for us.
type System.Collections.Concurrent.BlockingCollection<'a> with
  member private x.TryTakeOutParam
    (
      (timeout : System.TimeSpan),
      ([<System.Runtime.InteropServices.Out>] output : byref<'a>)
    ) =
    // TryTakeOutParam is a workaround because we can't define the same member twice
    // calling each other with different overloads, it seems (we get FS0001 from the
    // compiler.)
    x.TryTake(&output, timeout)

  member private x.TryTakeOutParam
    (
      (timeout : System.TimeSpan),
      (cancellation_token : System.Threading.CancellationToken),
      ([<System.Runtime.InteropServices.Out>] output : byref<'a>)
    ) =
    // Same as above for cancellation token overload.
    x.TryTake(&output, int timeout.TotalMilliseconds, cancellation_token)

  member x.TryTake
    (
      (timeout : System.TimeSpan),
      (cancellation_token : CancellationToken)
    ) =
    x.TryTakeOutParam(timeout, cancellation_token)
    |> option_some_if_tuple

  member x.TryTake(timeout : System.TimeSpan) =
    x.TryTakeOutParam(timeout) |> option_some_if_tuple

type 'a queue = 'a BlockingCollection

module Writer =
  type 'a t = T of 'a queue

  let write (T t) value = t.Add value

module Reader =
  type 'a t = T of 'a queue

  let read (T t) = t.Take()

  let try_read (T t) (timeout : System.TimeSpan) = t.TryTake(timeout)

  let try_read_with_cancellation (T t) (timeout : System.TimeSpan) cancellation_token =
    t.TryTake(timeout, cancellation_token)

  let count (T t) = t.Count

let create () =
  let t = new BlockingCollection<_>()
  Reader.T t, Writer.T t
