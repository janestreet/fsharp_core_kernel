module Core_kernel.Integer

module ActivePatterns =
  val (|ParseInteger|_|) : string -> int option
