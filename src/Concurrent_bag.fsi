module Core_kernel.Concurrent_bag

open System.Collections.Concurrent

type 'a t = 'a ConcurrentBag

val take : 'a t -> 'a option
