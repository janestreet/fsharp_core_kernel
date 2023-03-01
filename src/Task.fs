module Core_kernel.Task

type t<'a> = System.Threading.Tasks.Task<'a>

[<AutoOpen>]
module Repeat_or_finished =
  type t<'state, 'finished> =
    | Repeat of 'state
    | Finished of 'finished

  let is_repeating =
    function
    | Repeat (_ : 'state) -> true
    | Finished (_ : 'finished) -> false

  let repeat_state_exn =
    function
    | Repeat state -> state
    | Finished finished_state ->
      failwithf
        "[repeat_state_exn] called on [Finished] state (finished_state : %A)"
        finished_state

let repeat_until_finished
  (initial_state : 'state)
  (f : 'state -> t<Repeat_or_finished.t<'state, 'finished>>)
  =
  task {
    let mutable state = Repeat initial_state

    while is_repeating state do
      let! repeat_state = f (repeat_state_exn state)
      state <- repeat_state

    return
      match state with
      | Repeat state -> failwithf "Should not have stopped looping (state : %A)" state
      | Finished result -> result
  }

module Or_error =
  type t<'a> = System.Threading.Tasks.Task<Or_error.t<'a>>

  [<Sealed>]
  type ResultBuilder () =
    member __.Bind(t : t<'a>, f : 'a -> t<'b>) : t<'b> =
      task {
        match! t with
        | Error e -> return Error e
        | Ok v -> return! f v
      }

    member __.Delay(f : unit -> t<'a>) = f

    member __.Return(x : 'a) : t<'a> = task { return Ok x }

    member __.ReturnFrom(x : t<'a>) = x

    member __.Run(f : unit -> t<'a>) = f ()

    member __.Zero() : t<unit> = task { return Ok() }

    member __.TryWith(f : unit -> t<'a>, handler : exn -> t<'a>) =
      task {
        try
          return! f ()
        with
        | e -> return! handler e
      }

  let let_syntax = ResultBuilder()

  let tag msg t =
    task {
      let! result = t
      return Result.mapError (Error.tag msg) result
    }

  let try_with (f : unit -> t<'a>) =
    let_syntax {
      try
        return! f ()
      with
      | exn -> return! task { return Or_error.Error.string $"{exn}" }
    }
