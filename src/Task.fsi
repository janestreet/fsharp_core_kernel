module Core_kernel.Task

type t<'a> = System.Threading.Tasks.Task<'a>

module Repeat_or_finished =
  type t<'state, 'finished> =
    | Repeat of 'state
    | Finished of 'finished

val repeat_until_finished :
  initial_state : 'state ->
  f : ('state -> t<Repeat_or_finished.t<'state, 'finished>>) ->
    t<'finished>

module Or_error =
  type t<'a> = System.Threading.Tasks.Task<Or_error.t<'a>>

  [<Sealed>]
  type ResultBuilder =
    member Bind : t<'a> * ('a -> t<'c>) -> t<'c>
    member Delay : (unit -> t<'a>) -> (unit -> t<'a>)
    member Return : 'a -> t<'a>
    member ReturnFrom : t<'a> -> t<'a>
    member Run : (unit -> t<'a>) -> t<'a>
    member Zero : unit -> t<unit>
    member TryWith : (unit -> t<'a>) * (exn -> t<'a>) -> t<'a>

  val let_syntax : ResultBuilder

  val tag : string -> t<'a> -> t<'a>
  val try_with : (unit -> t<'a>) -> t<'a>
