module Core_kernel.User_groups

type t = string list

val of_current_user : unit -> t Or_error.t
