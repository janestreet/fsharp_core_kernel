module Core_kernel.Blocking_queue

open System.Collections.Concurrent

type 'a queue = 'a BlockingCollection

module Writer =
  type 'a t = T of 'a queue

  let write (T t) value = t.Add value

module Reader =
  type 'a t = T of 'a queue

  let read (T t) = t.Take()

let create () =
  let t = new BlockingCollection<_>()
  Reader.T t, Writer.T t
