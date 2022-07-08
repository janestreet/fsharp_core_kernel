module Core_kernel.Result

type ('a, 'b) t = Result<'a, 'b>

val ok_exn : t<'a, 'b> -> 'a

[<Sealed>]
type ResultBuilder =
  member Bind : t<'a, 'b> * ('a -> t<'c, 'b>) -> t<'c, 'b>
  member Return : 'a -> t<'a, _>
  member ReturnFrom : 'a -> 'a
  member Zero : unit -> Result<unit, _>
  member Using : 'a * ('a -> 'b) -> 'b when 'a :> System.IDisposable

val let_syntax : ResultBuilder
val join : t<t<'a, 'b>, 'b> -> t<'a, 'b>
val all : t<'a, 'b> list -> t<'a list, 'b>
val combine_errors : t<'a, 'b> list -> t<'a list, 'b list>
val iter : t<'a, _> -> ('a -> unit) -> unit
