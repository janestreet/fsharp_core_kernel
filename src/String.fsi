module Core_kernel.String

val lowercase : s : string -> string
val split : on : char -> s : string -> string list
val split_remove_empty : on : char -> s : string -> string list
