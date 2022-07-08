module Core_kernel.Sequencer

type 'a t = T of 'a

let create t = T t
let with_ (T t) f = lock t (fun () -> f t)
