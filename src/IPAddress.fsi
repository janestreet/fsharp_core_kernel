module Core_kernel.IPAddress

open System.Net
open System.Security.Principal

type t = IPAddress

/// If there are multiple ip addresses corresponding to the given hostname, the first is
/// returned.
val ipv4_of_hostname : Hostname.t -> t Or_error.t
