module Core_kernel.List

type 'a t = 'a list

let partition_map f ts =
  let firsts, seconds =
    List.fold
      (fun (firsts, seconds) t ->
        match f t with
        | Either.First t -> t :: firsts, seconds
        | Either.Second t -> firsts, t :: seconds)
      ([], [])
      ts

  List.rev firsts, List.rev seconds

let partition_tf f =
  partition_map (fun t ->
    if f t then
      Either.First t
    else
      Either.Second t)

// From [lib/base/src/list.ml]
let transpose t =
  let rec split_off_first_column t column_acc trimmed found_empty =
    match t with
    | [] -> column_acc, trimmed, found_empty
    | [] :: tl -> split_off_first_column tl column_acc trimmed true
    | (x :: xs) :: tl ->
      split_off_first_column tl (x :: column_acc) (xs :: trimmed) found_empty

  let split_off_first_column rows = split_off_first_column rows [] [] false in

  let rec loop rows columns do_rev =
    match split_off_first_column rows with
    | [], [], _ -> Some(List.rev columns)
    | column, trimmed_rows, found_empty ->
      if found_empty then
        None
      else
        (let column =
          if do_rev then
            List.rev column
          else
            column in

         loop trimmed_rows (column :: columns) (not do_rev))

  loop t [] true

let rev_filter_map f l =
  let rec loop l accum =
    match l with
    | [] -> accum
    | hd :: tl ->
      (match f hd with
       | Some x -> loop tl (x :: accum)
       | None -> loop tl accum)

  loop l []

let filter_map f l = List.rev (rev_filter_map f l)
let filter_opt t = filter_map id t
