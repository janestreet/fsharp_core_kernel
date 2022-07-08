module Core_kernel.User_groups

open System.Net
open System.Security.Principal
open System.Runtime.InteropServices
open System.Diagnostics

type t = string list

let windows_groups () =
  let remove_domain (s : string) =
    match s.IndexOf('\\') with
    | (-1) -> s
    | l -> s.Substring(l + 1)

  let id = WindowsIdentity.GetCurrent()

  Seq.choose
    (fun (g : IdentityReference) ->
      try
        (remove_domain (g.Translate(typeof<NTAccount>).ToString()))
          .ToLower()
        |> Some
      with
      | (_ : exn) ->
        None)
    id.Groups
  |> Seq.toList


let linux_groups () =
  Or_error.try_with
    (fun () ->
      let process_ = new Process()
      let start_info = new ProcessStartInfo("/usr/bin/groups")
      start_info.RedirectStandardOutput <- true
      process_.StartInfo <- start_info
      let (_ : bool) = process_.Start()
      let groups = process_.StandardOutput.ReadToEnd()
      process_.WaitForExit()
      groups.Split ' ' |> Array.toList)


let of_current_user () =
  if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
    Ok(windows_groups ())
  else if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) then
    linux_groups ()
  else
    Or_error.Error.string "Unexpected non-Windows, non-Linux OS"
