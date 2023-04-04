module Core_kernel.Pipe

open System.Threading.Tasks

type t<'a, 'phantom>

module Reader =
  type private reader = unit
  type 'a t = t<'a, reader>

module Writer =
  type private writer = unit
  type 'a t = t<'a, writer>

val create : unit -> 'a Reader.t * 'a Writer.t

/// closes the write end of [t]
/// - future calls to [write] will raise
/// - all future read attempts will drain the data in the pipe at the time of
///   [close] until the pipe is empty; subsequent reads will return [Eof]
val close : t<'a, _> -> unit

/// returns a Task which completes when [t] is closed
val closed : t<'a, _> -> Task
val closed_async : t<'a, _> -> unit Async

/// returns [True] if [close] has been called
val is_closed : t<'a, _> -> bool

/// raises if [writer] is closed
val write : 'a Writer.t -> 'a -> Task
val write_async : 'a Writer.t -> 'a -> unit Async

/// raises if [writer] is closed
val write_without_pushback : 'a Writer.t -> 'a -> unit

/// like [write_without_pushback] but doesn't raise if [writer] is closed
val write_without_pushback_if_open : 'a Writer.t -> 'a -> unit

module Read_result =
  type 'a t =
    | Ok of 'a
    | Eof

/// returns [Eof] if pipe is completed
val read : 'a Reader.t -> Task<'a Read_result.t>
val read_async : 'a Reader.t -> Async<'a Read_result.t>

module Read_now_result =
  type 'a t =
    | Ok of 'a
    | Nothing_available
    | Eof

/// returns [Nothing_available] if pipe is not complete but empty;
/// returns [Eof] if pipe is complete
val read_now : 'a Reader.t -> 'a Read_now_result.t

/// repeately applies [f] to elements of reader, waiting for each call to [f]
/// to finish before continuing. The Task returned by [iter] completes when
/// the pipe is closed and the call to [f] on the final element in the pipe is
/// completed.
val iter : 'a Reader.t -> f : ('a -> Task) -> Task

val iter_async : 'a Reader.t -> f : ('a -> unit Async) -> unit Async

/// consumes elements from the input reader, applies [f] to said element, then
/// makes results available via a new reader pipe. Closing the original reader
/// or writer pipe closes the returned pipe.
val map : 'a Reader.t -> f : ('a -> 'b) -> 'b Reader.t

/// reads everything into a list that's returned when the pipe is closed
val to_list : 'a Reader.t -> 'a list Task
