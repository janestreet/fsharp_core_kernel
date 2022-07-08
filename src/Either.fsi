module Core_kernel.Either

type ('a, 'b) t =
  | First of 'a
  | Second of 'b
