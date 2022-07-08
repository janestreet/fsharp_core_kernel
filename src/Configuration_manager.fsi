module Core_kernel.Configuration_manager

(* Light wrapper for accessing dotnet configuration settings:

https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager?view=dotnet-plat-ext-6.0
*)

val try_get : key : string -> string option

val get_exn : key : string -> string
