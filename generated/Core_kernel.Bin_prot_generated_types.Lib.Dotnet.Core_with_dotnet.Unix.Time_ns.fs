module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Unix.Time_ns
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module Stable = struct
  module Span = struct
    module V2 = struct
      type t = int64
      let bin_size_t = bin_size_int64
      let bin_write_t = bin_write_int64
      let bin_writer_t =
        {
          Bin_prot.Type_class.size = bin_size_t;
          Bin_prot.Type_class.write = bin_write_t
        }
      let __bin_read_t__ = __bin_read_int64__
      let bin_read_t = bin_read_int64
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
  module V1 = struct
    type t = int64
    let bin_size_t = bin_size_int64
    let bin_write_t = bin_write_int64
    let bin_writer_t =
      {
        Bin_prot.Type_class.size = bin_size_t;
        Bin_prot.Type_class.write = bin_write_t
      }
    let __bin_read_t__ = __bin_read_int64__
    let bin_read_t = bin_read_int64
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
