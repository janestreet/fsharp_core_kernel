module Core_kernel.Map

type t<'key, 'value when 'key : comparison> = Map<'key, 'value>

let values t = t |> Map.toSeq |> Seq.map snd

let change key f t =
  Map.tryFind key t
  |> f
  |> function
    | Some value -> Map.add key value t
    | None -> Map.remove key t

let merge_skewed
  (combine : 'key -> 'value -> 'value -> 'value)
  (t1 : t<'key, 'value>)
  (t2 : t<'key, 'value>)
  =
  let t_large, t_small, combine_large_small =
    if Map.count t1 >= Map.count t2 then
      t1, t2, combine
    else
      t2, t1, (fun key v1 v2 -> combine key v2 v1)

  // Map.fold folder state table
  Map.fold
    (fun acc key small_data ->
      change
        key
        (function
        | Some large_data ->
          combine_large_small key large_data small_data
          |> Some
        | None -> Some small_data)
        acc)
    t_large
    t_small


let merge_skewed_left (t1 : t<'key, 'value>) (t2 : t<'key, 'value>) =
  merge_skewed (fun (_ : 'key) left_value (_ : 'value) -> left_value) t1 t2

let of_alist_multi alist =
  List.fold
    (fun map (k, v) ->
      Map.change
        k
        (function
        | None -> Some [ v ]
        | Some values -> Some(v :: values))
        map)
    Map.empty
    alist
  |> Map.map (fun (_ : 'a) values -> List.rev values)
