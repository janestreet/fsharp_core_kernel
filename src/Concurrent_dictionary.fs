module Core_kernel.Concurrent_dictionary

open System.Collections.Concurrent

type t<'key, 'value> = ConcurrentDictionary<'key, 'value>

let find key (t : t<_, _>) =
  let res, value = t.TryGetValue key
  if res then Some value else None

let find_exn key t = find key t |> Option.get

let remove_exn key (t : t<_, _>) =
  let res, value = t.TryRemove key

  if not res then
    failwithf "Failure removing key %A %A" t key
  else
    value

let add_exn key value (t : t<_, _>) =
  let res = t.TryAdd(key, value)

  if not res then
    failwithf "Key already exists %A %A" t key

let update key f (t : t<'key, _>) =
  t.AddOrUpdate(key, (fun (_ : 'key) -> f None), (fun (_ : 'key) value -> f (Some value)))
