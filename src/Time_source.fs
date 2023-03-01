module Core_kernel.Time_source

open System.Threading

type t =
  abstract member now : unit -> System.DateTime
  abstract member sleep_for : System.TimeSpan -> unit

type time_source = t

module Wall_clock =
  type t () =
    interface time_source with
      member this.now() = System.DateTime.Now
      member this.sleep_for span = System.Threading.Thread.Sleep span


module Controllable =
  type t (start_time : System.DateTime) =
    let now = Sequencer.create (ref start_time)
    let sleep_signal = new AutoResetEvent(false)

    let advance_to time (now : 'a ref) =
      if time < now.Value then
        failwithf
          "Time_source.Controllable.advance_to: Tried to advance to time in the past (time: %O) (now: %O)"
          time
          now
      else
        now.Value <- time
        System.Threading.Monitor.PulseAll now

    member this.advance_immediately_to time = Sequencer.with_ now (advance_to time)

    member this.advance_immediately_by span =
      Sequencer.with_ now (fun now -> advance_to (now.Value + span) now)

    member this.advance_after_sleep_to time =
      Auto_reset_event.wait_one sleep_signal
      this.advance_immediately_to time

    member this.advance_after_sleep_by span =
      Auto_reset_event.wait_one sleep_signal
      this.advance_immediately_by span

    interface time_source with
      member this.now() = Sequencer.with_ now (fun now -> now.Value)

      member this.sleep_for span =
        Sequencer.with_ now (fun now ->
          let sleep_until = now.Value + span

          Auto_reset_event.set sleep_signal

          while now.Value < sleep_until do
            while not (System.Threading.Monitor.Wait now) do
              ())

    interface System.IDisposable with
      member this.Dispose() = sleep_signal.Dispose()

module Constant =
  type t (time : System.DateTime) =
    let now = time

    interface time_source with
      member this.now() = now

      member this.sleep_for(_ : System.TimeSpan) =
        System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite)

  // This is fixed to MinValue instead of UnixEpoch because the latter is unavailable in
  // .NET Standard 2.0.
  let dotnet_epoch = new t (System.DateTime.MinValue)
