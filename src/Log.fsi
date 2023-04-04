module Core_kernel.Log

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

/// [printfn] which additionally prepends the message with [DateTime.Now]
val printfn : Printf.TextWriterFormat<'a> -> 'a

[<Sealed>]
type Limited =
  class
    static member printfn_every_n :
      n : int *
      format : Printf.StringFormat<'a, unit> *
      [<CallerFilePath; Optional; DefaultParameterValue("")>] path : string *
      [<CallerLineNumber; Optional; DefaultParameterValue(0)>] line : int ->
        'a
  end

module For_testing =
  val sanitize_timestamp : string -> string

/// A set of standard logging levels.
module Level =
  type t =
    | Fatal
    | Error
    | Warn
    | Info
    | Verbose
    | Debug

  val of_string : s : string -> t

module Retention =
  module When_to_archive =
    type t

    val never : t
    val after : timespan : TimeSpan -> t

  module When_to_delete =
    type t

    val never : t
    val after : timespan : TimeSpan -> t

  type t

  val create :
    when_to_archive : When_to_archive.t -> when_to_delete : When_to_delete.t -> t

/// Output destinations for log messages.
module Destination =
  type t

  /// Console standard error stream.
  val stderr : t

  /// Console standard output stream.
  val stdout : t

  /// Summary: File with a dated name that is rotated daily.
  ///
  /// path: Relative or absolute path to log directory or log file without extension.
  ///
  /// Remarks: Relative paths will use the application base directory as the root.
  /// All directories in the path must exist or an exception will be thrown.
  /// The file name component of the path will be used as a prefix for the log file name.
  /// The file name format is: [prefix].yyyy-mm-dd.log.
  val file_dated : path : string -> t

  /// Summary: File with a dated name that is rotated daily and files older than the provided retention period
  ///          are deleted on rotation.
  ///
  /// path: Relative or absolute path to log directory or log file without extension.
  ///
  /// retention: Specifies the amount of time to keep log files before archiving or deleting them on rotation.
  ///
  /// Remarks: Relative paths will use the application base directory as the root.
  /// All directories in the path must exist or an exception will be thrown.
  /// The file name component of the path will be used as a prefix for the log file name.
  /// The file name format is: [prefix].yyyy-mm-dd.log.
  val file_dated_with_retention : path : string -> retention : Retention.t -> t

  /// Summary: File with a timestamped name that is rotated daily.
  ///
  /// path: Relative or absolute path to log directory or log file without extension.
  ///
  /// Remarks: Relative paths will use the application base directory as the root.
  /// All directories in the path must exist or an exception will be thrown.
  /// The file name component of the path will be used as a prefix for the log file name.
  /// The file name format is: [prefix].yyyy-mm-dd_HH-mm-ss.ffffff.log.
  val file_timestamped : path : string -> t

  /// Summary: File with a timestamped name that is rotated daily and files older than the provided retention
  ///          period are deleted on rotation.
  ///
  /// path: Relative or absolute path to log directory or log file without extension.
  ///
  /// retention: Specifies the amount of time to keep log files before archiving or deleting them on rotation.
  ///
  /// Remarks: Relative paths will use the application base directory as the root.
  /// All directories in the path must exist or an exception will be thrown.
  /// The file name component of the path will be used as a prefix for the log file name.
  /// The file name format is: [prefix].yyyy-mm-dd_HH-mm-ss.ffffff.log.
  val file_timestamped_with_retention : path : string -> retention : Retention.t -> t

  /// Summary: Custom destination
  ///
  /// write: Function that writes log messages to a custom destination
  ///
  /// dispose: Function that disposes resources used to implement [write]
  ///
  /// Remarks: Intended for writing log messages to a custom destination without adding dependencies to Jane.Core.
  /// Attempting to call any logging functions in this module from [write] will cause infinite recursion.
  /// The [dispose] function will be called once, when [Log.t] is disposed or when [Log.shutdown] is called.
  val custom :
    {| write : (string -> unit)
       flush : (unit -> Async<unit>)
       dispose : (unit -> unit) |} ->
      t


/// Error action for exceptions thrown while writing log messages to output destinations.
module On_error =
  type t =
    /// Swallow exceptions.
    | Ignore
    /// Callback function to handle the exception.
    | Call of (Exception -> unit)

/// Options for controlling actions performed during shutdown
module On_shutdown =
  type t =
    { /// Flush writer for every output destination
      flush : bool }

[<Sealed>]
type t =
  interface IDisposable

/// Summary: Creates a new log instance.
///
/// level: Level at which log messages should be output.
///
/// destinations: Output destinations.
///
/// on_error: Error action for exceptions.
///
/// on_shutdown: Options for controlling shutdown behavior
///
/// Remarks: Log messages will be written to output destinations if the level.
/// is at or below the level specified by the writer or the log instance.
///
/// - Level.Error outputs error messages only
///
/// - Level.Info outputs error and info messages only
///
/// - Level.Debug outputs all messages
val create :
  level : Level.t ->
  destinations : Destination.t list ->
  on_error : On_error.t ->
  on_shutdown : On_shutdown.t ->
    t

/// Summary: Writes all pending log messages then flushes writer for every output destination
///
/// t: Log instance.
val flush : t -> unit

/// Summary: Asynchronously writes all pending log messages then flushes writer for every output destination
///
/// t: Log instance.
val flush_async : t -> Async<unit>

/// Summary: Shuts down the logging infrastructure
///
/// t: Log instance.
val shutdown : t -> unit

/// Summary: Asynchronously shuts down the logging infrastructure
///
/// t: Log instance.
val shutdown_async : t -> Async<unit>

/// Summary: Sets the current log level.
///
/// t: Log instance.
///
/// level: Level at which log messages should be output.
val set_level : t -> level : Level.t -> unit

/// Summary: Sets the current output destinations.
///
/// t: Log instance.
///
/// destinations: Output destinations.
val set_destinations : t -> destinations : Destination.t list -> unit

/// Summary: Sets the current error action.
///
/// t: Log instance.
///
/// on_error: Error action.
val set_on_error : t -> on_error : On_error.t -> unit

/// Summary: Sets the current options for controlling shutdown behavior
///
/// t: Log instance.
///
/// on_shutdown: Shutdown options.
val set_on_shutdown : t -> on_shutdown : On_shutdown.t -> unit

/// Sprintf style functions for generating log messages.
val fatal : t -> format : Printf.StringFormat<'a, unit> -> 'a
val error : t -> format : Printf.StringFormat<'a, unit> -> 'a
val warn : t -> format : Printf.StringFormat<'a, unit> -> 'a
val info : t -> format : Printf.StringFormat<'a, unit> -> 'a
val verbose : t -> format : Printf.StringFormat<'a, unit> -> 'a
val debug : t -> format : Printf.StringFormat<'a, unit> -> 'a

/// Functions for generating log messages from a string
val fatal_string : t -> msg : string -> unit
val error_string : t -> msg : string -> unit
val warn_string : t -> msg : string -> unit
val info_string : t -> msg : string -> unit
val verbose_string : t -> msg : string -> unit
val debug_string : t -> msg : string -> unit

/// Sprintf style functions for generating log messages with event id (used by the event log output destination).
val fatal_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
val error_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
val warn_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
val info_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
val verbose_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
val debug_event : t -> event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a

/// Functions for generating log messages from a string with event id (used by the event log output destination).
val fatal_event_string : t -> event_id : int -> msg : string -> unit
val error_event_string : t -> event_id : int -> msg : string -> unit
val warn_event_string : t -> event_id : int -> msg : string -> unit
val info_event_string : t -> event_id : int -> msg : string -> unit
val verbose_event_string : t -> event_id : int -> msg : string -> unit
val debug_event_string : t -> event_id : int -> msg : string -> unit

/// Summary: Singleton like global log instance.
///
/// Remarks: The following default settings are used.
///
/// - Log level defaults to Level.Info
///
/// - Output destinations defaults to Destination.Stderr
///
/// - Error action defaults to On_Error.Ignore
module Global =

  /// Summary: Writes all pending log messages then flushes writer for every output destination
  val flush : unit -> unit

  /// Summary: Asynchronously writes all pending log messages then flushes writer for every output destination
  val flush_async : unit -> Async<unit>

  /// Summary: Shuts down the logging infrastructure
  val shutdown : unit -> unit

  /// Summary: Asynchronously shuts down the logging infrastructure
  val shutdown_async : unit -> Async<unit>

  /// Summary: Sets the current log level.
  ///
  /// level: Level at which log messages should be output.
  val set_level : level : Level.t -> unit

  /// Summary: Sets the current output destinations.
  ///
  /// destinations: Output destinations.
  val set_destinations : destinations : Destination.t list -> unit

  /// Summary: Sets the current error action.
  ///
  /// on_error: Error action.
  val set_on_error : on_error : On_error.t -> unit

  /// Summary: Sets the current options for controlling shutdown behavior
  ///
  /// on_shutdown: Shutdown options.
  val set_on_shutdown : on_shutdown : On_shutdown.t -> unit

  /// Sprintf style functions for generating log messages.
  val fatal : format : Printf.StringFormat<'a, unit> -> 'a
  val error : format : Printf.StringFormat<'a, unit> -> 'a
  val warn : format : Printf.StringFormat<'a, unit> -> 'a
  val info : format : Printf.StringFormat<'a, unit> -> 'a
  val verbose : format : Printf.StringFormat<'a, unit> -> 'a
  val debug : format : Printf.StringFormat<'a, unit> -> 'a

  /// Functions for generating log messages from a string
  val fatal_string : msg : string -> unit
  val error_string : msg : string -> unit
  val warn_string : msg : string -> unit
  val info_string : msg : string -> unit
  val verbose_string : msg : string -> unit
  val debug_string : msg : string -> unit

  /// Sprintf style functions for generating log messages with event id (used by the event log output destination).
  val fatal_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
  val error_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
  val warn_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
  val info_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
  val verbose_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a
  val debug_event : event_id : int -> format : Printf.StringFormat<'a, unit> -> 'a

  /// Functions for generating log messages from a string with event id (used by the event log output destination).
  val fatal_event_string : event_id : int -> msg : string -> unit
  val error_event_string : event_id : int -> msg : string -> unit
  val warn_event_string : event_id : int -> msg : string -> unit
  val info_event_string : event_id : int -> msg : string -> unit
  val verbose_event_string : event_id : int -> msg : string -> unit
  val debug_event_string : event_id : int -> msg : string -> unit

#if DEBUG
module Test =
  open System.IO

  val file_dated_build_path :
    directory : string -> file_name : string -> date : DateTime -> string

  val file_timestamped_build_path :
    directory : string -> file_name : string -> date : DateTime -> string

  val file_dated_get_files_to_delete :
    retention : Retention.t option -> path : string -> FileInfo list

  val file_timestamped_get_files_to_delete :
    retention : Retention.t option -> path : string -> FileInfo list

  val file_dated_get_files_to_archive :
    retention : Retention.t option -> path : string -> FileInfo list

  val file_timestamped_get_files_to_archive :
    retention : Retention.t option -> path : string -> FileInfo list

  val file_dated_execute_retention_actions :
    retention : Retention.t option -> path : string -> unit

  val file_timestamped_execute_retention_actions :
    retention : Retention.t option -> path : string -> unit

  val should_write : t -> Level.t -> bool
#endif
