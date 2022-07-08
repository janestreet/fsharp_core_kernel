module Core_kernel.Hostname

open System.Net

type t = T of string

let to_string (T string) = string
let of_string string = T string

let current () = Dns.GetHostName() |> T
