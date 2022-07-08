module Core_kernel.Dictionary

open System.Collections.Generic

type t<'key, 'value> = Dictionary<'key, 'value>

let of_alist_exn (entries : ('key * 'value) list) (comparer : IEqualityComparer<'key>) =
  let t = new Dictionary<'key, 'value>(comparer)
  List.iter (fun (key, value) -> t.Add(key, value)) entries
  t

let find (t : t<_, _>) key =
  let res, value = t.TryGetValue key
  if res then Some value else None

let remove (t : t<_, _>) key =
  let was_removed = t.Remove key
  ignore (was_removed : bool)

let set (t : t<_, _>) key value = t.[key] <- value

let iter f (t : t<_, _>) =
  for KeyValue (k, v) in t do
    f k v
