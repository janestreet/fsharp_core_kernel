namespace Core_kernel.Bin_prot_generated_types
module Result =
  type ('a, 'b) t = Result<'a, 'b>

  // The following is a fantomas-formatted copy of the bin_prot code that would be
  // generated if we redefined the 2-constructor variant [Ok of 'a | Error of 'b].
  let bin_size_t _size_of_a _size_of_b =
    function
    | Ok v1 -> let size = 1 in Bin_prot.Common.op_Addition size (_size_of_a v1)
    | Error v1 -> let size = 1 in Bin_prot.Common.op_Addition size (_size_of_b v1)

  let bin_write_t _write_a _write_b buf pos =
    function
    | Ok v1 ->
      let pos = Bin_prot.Write.bin_write_int_8bit buf pos 0 in _write_a buf pos v1
    | Error v1 ->
      let pos = Bin_prot.Write.bin_write_int_8bit buf pos 1 in _write_b buf pos v1

  let bin_writer_t bin_writer_a bin_writer_b =
    { Bin_prot.Type_class.size =
        (fun v ->
          bin_size_t
            (bin_writer_a : _ Bin_prot.Type_class.writer).size
            (bin_writer_b : _ Bin_prot.Type_class.writer).size
            v)
      Bin_prot.Type_class.write =
        (fun v ->
          bin_write_t
            (bin_writer_a : _ Bin_prot.Type_class.writer)
              .write
            (bin_writer_b : _ Bin_prot.Type_class.writer)
              .write
            v) }

  let __bin_read_t__ _of__a _of__b _buf pos_ref _vint =
    Bin_prot.Common.raise_variant_wrong_type "Result.fs.t" (!pos_ref)

  let bin_read_t _of__a _of__b buf pos_ref =
    match Bin_prot.Read.bin_read_int_8bit buf pos_ref with
    | 0 -> let arg_1 = _of__a buf pos_ref in Ok arg_1
    | 1 -> let arg_1 = _of__b buf pos_ref in Error arg_1
    | _ ->
      Bin_prot.Common.raise_read_error
        (Bin_prot.Common.ReadError.Sum_tag "Result.fs.t")
        (!pos_ref)

  let bin_reader_t bin_reader_a bin_reader_b =
    { Bin_prot.Type_class.read =
        (fun buf ->
          fun pos_ref ->
            (bin_read_t
              (bin_reader_a : _ Bin_prot.Type_class.reader).read
              (bin_reader_b : _ Bin_prot.Type_class.reader).read)
              buf
              pos_ref)
      Bin_prot.Type_class.vtag_read =
        (fun buf ->
          fun pos_ref ->
            fun vtag ->
              (__bin_read_t__
                (bin_reader_a : _ Bin_prot.Type_class.reader).read
                (bin_reader_b : _ Bin_prot.Type_class.reader).read)
                buf
                pos_ref
                vtag) }
