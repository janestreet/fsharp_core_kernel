module Core_kernel.String

open System

let lowercase (s : string) = s.ToLower()

let split (on : char) (s : string) = s.Split(Array.singleton on) |> List.ofArray

let split_remove_empty (on : char) (s : string) =
  s.Split(Array.singleton on, StringSplitOptions.RemoveEmptyEntries)
  |> List.ofArray
