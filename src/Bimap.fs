module Core_kernel.Bimap

open System.Collections.Generic

type t<'a, 'b> =
  { a_to_b : Dictionary<'a, 'b>
    b_to_a : Dictionary<'b, 'a> }

let create () =
  { a_to_b = new Dictionary<_, _>(HashIdentity.Structural)
    b_to_a = new Dictionary<_, _>(HashIdentity.Structural) }

let try_find_a (t : t<'a, 'b>) b = Dictionary.find t.b_to_a b

let try_find_b (t : t<'a, 'b>) a = Dictionary.find t.a_to_b a

let add (t : t<'a, 'b>) a b =
  Dictionary.set t.a_to_b a b
  Dictionary.set t.b_to_a b a

let remove_by_a (t : t<'a, 'b>) a =
  match try_find_b t a with
  | None -> ()
  | Some b ->
    Dictionary.remove t.a_to_b a
    Dictionary.remove t.b_to_a b

let remove_by_b (t : t<'a, 'b>) b =
  match try_find_a t b with
  | None -> ()
  | Some a ->
    Dictionary.remove t.a_to_b a
    Dictionary.remove t.b_to_a b
