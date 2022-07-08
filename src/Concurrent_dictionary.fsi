module Core_kernel.Concurrent_dictionary

open System.Collections.Concurrent

type t<'key, 'value> = ConcurrentDictionary<'key, 'value>

val find : 'key -> t<'key, 'value> -> 'value option

val remove_exn : 'key -> t<'key, 'value> -> 'value

val add_exn : 'key -> 'value -> t<'key, 'value> -> unit

val update : 'key -> ('value option -> 'value) -> t<'key, 'value> -> 'value
