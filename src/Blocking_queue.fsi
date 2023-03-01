module Core_kernel.Blocking_queue

module Writer =
  type 'a t
  val write : 'a t -> 'a -> unit

module Reader =
  type 'a t
  val read : 'a t -> 'a

  val try_read : 'a t -> timeout : System.TimeSpan -> 'a option

  val try_read_with_cancellation :
    'a t -> timeout : System.TimeSpan -> System.Threading.CancellationToken -> 'a option

  val count : 'a t -> int

/// A thread-safe queue with blocking read semantics
/// and separate reader and writer handles.
val create : unit -> 'a Reader.t * 'a Writer.t
