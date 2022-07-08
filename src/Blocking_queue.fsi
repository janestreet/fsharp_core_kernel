module Core_kernel.Blocking_queue

module Writer =
  type 'a t
  val write : 'a t -> 'a -> unit

module Reader =
  type 'a t
  val read : 'a t -> 'a

/// A thread-safe queue with blocking read semantics
/// and separate reader and writer handles.
val create : unit -> 'a Reader.t * 'a Writer.t
