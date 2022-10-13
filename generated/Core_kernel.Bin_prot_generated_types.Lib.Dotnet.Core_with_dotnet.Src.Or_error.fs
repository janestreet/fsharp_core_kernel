module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Or_error
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module T = struct
  type 'a t = ('a, Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.t) Core_kernel.Bin_prot_generated_types.Result.t
  let bin_size_t _size_of_a v =
    Core_kernel.Bin_prot_generated_types.Result.bin_size_t _size_of_a
      Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.bin_size_t
      v
  let bin_write_t _write_a buf pos v =
    Core_kernel.Bin_prot_generated_types.Result.bin_write_t _write_a
      Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.bin_write_t
      buf pos v
  let bin_writer_t bin_writer_a =
    {
      Bin_prot.Type_class.size =
        (fun v ->
           bin_size_t (bin_writer_a : _ Bin_prot.Type_class.writer).size v);
      Bin_prot.Type_class.write =
        (fun v ->
           bin_write_t (bin_writer_a : _ Bin_prot.Type_class.writer).write v)
    }
  let __bin_read_t__ _of__a buf pos_ref vint =
    (Core_kernel.Bin_prot_generated_types.Result.__bin_read_t__ _of__a
       Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.bin_read_t)
      buf pos_ref vint
  let bin_read_t _of__a buf pos_ref =
    (Core_kernel.Bin_prot_generated_types.Result.bin_read_t _of__a
       Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Error.T.bin_read_t)
      buf pos_ref
  let bin_reader_t bin_reader_a =
    {
      Bin_prot.Type_class.read =
        (fun buf ->
           fun pos_ref ->
             (bin_read_t (bin_reader_a : _ Bin_prot.Type_class.reader).read)
               buf pos_ref);
      Bin_prot.Type_class.vtag_read =
        (fun buf ->
           fun pos_ref ->
             fun vtag ->
               (__bin_read_t__
                  (bin_reader_a : _ Bin_prot.Type_class.reader).read) buf
                 pos_ref vtag)
    }
  let bin_t bin_a =
    {
      Bin_prot.Type_class.writer =
        (bin_writer_t (bin_a : _ Bin_prot.Type_class.t).writer);
      Bin_prot.Type_class.reader =
        (bin_reader_t (bin_a : _ Bin_prot.Type_class.t).reader)
    }
end
module Stable = struct
  module V2 = struct
    type 'a t = 'a T.t
    let bin_size_t _size_of_a v = T.bin_size_t _size_of_a v
    let bin_write_t _write_a buf pos v = T.bin_write_t _write_a buf pos v
    let bin_writer_t bin_writer_a =
      {
        Bin_prot.Type_class.size =
          (fun v ->
             bin_size_t (bin_writer_a : _ Bin_prot.Type_class.writer).size v);
        Bin_prot.Type_class.write =
          (fun v ->
             bin_write_t (bin_writer_a : _ Bin_prot.Type_class.writer).write v)
      }
    let __bin_read_t__ _of__a buf pos_ref vint =
      (T.__bin_read_t__ _of__a) buf pos_ref vint
    let bin_read_t _of__a buf pos_ref = (T.bin_read_t _of__a) buf pos_ref
    let bin_reader_t bin_reader_a =
      {
        Bin_prot.Type_class.read =
          (fun buf ->
             fun pos_ref ->
               (bin_read_t (bin_reader_a : _ Bin_prot.Type_class.reader).read)
                 buf pos_ref);
        Bin_prot.Type_class.vtag_read =
          (fun buf ->
             fun pos_ref ->
               fun vtag ->
                 (__bin_read_t__
                    (bin_reader_a : _ Bin_prot.Type_class.reader).read) buf
                   pos_ref vtag)
      }
    let bin_t bin_a =
      {
        Bin_prot.Type_class.writer =
          (bin_writer_t (bin_a : _ Bin_prot.Type_class.t).writer);
        Bin_prot.Type_class.reader =
          (bin_reader_t (bin_a : _ Bin_prot.Type_class.t).reader)
      }
  end
end
