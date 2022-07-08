module Core_kernel.Sequencer

/// Uses [lock] to control access to underlying data. Because [lock] can only operate on
/// reference types, [struct]s don't work here. (We could internally wrap ['a] in a
/// reference type, but that prevents callers from using [Monitor.wait] or [Monitor.pulse]
/// to signal on the enclosed value)
type 'a t

val create : t : 'a -> 'a t
val with_ : 'a t -> ('a -> 'b) -> 'b when 'a : not struct
