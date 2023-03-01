module Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position
open Bin_prot.Write
open Bin_prot.Read
open Bin_prot.Size
module T =
  type t = {
    pos_fname: string ;
    pos_lnum: int64 ;
    pos_bol: int64 ;
    pos_cnum: int64 }
  let bin_size_t =
    function
    | { pos_fname = v1; pos_lnum = v2; pos_bol = v3; pos_cnum = v4 } ->
        let size = 0 in
        let size = Bin_prot.Common.(+) size (bin_size_string v1) in
        let size = Bin_prot.Common.(+) size (bin_size_int64 v2) in
        let size = Bin_prot.Common.(+) size (bin_size_int64 v3) in
        Bin_prot.Common.(+) size (bin_size_int64 v4)
  let bin_write_t buf pos =
    function
    | { pos_fname = v1; pos_lnum = v2; pos_bol = v3; pos_cnum = v4 } ->
        let pos = bin_write_string buf pos v1 in
        let pos = bin_write_int64 buf pos v2 in
        let pos = bin_write_int64 buf pos v3 in bin_write_int64 buf pos v4
  let bin_writer_t =
    {
      Bin_prot.Type_class.size = bin_size_t;
      Bin_prot.Type_class.write = bin_write_t
    }
  let __bin_read_t__ _buf pos_ref _vint =
    Bin_prot.Common.raise_variant_wrong_type
      "Core_kernel.Bin_prot_generated_types.Lib.Dotnet.Core_with_dotnet.Src.Source_code_position.fs.t"
      (!pos_ref)
  let bin_read_t buf pos_ref =
    let v_pos_fname = bin_read_string buf pos_ref in
    let v_pos_lnum = bin_read_int64 buf pos_ref in
    let v_pos_bol = bin_read_int64 buf pos_ref in
    let v_pos_cnum = bin_read_int64 buf pos_ref in
    {
      pos_fname = v_pos_fname;
      pos_lnum = v_pos_lnum;
      pos_bol = v_pos_bol;
      pos_cnum = v_pos_cnum
    }
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
