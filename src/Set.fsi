module Core_kernel.Set

type t<'a when 'a : comparison> = 'a Set

val maxElement : 'a t -> 'a option
val minElement : 'a t -> 'a option
