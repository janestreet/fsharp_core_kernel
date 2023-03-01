module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module T =
  type t =
    | Could_not_construct of Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.t 
    | String of string 
    | Exn of Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.t 
    | Sexp of Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.t 
    | Tag_sexp of string * Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.t * option<Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position.T.t> 
    | Tag_t of string * t 
    | Tag_arg of string * Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.t * t 
    | Of_list of option<int64> * list<t> 
    | With_backtrace of t * string 
  let rec bin_size_t =
    function
    | Could_not_construct v1 ->
        let size = 1 in
        Bin_prot.Common.(+) size
          (Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_size_t
             v1)
    | String v1 ->
        let size = 1 in Bin_prot.Common.(+) size (bin_size_string v1)
    | Exn v1 ->
        let size = 1 in
        Bin_prot.Common.(+) size
          (Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_size_t
             v1)
    | Sexp v1 ->
        let size = 1 in
        Bin_prot.Common.(+) size
          (Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_size_t
             v1)
    | Tag_sexp (v1, v2, v3) ->
        let size = 1 in
        let size = Bin_prot.Common.(+) size (bin_size_string v1) in
        let size =
          Bin_prot.Common.(+) size
            (Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_size_t
               v2) in
        Bin_prot.Common.(+) size
          (bin_size_option
             Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position.T.bin_size_t
             v3)
    | Tag_t (v1, v2) ->
        let size = 1 in
        let size = Bin_prot.Common.(+) size (bin_size_string v1) in
        Bin_prot.Common.(+) size (bin_size_t v2)
    | Tag_arg (v1, v2, v3) ->
        let size = 1 in
        let size = Bin_prot.Common.(+) size (bin_size_string v1) in
        let size =
          Bin_prot.Common.(+) size
            (Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_size_t
               v2) in
        Bin_prot.Common.(+) size (bin_size_t v3)
    | Of_list (v1, v2) ->
        let size = 1 in
        let size = Bin_prot.Common.(+) size (bin_size_option bin_size_int64 v1) in
        Bin_prot.Common.(+) size (bin_size_list bin_size_t v2)
    | With_backtrace (v1, v2) ->
        let size = 1 in
        let size = Bin_prot.Common.(+) size (bin_size_t v1) in
        Bin_prot.Common.(+) size (bin_size_string v2)
  let rec bin_write_t buf pos =
    function
    | Could_not_construct v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 0 in
        Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_write_t
          buf pos v1
    | String v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 1 in
        bin_write_string buf pos v1
    | Exn v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 2 in
        Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_write_t
          buf pos v1
    | Sexp v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 3 in
        Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_write_t
          buf pos v1
    | Tag_sexp (v1, v2, v3) ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 4 in
        let pos = bin_write_string buf pos v1 in
        let pos =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_write_t
            buf pos v2 in
        bin_write_option
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position.T.bin_write_t
          buf pos v3
    | Tag_t (v1, v2) ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 5 in
        let pos = bin_write_string buf pos v1 in bin_write_t buf pos v2
    | Tag_arg (v1, v2, v3) ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 6 in
        let pos = bin_write_string buf pos v1 in
        let pos =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_write_t
            buf pos v2 in
        bin_write_t buf pos v3
    | Of_list (v1, v2) ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 7 in
        let pos = bin_write_option bin_write_int64 buf pos v1 in
        bin_write_list bin_write_t buf pos v2
    | With_backtrace (v1, v2) ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 8 in
        let pos = bin_write_t buf pos v1 in bin_write_string buf pos v2
  let bin_writer_t =
    {
      Bin_prot.Type_class.size = bin_size_t;
      Bin_prot.Type_class.write = bin_write_t
    }
  let rec __bin_read_t__ _buf pos_ref _vint =
    Bin_prot.Common.raise_variant_wrong_type
      "Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.fs.t"
      (!pos_ref)
  and bin_read_t buf pos_ref =
    match Bin_prot.Read.bin_read_int_8bit buf pos_ref with
    | 0 ->
        let arg_1 =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_read_t
            buf pos_ref in
        Could_not_construct arg_1
    | 1 -> let arg_1 = bin_read_string buf pos_ref in String arg_1
    | 2 ->
        let arg_1 =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_read_t
            buf pos_ref in
        Exn arg_1
    | 3 ->
        let arg_1 =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_read_t
            buf pos_ref in
        Sexp arg_1
    | 4 ->
        let arg_1 = bin_read_string buf pos_ref in
        let arg_2 =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_read_t
            buf pos_ref in
        let arg_3 =
          (bin_read_option
             Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position.T.bin_read_t)
            buf pos_ref in
        Tag_sexp (arg_1, arg_2, arg_3)
    | 5 ->
        let arg_1 = bin_read_string buf pos_ref in
        let arg_2 = bin_read_t buf pos_ref in Tag_t (arg_1, arg_2)
    | 6 ->
        let arg_1 = bin_read_string buf pos_ref in
        let arg_2 =
          Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.T.bin_read_t
            buf pos_ref in
        let arg_3 = bin_read_t buf pos_ref in Tag_arg (arg_1, arg_2, arg_3)
    | 7 ->
        let arg_1 = (bin_read_option bin_read_int64) buf pos_ref in
        let arg_2 = (bin_read_list bin_read_t) buf pos_ref in
        Of_list (arg_1, arg_2)
    | 8 ->
        let arg_1 = bin_read_t buf pos_ref in
        let arg_2 = bin_read_string buf pos_ref in
        With_backtrace (arg_1, arg_2)
    | _ ->
        Bin_prot.Common.raise_read_error
          (Bin_prot.Common.ReadError.Sum_tag
             "Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.fs.t")
          (!pos_ref)
  let bin_reader_t =
    {
      Bin_prot.Type_class.read = bin_read_t;
      Bin_prot.Type_class.vtag_read = __bin_read_t__
    }
  let rec bin_t =
    {
      Bin_prot.Type_class.writer = bin_writer_t;
      Bin_prot.Type_class.reader = bin_reader_t
    }
