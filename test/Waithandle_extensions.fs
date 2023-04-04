module Core_kernel.Test.Waithandle_extensions

open NUnit.Framework
open Core_kernel
open Waithandle_extensions
open System.Threading

[<Test>]
let ``wait_for_signal only determines after signal received`` () =
  let cancellation_token_source = new CancellationTokenSource()

  let cancellation_token_cancelled =
    cancellation_token_source.Token.WaitHandle.wait_for_signal ()

  Assert.False(cancellation_token_cancelled.Wait(50))
  cancellation_token_source.Cancel()
  Assert.True(cancellation_token_cancelled.Wait(50))
