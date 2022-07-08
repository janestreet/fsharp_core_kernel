module Core_kernel.List

type 'a t = 'a list

val partition_map : f : ('a -> Either.t<'b, 'c>) -> 'a list -> 'b list * 'c list
val partition_tf : f : ('a -> bool) -> ('a list -> 'a list * 'a list)
val transpose : 'a list list -> 'a list list option
val filter_map : f : ('a -> 'b option) -> 'a list -> 'b list
val filter_opt : 'a option list -> 'a list
