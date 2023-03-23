module Core_kernel.Username

type t = T of string

let current () = T(System.Environment.UserName.ToLower())
let to_string (T t) = t
