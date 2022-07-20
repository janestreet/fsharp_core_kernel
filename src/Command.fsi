module Core_kernel.Command

module Arg_type =
    type 'a t
    val create: (string -> 'a) -> 'a t

module Flag =
    type 'a t
    val required: 'a Arg_type.t -> string -> string -> 'a t
    val optional: 'a Arg_type.t -> string -> string -> 'a option t
    val no_arg: string -> string -> bool t

module Param =
    type 'a t
    val parse: 'a t -> string list -> 'a * string list
    val flag: 'a Flag.t -> 'a t
    val bind: 'a t -> ('a -> 'b t) -> 'b t
    val map: 'a t -> ('a -> 'b) -> 'b t
    val both: 'a t -> 'b t -> ('a * 'b) t
    val return_: 'a -> 'a t
    val zero: unit -> unit t

    [<Sealed>]
    type ResultBuilder =
        member Bind: 'a t * ('a -> 'b t) -> 'b t
        member MergeSources: 'a t * 'b t -> ('a * 'b) t
        member BindReturn: 'a t * ('a -> 'b) -> 'b t
        member Return: 'a -> 'a t
        member Zero: unit -> unit t

    val let_syntax: ResultBuilder

val run_exn: unit Param.t -> string list -> int
val run: unit Param.t -> string list -> int
