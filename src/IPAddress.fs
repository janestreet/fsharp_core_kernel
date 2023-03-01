module Core_kernel.IPAddress

open System.Net
open System.Security.Principal

type t = IPAddress

let ipv4_of_hostname hostname =
  let addresses =
    Hostname.to_string hostname
    |> Dns.GetHostAddresses
    |> Array.filter (fun (address : IPAddress) ->
      (* https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.addressfamily?view=net-6.0 *)
      address.AddressFamily = Sockets.AddressFamily.InterNetwork)

  match Array.tryHead addresses with
  | Some address -> Ok address
  | None -> Or_error.Error.format "Failed to find any addresses for hostname %A" hostname
