module Core_kernel.Result

type ('a, 'b) t = Result<'a, 'b>

let ok_exn =
  function
  | Ok x -> x
  | Error e -> failwithf "Result.ok_exn: %O" e

[<Sealed>]
type ResultBuilder () =
  member __.Bind(r, f) = Result.bind f r

  member __.Return(x) = Ok x

  member __.ReturnFrom(x) = x

  member __.Zero() = Ok()

  member __.TryFinally(x, f) =
    try
      __.ReturnFrom(x)
    finally
      f ()

  member __.Using(r, f) =
    __.TryFinally(f r, (fun () -> (r :> System.IDisposable).Dispose()))

let let_syntax = ResultBuilder()

let try_with f =
  try
    Ok(f ())
  with
  | exn -> Error exn

let iter t f =
  match t with
  | Ok v -> f v
  | Error (_ : 'a) -> ()

let iter_error t f =
  match t with
  | Ok (_ : 'a) -> ()
  | Error e -> f e

let join =
  function
  | Ok (Ok x) -> Ok x
  | Ok (Error e)
  | Error e -> Error e

let to_either =
  function
  | Ok x -> Either.First x
  | Error e -> Either.Second e

let combine_errors ts =
  let ok, errs = List.partition_map to_either ts in

  match errs with
  | [] -> Ok ok
  | _ :: _ -> Error errs

let all ts =
  let rec loop vs ts =
    let_syntax {
      match ts with
      | [] -> return List.rev vs
      | t :: ts ->
        let! v = t
        return! loop (v :: vs) ts
    }

  loop [] ts

let all_unit ts =
  all ts
  |> Result.map (ignore : (unit list -> unit))
