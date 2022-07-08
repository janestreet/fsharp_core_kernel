module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Percent
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module T = struct
  type t = float
  let bin_size_t = bin_size_float
  let bin_write_t = bin_write_float
  let bin_writer_t =
    {
      Bin_prot.Type_class.size = bin_size_t;
      Bin_prot.Type_class.write = bin_write_t
    }
  let __bin_read_t__ = __bin_read_float__
  let bin_read_t = bin_read_float
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
