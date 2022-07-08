module Core_kernel.Set

type t<'a when 'a : comparison> = 'a Set

let if_nonempty f t =
  if Set.isEmpty t then
    None
  else
    Some(f t)

let maxElement t = if_nonempty Set.maxElement t
let minElement t = if_nonempty Set.minElement t
