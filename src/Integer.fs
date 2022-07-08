module Core_kernel.Integer

module ActivePatterns =
  let (|ParseInteger|_|) (str : string) =
    let mutable intvalue = 0

    if System.Int32.TryParse(str, &intvalue) then
      Some(intvalue)
    else
      None
