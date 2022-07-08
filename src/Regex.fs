module Core_kernel.Regex

open System.Text.RegularExpressions

module ActivePatterns =
  // From
  // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/active-patterns:
  //
  // ParseRegex parses a regular expression and returns a list of the strings that match
  // each group in the regular expression. List.tail is called to eliminate the first
  // element in the list, which is the full matched expression, since only the matches for
  // each group are wanted.
  let (|ParseRegex|_|) (regex : string) (string : string) =
    let m = Regex(regex).Match(string)

    if m.Success then
      Some(List.tail [ for x in m.Groups -> x.Value ])
    else
      None
