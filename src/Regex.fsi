module Core_kernel.Regex

module ActivePatterns =
  val (|ParseRegex|_|) : regex : string -> string : string -> string list option
