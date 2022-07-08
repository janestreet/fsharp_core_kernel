module Core_kernel.Configuration_manager

open System.Configuration

let try_get (key : string) =
  match ConfigurationManager.AppSettings.[key] with
  | null -> None
  | value -> Some value

let get_exn (key : string) = (try_get key).Value
