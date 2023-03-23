module Core_kernel.Timing_wheel

open Core_kernel
open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

module Action =
  type t =
    { action : unit -> unit
      time : DateTime
      id : int }

  let id = ref 0

  let create action time =
    let id = Interlocked.Increment id

    { action = action
      time = time
      id = id }

  let run t = t.action ()

  let comparer =
    Comparer<t>.Create
      (fun x y ->
        let time_cmp = x.time.CompareTo(y.time)

        if time_cmp = 0 then
          x.id - y.id
        else
          time_cmp)

type t =
  { set : SortedSet<Action.t>
    mutable is_closed : bool }

let get_next_action_and_time_offset (t : t) now =
  if t.set.Count = 0 then
    None
  else
    let min = t.set.Min
    Some(min, (min.time - now))

let pop_ready_actions_or_get_wait_time t now =
  let rec loop ready_actions =
    match get_next_action_and_time_offset t now with
    | Some (next_action, wait_time) when wait_time <= TimeSpan.Zero ->
      let ready_actions = next_action :: ready_actions

      t.set.Remove(next_action)
      |> (ignore : bool -> unit)

      loop ready_actions
    | (Some (_ : (Action.t * TimeSpan))
    | None) as next_action_and_time_offset ->
      if List.length ready_actions > 0 then
        Either.First(List.rev ready_actions)
      else
        Either.Second(Option.map snd next_action_and_time_offset)

  loop []

let create () =
  let t =
    { set = new SortedSet<_>(Action.comparer)
      is_closed = false }

  Thread.spawn_and_ignore "timing wheel" (fun () ->
    lock t (fun () ->
      while not t.is_closed do
        match pop_ready_actions_or_get_wait_time t DateTime.Now with
        | Either.First ready_actions ->
          Thread.blocking_section t (fun () -> ready_actions |> List.iter Action.run)
        | Either.Second wait_time ->
          let (_ : bool) =
            match wait_time with
            | Some timeout -> Monitor.Wait(t, timeout)
            | None -> Monitor.Wait t

          ()))

  t

let schedule_action (t : t) (delay : TimeSpan) action =
  let action = Action.create action (DateTime.Now + delay)

  lock t (fun () ->
    t.set.Add action |> (ignore : bool -> unit)
    Monitor.Pulse t)

  action

let try_cancel (t : t) action =
  lock t (fun () -> t.set.Remove action |> (ignore : bool -> unit))

let close (t : t) =
  lock t (fun () ->
    t.is_closed <- true
    Monitor.Pulse t)
