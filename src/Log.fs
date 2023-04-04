module Core_kernel.Log

open System
open System.Collections.Concurrent
open System.Text.RegularExpressions
open System.Threading.Tasks

let printfn format =
  // Example: 2022-03-02 13:03:14.765-05:00:
  printf "%s: " (DateTime.Now.ToString "yyyy-MM-dd HH:mm:ss.fffK")
  printfn format

let global_counter : ConcurrentDictionary<string * int, int> = ConcurrentDictionary()

[<Sealed>]
type Limited () =
  static member printfn_every_n(n : int, format, path : string, line : int) =
    let i =
      Concurrent_dictionary.update
        (path, line)
        (function
        | None -> 0
        | Some i -> i + 1)
        global_counter

    Printf.ksprintf
      (fun message ->
        if i % n = 0 then
          printfn "%s" message
        else
          ())
      format

module For_testing =
  let sanitize_timestamp string =
    let regex = new Regex("\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3}[+-]\d{2}:\d{2}")
    regex.Replace(string, "<TIMESTAMP>")

module Level =
  type t =
    | Fatal
    | Error
    | Warn
    | Info
    | Verbose
    | Debug

  let of_string (s : string) =
    if s = null then
      raise (ArgumentNullException())
    else
      (match s.ToLower() with
       | "fatal" -> Fatal
       | "error" -> Error
       | "warn" -> Warn
       | "info" -> Info
       | "verbose" -> Verbose
       | "debug" -> Debug
       | (_ : string) -> failwithf "Invalid log level: '%s'" s)

  let to_string =
    function
    | Fatal -> "FATAL"
    | Error -> "ERROR"
    | Warn -> "WARN"
    | Info -> "INFO"
    | Verbose -> "VERBOSE"
    | Debug -> "DEBUG"

module Retention =

  module Action =
    type t =
      | Archive
      | Delete

  module When_to_archive =
    type t =
      | Never
      | After of TimeSpan

    let never = Never
    let after timespan = After timespan

    let to_timespan =
      function
      | Never -> None
      | After timespan -> Some timespan

  module When_to_delete =
    type t =
      | Never
      | After of TimeSpan

    let never = Never
    let after timespan = After timespan

    let to_timespan =
      function
      | Never -> None
      | After timespan -> Some timespan

  type t =
    { when_to_archive : When_to_archive.t
      when_to_delete : When_to_delete.t }

  let create when_to_archive when_to_delete =
    match when_to_archive, when_to_delete with
    | When_to_archive.After archive_after, When_to_delete.After delete_after when
      archive_after > delete_after
      ->
      failwith
        "Archive retention period should not be longer than delete retention period"
    | (_ : When_to_archive.t), (_ : When_to_delete.t) -> ()

    { when_to_archive = when_to_archive
      when_to_delete = when_to_delete }

/// Writers are used to output log messages to various destinations.
module Writer =

  open System.IO

  /// Output log messages to a file.
  module File =

    open System.IO.Compression

    module Naming_scheme =
      type t =
        | Dated
        | Timestamped

    module Id =
      type t = T of Naming_scheme.t * DateTime

      let create naming_scheme date = T(naming_scheme, date)

      let date (T ((_ : Naming_scheme.t), date)) = date

      let parse_date (date : string) =
        match DateTime.TryParse(date) with
        | true, date -> Some date
        | false, (_ : DateTime) -> None

      let of_string naming_scheme (s : string) =
        Option.map
          (create naming_scheme)
          (match naming_scheme with
           | Naming_scheme.Dated -> parse_date s
           | Naming_scheme.Timestamped ->
             match s.Split([| '_' |]) with
             | [| date; time |] ->
               parse_date (sprintf "%s %s" date (time.Replace("-", ":")))
             | (_ : string array) -> None)

      let to_string (T (naming_scheme, date)) =
        match naming_scheme with
        | Naming_scheme.Dated -> date.ToString("yyyy-MM-dd")
        | Naming_scheme.Timestamped -> date.ToString("yyyy-MM-dd_HH-mm-ss.ffffff")

    module Extension =
      type t =
        | Log
        | Zip

      let to_string =
        function
        | Log -> "log"
        | Zip -> "zip"

    type t =
      { directory : string
        file_name : string
        naming_scheme : Naming_scheme.t
        retention : Retention.t option
        mutable file_date : DateTime
        mutable path : string
        mutable writer : Lazy<StreamWriter> } // Don't open stream until first write

    let build_path directory file_name id extension =
      let file_name =
        if String.IsNullOrEmpty(file_name) then
          String.concat "." [ Id.to_string id; Extension.to_string extension ]
        else
          String.concat "." [ file_name; Id.to_string id; Extension.to_string extension ]

      Path.Combine(directory, file_name)

    // Open files for appending so that we don't overwrite existing log files.
    // Flush after every write operation to minimize the risk of losing log messages.
    let create_writer path =
      lazy (new StreamWriter(path, append = true, AutoFlush = true))

    let create naming_scheme retention (path : string) =
      let directory =
        if Path.IsPathRooted(path) then
          Path.GetFullPath(Path.GetDirectoryName(path))
        else
          Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path))

      if not (Directory.Exists(directory)) then
        failwithf "The log file directory %s does not exist" directory

      let file_name = Path.GetFileName(path)

      let path =
        build_path
          directory
          file_name
          (Id.create naming_scheme DateTime.Now)
          Extension.Log

      let writer = create_writer path

      { directory = directory
        file_name = file_name
        naming_scheme = naming_scheme
        retention = retention
        file_date = DateTime.Today
        path = path
        writer = writer }

    let get_file_name_prefix_with_seperator t =
      if t.file_name.Length = 0 then
        ""
      else
        sprintf "%s%s" t.file_name "."

    let get_file_id t (file : FileInfo) =
      Path
        .GetFileNameWithoutExtension(file.Name)
        .Substring((get_file_name_prefix_with_seperator t).Length)
      |> Id.of_string t.naming_scheme

    let get_files_for_action t retention_action =
      t.retention
      |> Option.map (fun retention ->
        try
          let retention_period =
            match retention_action with
            | Retention.Action.Archive ->
              Retention.When_to_archive.to_timespan retention.when_to_archive
            | Retention.Action.Delete ->
              Retention.When_to_delete.to_timespan retention.when_to_delete

          match retention_period with
          | None -> []
          | Some retention_period ->
            let cutoff = DateTime.Today - retention_period
            let log_files = Directory.GetFiles(t.directory, "*.log")

            let zip_files =
              if retention_action = Retention.Action.Delete then
                Directory.GetFiles(t.directory, "*.zip")
              else
                Array.empty

            Seq.append log_files zip_files
            |> Seq.map FileInfo
            |> Seq.filter (fun file ->
              match get_file_id t file with
              | None -> false
              | Some id ->
                file.Name.StartsWith(get_file_name_prefix_with_seperator t)
                && (Id.date id) < cutoff)
            |> List.ofSeq
        with
        | (_ : exn) -> [])
      |> Option.defaultValue []

    let maybe_archive_files t =
      get_files_for_action t Retention.Action.Archive
      |> List.fold
           (fun files_to_zip_by_date file ->
             match get_file_id t file with
             | None -> files_to_zip_by_date
             | Some id ->
               let date = (Id.date id).Date

               match Map.tryFind date files_to_zip_by_date with
               | None -> Map.add date [ file ] files_to_zip_by_date
               | Some files_to_zip ->
                 Map.add date (file :: files_to_zip) files_to_zip_by_date)
           Map.empty
      |> Map.iter (fun date files ->
        try
          let path =
            build_path
              t.directory
              t.file_name
              (Id.create t.naming_scheme date)
              Extension.Zip

          use stream = new FileStream(path, FileMode.Create)

          use archive = new ZipArchive(stream, ZipArchiveMode.Create)

          files
          |> List.iter (fun file ->
            (archive.CreateEntryFromFile(file.FullName, file.Name) : ZipArchiveEntry)
            |> ignore

            file.Delete())
        with
        | (_ : exn) -> ())

    let maybe_delete_files t =
      get_files_for_action t Retention.Action.Delete
      |> List.iter (fun file ->
        try
          file.Delete()
        with
        | (_ : exn) -> ())

    // Files will be rotated daily.
    let should_rotate t = t.file_date < DateTime.Today

    let rotate t =
      let path =
        build_path
          t.directory
          t.file_name
          (Id.create t.naming_scheme DateTime.Now)
          Extension.Log

      let previous_writer = t.writer
      let writer = create_writer path
      t.file_date <- DateTime.Today
      t.path <- path
      t.writer <- writer

      if previous_writer.IsValueCreated then
        previous_writer.Value.Dispose()

      async {
        maybe_delete_files t
        maybe_archive_files t
      }
      |> Async.StartAsTask
      |> ignore

    let write (t : t) _level _event_id (message : string) =
      if should_rotate t then rotate t
      t.writer.Value.WriteLine(message)

    let flush t () = t.writer.Value.FlushAsync() |> Async.AwaitTask

    let dispose t () =
      if t.writer.IsValueCreated then
        t.writer.Value.Dispose()

  module Console =
    type t = TextWriter

    let stdout = Console.Out

    let stderr = Console.Error

    let write (t : t) _level _event_id (message : string) = t.WriteLine(message)

    let flush (t : t) () = t.FlushAsync() |> Async.AwaitTask

  type t =
    { write : Level.t -> int -> string -> unit
      flush : unit -> Async<unit>
      dispose : unit -> unit
      mutable is_disposed : bool }

  let create write flush dispose =
    { write = write
      flush = flush
      dispose = dispose
      is_disposed = false }

  let write t = t.write

  let flush t = t.flush ()

  let dispose t =
    if not t.is_disposed then
      (t.dispose ()
       t.is_disposed <- true)

  let stdout = create (Console.write Console.stdout) (Console.flush Console.stdout) id

  let stderr = create (Console.write Console.stderr) (Console.flush Console.stderr) id

  let file naming_scheme retention path =
    let file = File.create naming_scheme retention path
    create (File.write file) (File.flush file) (File.dispose file)

module On_error =
  type t =
    | Ignore
    | Call of (Exception -> unit)

  let try_with_async t (error_message : Lazy<string>) f =
    async {
      match t with
      | Ignore ->
        try
          do! f ()
        with
        | (_ : exn) -> ()
      | Call call ->
        try
          do! f ()
        with
        | ex -> call (Exception(error_message.Value, ex))
    }

  let try_with t (error_message : Lazy<string>) f =
    try_with_async t error_message (fun () -> async { f () })
    |> Async.RunSynchronously

module On_shutdown =
  type t = { flush : bool }

/// Destinations serve as a specification for creating writers. This allows us to create
/// and dispose all writers internally.
module Destination =
  type t =
    | Stderr
    | Stdout
    | File_dated of string * Retention.t option
    | File_timestamped of string * Retention.t option
    | Custom of
      {| write : (string -> unit)
         flush : (unit -> Async<unit>)
         dispose : (unit -> unit) |}

  let stderr = Stderr

  let stdout = Stdout

  let file_dated path = File_dated(path, None)

  let file_dated_with_retention path retention = File_dated(path, Some retention)

  let file_timestamped path = File_timestamped(path, None)

  let file_timestamped_with_retention path retention =
    File_timestamped(path, Some retention)

  let custom args = Custom args

  let to_writer =
    function
    | Stderr -> Writer.stderr
    | Stdout -> Writer.stdout
    | File_dated (path, retention) ->
      Writer.file Writer.File.Naming_scheme.Dated retention path
    | File_timestamped (path, retention) ->
      Writer.file Writer.File.Naming_scheme.Timestamped retention path
    | Custom args -> Writer.create (fun _ _ -> args.write) args.flush args.dispose

// Here is the high level message flow for writing log messages:
//
//   Message --> MailboxProcessor Queue --> BlockingCollection --> Output Destinations
//
// Here is the process that happens when a message is logged:
//   1. The message, message level, and event id, are wrapped in a [Message.Write] and
//      asynchronously added to the MailboxProcessor queue
//   2. An asynchronous loop receives the [Message.Write] from the MailboxProcessor queue
//      and adds a tuple with the current date/time, message, message level, and event id
//      to the BlockingCollection
//
//        *** Note ***
//        The date/time is not added in step 1 to prevent a race condition where messages
//        that were logged later are added to the queue before earlier log messages
//
//   3. An asynchronous task enumerates the items in the BlockingCollection, formats the
//      messages, and writes them to the current output destinations
//
// The logging infrastructure must be shutdown before an application terminates otherwise
// log messages could be lost. Shutdown is automatically performed when the shutdown
// function is called, a log instance is disposed, and upon a ProcessExit event
//
// Here is the process for shutting down the logging infrastructure:
//   1.  A [Message.Shutdown] is synchronously added to the MailboxProcessor queue using
//       a blocking call that waits for a reply from the MailboxProcessor receiving loop
//       once it has finished performing shutdown activities
//   2.  The receiving loop receives the [Message.Shutdown]
//   2a. The receiving loop signals the BlockingCollection to indicate that no more items
//       will be added
//   2b. The receiving loop waits on the BlockingCollection processing task to complete
//   3.  The BlockingCollection processing task completes after it has processed the last
//       item and written the message to the current output destinations
//   4.  The receiving loop disposes the current writers
//   5.  The receiving loop sends a reply indicating that shutdown is complete then the
//       loop terminates

// The MailboxProcessor queue processes messages of this type.
module Message =
  type t =
    | Write of Level.t * int * string
    | Flush of AsyncReplyChannel<unit>
    | Shutdown of AsyncReplyChannel<unit>

// Log.t
type t =
  { mutable level : Level.t
    mutable writers : Writer.t list
    mutable on_error : On_error.t
    mutable on_shutdown : On_shutdown.t
    mutable agent : MailboxProcessor<Message.t> option
    mutable is_disposed : bool }
  interface IDisposable with
    member this.Dispose() =
      if not this.is_disposed then
        this.agent.Value.PostAndReply(Message.Shutdown)
        this.is_disposed <- true

let flush_writers t =
  List.map Writer.flush t.writers
  |> Async.Parallel
  |> Async.Ignore

// Main loop responsible for writing log messages and performing shutdown.
let message_handler t (inbox : MailboxProcessor<Message.t>) =
  // Returns a BlockingCollection used to store log messages before they are written to
  // the current output destinations (it is based on a ConcurrentQueue by default which
  // preserves the order of the log messages)
  let create_messages () =
    new BlockingCollection<DateTime * Level.t * int * string>()

  // Starts a BlockingCollection processing loop.
  let create_message_writer
    (messages : BlockingCollection<DateTime * Level.t * int * string>)
    =
    async {
      // This method enumerates the BlockingCollection then blocks until more items
      // are added or it receives a signal indicating that no more items will be added.
      messages.GetConsumingEnumerable()
      |> Seq.iter (fun (datetime, level, event_id, message) ->
        // The method below of formatting the date/time and performing string
        // concatenation resulted in the least amount of memory allocations/usage based
        // on the VS profiling tools (specifically, DateTime.ToString("o") and sprintf
        // resulted in a lot of allocations).
        let message =
          String.concat
            " "
            [ datetime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")
              Level.to_string level
              message ]
        // Try to write the log message to the output destination and handle exceptions
        // according to the current error action.
        On_error.try_with
          t.on_error
          (lazy (sprintf "Failed to write log entry: %s" message))
          (fun () ->
            t.writers
            |> List.iter (fun writer -> Writer.write writer level event_id message)))
    }
    |> Async.StartAsTask

  // This is the MailboxProcessor receiving loop.
  let rec receiving_loop
    (messages : BlockingCollection<_>)
    (message_writer : Task<unit>)
    =
    async {
      // Receive the next log message or block until one is available.
      let! message = inbox.Receive()

      match message with
      | Message.Write (level, event_id, message) ->
        // Add current date/time and log message to the BlockingCollection.
        On_error.try_with
          t.on_error
          (lazy (sprintf "Failed to write log entry: %s" message))
          (fun () -> messages.Add((DateTime.Now, level, event_id, message)))

        return! receiving_loop messages message_writer
      | Message.Flush reply_channel ->
        do!
          On_error.try_with_async t.on_error (lazy ("Failed to flush log")) (fun () ->
            async {
              messages.CompleteAdding()
              message_writer.Wait()
              messages.Dispose()
              message_writer.Dispose()
              do! flush_writers t
            })

        reply_channel.Reply()
        let messages = create_messages ()
        return! receiving_loop messages (create_message_writer messages)
      | Message.Shutdown reply_channel ->
        do!
          On_error.try_with_async t.on_error (lazy ("Failed to shutdown log")) (fun () ->
            async {
              messages.CompleteAdding()
              message_writer.Wait()

              if t.on_shutdown.flush then
                do! flush_writers t

              List.iter Writer.dispose t.writers
            })
        // End the asynchronous receiving loop.
        return reply_channel.Reply()
    }

  let messages = create_messages ()
  receiving_loop messages (create_message_writer messages)

let should_write (t : t) (level : Level.t) =
  match t.level, level with
  | Level.Fatal, Level.Fatal -> true
  | Level.Fatal,
    (Level.Debug
    | Level.Verbose
    | Level.Info
    | Level.Warn
    | Level.Error) -> false
  | Level.Error,
    (Level.Error
    | Level.Fatal) -> true
  | Level.Error,
    (Level.Debug
    | Level.Verbose
    | Level.Info
    | Level.Warn) -> false
  | Level.Warn,
    (Level.Warn
    | Level.Error
    | Level.Fatal) -> true
  | Level.Warn,
    (Level.Debug
    | Level.Verbose
    | Level.Info) -> false
  | Level.Info,
    (Level.Info
    | Level.Warn
    | Level.Error
    | Level.Fatal) -> true
  | Level.Info,
    (Level.Debug
    | Level.Verbose) -> false
  | Level.Verbose,
    (Level.Verbose
    | Level.Info
    | Level.Warn
    | Level.Error
    | Level.Fatal) -> true
  | Level.Verbose, Level.Debug -> false
  | Level.Debug,
    (Level.Debug
    | Level.Verbose
    | Level.Info
    | Level.Warn
    | Level.Error
    | Level.Fatal) -> true

let write (t : t) level event_id format =
  Printf.ksprintf
    (fun message ->
      if
        should_write t level
        && not (List.isEmpty t.writers)
      then
        t.agent.Value.Post(Message.Write(level, defaultArg event_id 0, message))
      else
        ())
    format

let create level destinations on_error on_shutdown =
  let t =
    { level = level
      writers = List.map Destination.to_writer destinations
      on_error = on_error
      on_shutdown = on_shutdown
      agent = None
      is_disposed = false }

  t.agent <- Some(MailboxProcessor.Start(message_handler t))

  // Exceptions thrown in the MailboxProcessor receiving loop are ignored by default
  // however they cause the loop to terminate. In this state new messages can still be
  // added to the queue but they will never be removed. We can configure the
  // MailboxProcessor to raise all exceptions that occur in the receiving loop to
  // prevent this. This is just a safeguard as the receiving loop handles exceptions.
  t.agent.Value.Error.Add(raise)

  // These event handlers ensure that the logging infrastructure is shutdown if
  // shutdown or dispose have not been called.
  AppDomain.CurrentDomain.ProcessExit.AddHandler (fun (_ : obj) (_ : EventArgs) ->
    (t :> IDisposable).Dispose())

  t

let flush t = t.agent.Value.PostAndReply(Message.Flush)

let flush_async t = t.agent.Value.PostAndAsyncReply(Message.Flush)

let shutdown (t : t) = (t :> IDisposable).Dispose()

let shutdown_async (t : t) = async { shutdown t }

let set_level t level = t.level <- level

let set_destinations t destinations =
  let previous_writers = t.writers
  t.writers <- List.map Destination.to_writer destinations
  List.iter Writer.dispose previous_writers

let set_on_error t on_error = t.on_error <- on_error

let set_on_shutdown t on_shutdown = t.on_shutdown <- on_shutdown

let fatal t format = write t Level.Fatal None format
let error t format = write t Level.Error None format
let warn t format = write t Level.Warn None format
let info t format = write t Level.Info None format
let verbose t format = write t Level.Verbose None format
let debug t format = write t Level.Debug None format

let fatal_string t msg = fatal t "%s" msg
let error_string t msg = error t "%s" msg
let warn_string t msg = warn t "%s" msg
let info_string t msg = info t "%s" msg
let verbose_string t msg = verbose t "%s" msg
let debug_string t msg = debug t "%s" msg

let fatal_event t event_id format = write t Level.Fatal (Some event_id) format

let error_event t event_id format = write t Level.Error (Some event_id) format

let warn_event t event_id format = write t Level.Warn (Some event_id) format

let info_event t event_id format = write t Level.Info (Some event_id) format

let verbose_event t event_id format = write t Level.Verbose (Some event_id) format

let debug_event t event_id format = write t Level.Debug (Some event_id) format

let fatal_event_string t event_id msg = fatal_event t event_id "%s" msg
let error_event_string t event_id msg = error_event t event_id "%s" msg
let warn_event_string t event_id msg = warn_event t event_id "%s" msg
let info_event_string t event_id msg = info_event t event_id "%s" msg
let verbose_event_string t event_id msg = verbose_event t event_id "%s" msg
let debug_event_string t event_id msg = debug_event t event_id "%s" msg

module Global =
  let log =
    lazy (create Level.Info [ Destination.stderr ] On_error.Ignore { flush = false })

  let flush () = flush log.Value

  let flush_async () = flush_async log.Value

  let shutdown () = shutdown log.Value

  let shutdown_async () = shutdown_async log.Value

  let set_level level = set_level log.Value level

  let set_destinations destinations = set_destinations log.Value destinations

  let set_on_error on_error = set_on_error log.Value on_error

  let set_on_shutdown on_shutdown = set_on_shutdown log.Value on_shutdown

  let fatal format = fatal log.Value format
  let error format = error log.Value format
  let warn format = warn log.Value format
  let info format = info log.Value format
  let verbose format = verbose log.Value format
  let debug format = debug log.Value format

  let fatal_string msg = fatal "%s" msg
  let error_string msg = error "%s" msg
  let warn_string msg = warn "%s" msg
  let info_string msg = info "%s" msg
  let verbose_string msg = verbose "%s" msg
  let debug_string msg = debug "%s" msg

  let fatal_event event_id format = fatal_event log.Value event_id format
  let error_event event_id format = error_event log.Value event_id format
  let warn_event event_id format = warn_event log.Value event_id format
  let info_event event_id format = info_event log.Value event_id format
  let verbose_event event_id format = verbose_event log.Value event_id format
  let debug_event event_id format = debug_event log.Value event_id format

  let fatal_event_string event_id msg = fatal_event event_id "%s" msg
  let error_event_string event_id msg = error_event event_id "%s" msg
  let warn_event_string event_id msg = warn_event event_id "%s" msg
  let info_event_string event_id msg = info_event event_id "%s" msg
  let verbose_event_string event_id msg = verbose_event event_id "%s" msg
  let debug_event_string event_id msg = debug_event event_id "%s" msg

#if DEBUG
module Test =
  open Writer

  let file_dated_build_path directory file_name date =
    File.build_path
      directory
      file_name
      (File.Id.create File.Naming_scheme.Dated date)
      File.Extension.Log

  let file_timestamped_build_path directory file_name date =
    File.build_path
      directory
      file_name
      (File.Id.create File.Naming_scheme.Timestamped date)
      File.Extension.Log

  let file_dated_get_files_to_delete retention path =
    let file = File.create File.Naming_scheme.Dated retention path

    let files_to_delete = File.get_files_for_action file Retention.Action.Delete

    File.dispose file ()
    files_to_delete

  let file_timestamped_get_files_to_delete retention path =
    let file = File.create File.Naming_scheme.Timestamped retention path

    let files_to_delete = File.get_files_for_action file Retention.Action.Delete

    File.dispose file ()
    files_to_delete

  let file_dated_get_files_to_archive retention path =
    let file = File.create File.Naming_scheme.Dated retention path

    let files_to_delete = File.get_files_for_action file Retention.Action.Archive

    File.dispose file ()
    files_to_delete

  let file_timestamped_get_files_to_archive retention path =
    let file = File.create File.Naming_scheme.Timestamped retention path

    let files_to_delete = File.get_files_for_action file Retention.Action.Archive

    File.dispose file ()
    files_to_delete

  let should_write t level = should_write t level

  let file_dated_execute_retention_actions retention path =
    let file = File.create File.Naming_scheme.Dated retention path

    File.maybe_archive_files file
    File.maybe_delete_files file
    File.dispose file ()

  let file_timestamped_execute_retention_actions retention path =
    let file = File.create File.Naming_scheme.Timestamped retention path

    File.maybe_archive_files file
    File.maybe_delete_files file
    File.dispose file ()
#endif
