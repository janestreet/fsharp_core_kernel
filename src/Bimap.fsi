module Core_kernel.Bimap

type t<'a, 'b when 'a : equality and 'b : equality>

val create : unit -> t<'a, 'b>
val try_find_a : t<'a, 'b> -> 'b -> option<'a>
val try_find_b : t<'a, 'b> -> 'a -> option<'b>
val add : t<'a, 'b> -> 'a -> 'b -> unit
val remove_by_a : t<'a, 'b> -> 'a -> unit
val remove_by_b : t<'a, 'b> -> 'b -> unit
