module Core_kernel.Map

type t<'key, 'value when 'key : comparison> = Map<'key, 'value>
val values : t<_, 'value> -> seq<'value>

val change :
  'key -> f : ('value option -> 'value option) -> t<'key, 'value> -> t<'key, 'value>

(** Merge two maps [t1], [t2] in O(min(|t1|, |t2|) * log(max(|t1|, |t2|))). [combine] will
be called if a key exists in both [t1] and [t2]. *)
val merge_skewed :
  combine : ('key -> 'value -> 'value -> 'value) ->
  t<'key, 'value> ->
  t<'key, 'value> ->
    t<'key, 'value>

(** [merge_skewed] preferring to keep the left map's values. *)
val merge_skewed_left : t<'key, 'value> -> t<'key, 'value> -> t<'key, 'value>
