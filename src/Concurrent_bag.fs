module Core_kernel.Concurrent_bag

open System.Collections.Concurrent

type 'a t = 'a ConcurrentBag

let take (t : _ t) =
  let res, value = t.TryTake()
  if res then Some value else None
