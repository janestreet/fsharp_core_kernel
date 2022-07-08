module Core_kernel.Dictionary

open System.Collections.Generic

type t<'key, 'value> = Dictionary<'key, 'value>

/// Raises if there is a duplicate key given
val of_alist_exn : ('key * 'value) list -> IEqualityComparer<'key> -> t<'key, 'value>

val find : t<'key, 'value> -> 'key -> 'value option

val set : t<'key, 'value> -> 'key -> 'value -> unit

val remove : t<'key, 'value> -> 'key -> unit

val iter : ('key -> 'value -> unit) -> t<'key, 'value> -> unit
