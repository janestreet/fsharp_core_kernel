module Core_kernel.Pipe

open System.Threading
open System.Threading.Channels
open System.Threading.Tasks

type t<'a, 'phantom> =
  { channel : Channel<'a>
    writer_closed : TaskCompletionSource<unit> }

module Reader =
  type reader = unit
  type 'a t = t<'a, reader>

module Writer =
  type writer = unit
  type 'a t = t<'a, writer>

let create () : 'a Reader.t * 'a Writer.t =
  let options = UnboundedChannelOptions()
  // allows for some speed optimizations (e.g. avoids locks for reads)
  options.SingleWriter <- true
  options.SingleReader <- true

  let channel = Channel.CreateUnbounded(options)
  let writer_closed = TaskCompletionSource<unit>()

  let t =
    { channel = channel
      writer_closed = writer_closed }

  t, t

let is_closed (t : t<'a, _>) = t.writer_closed.Task.IsCompleted

let is_open (t : t<'a, _>) = is_closed t |> not

let closed (t : t<'a, _>) : Task = t.writer_closed.Task
let closed_async (t : t<'a, _>) : unit Async = closed t |> Async.AwaitTask

let close (t : t<'a, _>) =
  if is_open t then
    t.writer_closed.SetResult(())
    let writer = t.channel.Writer
    // Note that after writer.Complete is called, reader.Completion
    // completes only after the channel is fully drained.
    writer.Complete()

let write (t : 'a Writer.t) value =
  match is_closed t with
  | true -> failwith $"Attempted write of {value} to a closed pipe"
  | false ->
    let writer = t.channel.Writer

    writer
      .WriteAsync(value, CancellationToken.None)
      .AsTask()

let write_async (t : 'a Writer.t) value = write t value |> Async.AwaitTask

let write_without_pushback (t : 'a Writer.t) value =
  match is_closed t with
  | true -> failwith $"Attempted write of {value} to a closed pipe"
  | false ->
    let writer = t.channel.Writer

    // [TryWrite] can fail on bounded-size channels. We define Pipe with
    // unbounded channels so this should never happen.
    match writer.TryWrite value with
    | true -> ()
    | false -> failwith $"Unexpected error when writing {value}!"

module Read_result =
  type 'a t =
    | Ok of 'a
    | Eof

let read (t : 'a Reader.t) =
  task {
    let reader = t.channel.Reader

    // Will continue to read elements remaining in channel after a Complete
    match! reader.WaitToReadAsync CancellationToken.None with
    | false -> return Read_result.Eof
    | true ->
      let ((_ : bool), value) = reader.TryRead()
      return Read_result.Ok value
  }

let read_async (t : 'a Reader.t) = read t |> Async.AwaitTask

module Read_now_result =
  type 'a t =
    | Ok of 'a
    | Nothing_available
    | Eof

let read_now (t : 'a Reader.t) =
  let reader = t.channel.Reader

  match reader.Completion.IsCompleted with
  | true -> Read_now_result.Eof
  | false ->
    match reader.TryRead() with
    | (true, value) -> Read_now_result.Ok value
    | (false, _) -> Read_now_result.Nothing_available

let iter (t : 'a Reader.t) (f : 'a -> Task) : Task =
  Task.repeat_until_finished () (fun () ->
    task {
      match! read t with
      | Read_result.Eof -> return (Task.Repeat_or_finished.Finished())
      | Read_result.Ok value ->
        do! f value
        return (Task.Repeat_or_finished.Repeat())
    })

let iter_async (t : 'a Reader.t) (f : 'a -> unit Async) : unit Async =
  let rec loop () =
    async {
      match! read_async t with
      | Read_result.Eof -> return ()
      | Read_result.Ok value ->
        do! f value
        return! (loop ())
    }

  loop ()
