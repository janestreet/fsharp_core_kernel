module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module T = struct
  type t =
    | Atom of string 
    | List of t list 
  let rec bin_size_t =
    function
    | Atom v1 -> let size = 1 in Bin_prot.Common.(+) size (bin_size_string v1)
    | List v1 ->
        let size = 1 in Bin_prot.Common.(+) size (bin_size_list bin_size_t v1)
  let rec bin_write_t buf pos =
    function
    | Atom v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 0 in
        bin_write_string buf pos v1
    | List v1 ->
        let pos = Bin_prot.Write.bin_write_int_8bit buf pos 1 in
        bin_write_list bin_write_t buf pos v1
  let bin_writer_t =
    {
      Bin_prot.Type_class.size = bin_size_t;
      Bin_prot.Type_class.write = bin_write_t
    }
  let rec __bin_read_t__ _buf pos_ref _vint =
    Bin_prot.Common.raise_variant_wrong_type
      "Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.fs.t"
      (!pos_ref)
  and bin_read_t buf pos_ref =
    match Bin_prot.Read.bin_read_int_8bit buf pos_ref with
    | 0 -> let arg_1 = bin_read_string buf pos_ref in Atom arg_1
    | 1 -> let arg_1 = (bin_read_list bin_read_t) buf pos_ref in List arg_1
    | _ ->
        Bin_prot.Common.raise_read_error
          (Bin_prot.Common.ReadError.Sum_tag
             "Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Sexp.fs.t")
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
end
module Stable = struct
  module V1 = struct
    type t = T.t
    let bin_size_t = T.bin_size_t
    let bin_write_t = T.bin_write_t
    let bin_writer_t =
      {
        Bin_prot.Type_class.size = bin_size_t;
        Bin_prot.Type_class.write = bin_write_t
      }
    let __bin_read_t__ = T.__bin_read_t__
    let bin_read_t = T.bin_read_t
    let bin_reader_t =
      {
        Bin_prot.Type_class.read = bin_read_t;
        Bin_prot.Type_class.vtag_read = __bin_read_t__
      }
    let bin_t =
      {
        Bin_prot.Type_class.writer = bin_writer_t;
        Bin_prot.Type_class.reader = bin_reader_t
      }
  end
end
