module Core_kernel.Command

module Arg_type =
  type 'a t
  val create : (string -> 'a) -> 'a t

  val string : string t
  val float : float t
  val int : int t

module Flag =
  type 'a t
  val required : 'a Arg_type.t -> name : string -> doc : string -> 'a t
  val optional : 'a Arg_type.t -> name : string -> doc : string -> 'a option t
  val no_arg : name : string -> doc : string -> bool t

  val map : ('a -> 'b) -> 'a t -> 'b t

module Param =
  type 'a t
  val parse : 'a t -> string list -> 'a * string list
  val flag : 'a Flag.t -> 'a t

  [<Sealed>]
  type ResultBuilder =
    member MergeSources : 'a t * 'b t -> ('a * 'b) t
    member BindReturn : 'a t * ('a -> 'b) -> 'b t
    member Return : 'a -> 'a t
    member Zero : unit -> unit t

  val let_syntax : ResultBuilder

type t

val group : {| summary : string |} -> (string * t) list -> t
val basic : {| summary : string |} -> unit Param.t -> t

val run_exn : string list -> t -> unit
val run : string list -> t -> int
