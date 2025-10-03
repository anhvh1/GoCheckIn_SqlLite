using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Gosol.INOUT.DAL.FileDinhKem;
using Com.Gosol.INOUT.DAL.InOut;

namespace Com.Gosol.INOUT.BUS.VaoRa
{
    public interface IVaoRaV4BUS
    {
        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID);
        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel);
        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath);
        public List<ThongTinVaoRaModel> Get_By_ThongTinKhachID(int ThongTinKhachID, Boolean LaCanBo, int LoaiCheckOut, int CoQuanID, string serverPath);
        public BaseResultModel InsertFilDinhKem(FileDinhKemModel FileDinhKemModel);
        public List<FileDinhKemModel> FileDinhKem_GetBy_ThongTinRaVaoID(int ThongTinRaVaoID);
        public List<ThongTinVaoRaModel> Get_For_CheckOut_By_AnhChanDung(int CoQuanID);
        public List<ThongTinVaoRaModel> ThongKe_KhachVaoRa(BasePagingParamsFilter p, ref int TotalRow, int? DonViSuDungID);
        public List<ThongTinVaoRaModel> GetListPageBySearch(BasePagingParams p, int? Type, ref int TotalRow, int CoQuanID, int NguoidungID, string serverPath);
        public TongHopTheoNgay ThongTinVaoRa_TongHopTheoNgay(int CoQuanID, int NguoidungID);
        public ThongTinVaoRaModel Get_By_ThongTinVaoRaID(int ThongTinVaoRaID, string serverPath);
        public DoiTuongGap DanhSachDoiTuongGap(int CoQuanID);
        public List<ThongTinVaoRaModel> TruyVetYTe_ToanTinh(BasePagingParamsFilter p, ref int TotalRow);
        public List<ThongTinVaoRaModel> TruyVetYTe_TrongDonVi(BasePagingParamsFilter p, ref int TotalRow);
        public int InsertThongTinKhach(ThongTinKhachModel ThongTinKhachModel, ref string Message);
        public ThongTinKhachModel GetByID(int? ThongTinKhachID);
        public BaseResultModel UpdateThongTinKhach(ThongTinKhachModel ThongTinKhachModel);
        public BaseResultModel UpdateFaceID(ThongTinKhachModel ThongTinKhachModel);
    }
    public class VaoRaV4BUS : IVaoRaV4BUS
    {
        private IVaoRaV4DAL _IVaoRaV4DAL;
        private IFileDinhKemDAL _IFileDinhKemDAL;
        public VaoRaV4BUS(IVaoRaV4DAL VaoRaV4DAL, IFileDinhKemDAL IFileDinhKemDAL)
        {
            _IVaoRaV4DAL = VaoRaV4DAL;
            _IFileDinhKemDAL = IFileDinhKemDAL;
        }

        public DoiTuongGap DanhSachDoiTuongGap(int CoQuanID)
        {
            return _IVaoRaV4DAL.DanhSachDoiTuongGap(CoQuanID);
        }

        public List<FileDinhKemModel> FileDinhKem_GetBy_ThongTinRaVaoID(int ThongTinRaVaoID)
        {
            return _IFileDinhKemDAL.GetBy_ThongTinRaVaoID(ThongTinRaVaoID);
        }

        public List<ThongTinVaoRaModel> GetListPageBySearch(BasePagingParams p, int? Type, ref int TotalRow, int CoQuanID, int NguoidungID, string serverPath)
        {
            return _IVaoRaV4DAL.GetListPageBySearch(p, Type, ref TotalRow, CoQuanID, NguoidungID, serverPath);
        }

        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath)
        {
            return _IVaoRaV4DAL.Get_By_MaThe(MaThe, LoaiCheckOut, CoQuanID, serverPath);
        }
        public List<ThongTinVaoRaModel> Get_By_ThongTinKhachID(int ThongTinKhachID, Boolean LaCanBo, int LoaiCheckOut, int CoQuanID, string serverPath)
        {
            return _IVaoRaV4DAL.Get_By_ThongTinKhachID(ThongTinKhachID, LaCanBo, LoaiCheckOut, CoQuanID, serverPath);
        }

        public ThongTinVaoRaModel Get_By_ThongTinVaoRaID(int ThongTinVaoRaID, string serverPath)
        {
            return _IVaoRaV4DAL.Get_By_ThongTinVaoRaID(ThongTinVaoRaID, serverPath);
        }

        public List<ThongTinVaoRaModel> Get_For_CheckOut_By_AnhChanDung(int CoQuanID)
        {
            return _IVaoRaV4DAL.Get_For_CheckOut_By_AnhChanDung(CoQuanID);
        }

        public BaseResultModel InsertFilDinhKem(FileDinhKemModel FileDinhKemModel)
        {
            return _IFileDinhKemDAL.Insert(FileDinhKemModel);
        }

        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel)
        {
            return _IVaoRaV4DAL.Ra(TTKhachModel);
        }

        public List<ThongTinVaoRaModel> ThongKe_KhachVaoRa(BasePagingParamsFilter p, ref int TotalRow, int? DonViSuDungID)
        {
            return _IVaoRaV4DAL.ThongKe_KhachVaoRa(p, ref TotalRow, DonViSuDungID);
        }

        public TongHopTheoNgay ThongTinVaoRa_TongHopTheoNgay(int CoQuanID, int NguoidungID)
        {
            return _IVaoRaV4DAL.ThongTinVaoRa_TongHopTheoNgay(CoQuanID, NguoidungID);
        }

        public List<ThongTinVaoRaModel> TruyVetYTe_ToanTinh(BasePagingParamsFilter p, ref int TotalRow)
        {
            return _IVaoRaV4DAL.TruyVetYTe_ToanTinh(p, ref TotalRow);
        }

        public List<ThongTinVaoRaModel> TruyVetYTe_TrongDonVi(BasePagingParamsFilter p, ref int TotalRow)
        {
            return _IVaoRaV4DAL.TruyVetYTe_TrongDonVi(p, ref TotalRow);
        }

        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID)
        {
            return _IVaoRaV4DAL.Vao(VaoRaModel, ref MaThe, ref SoCMND, CoQuanID);
        }
        public int InsertThongTinKhach(ThongTinKhachModel ThongTinKhachModel, ref string Message)
        {
            return _IVaoRaV4DAL.InsertThongTinKhach(ThongTinKhachModel, ref Message);
        }
        public ThongTinKhachModel GetByID(int? ThongTinKhachID)
        {
            return _IVaoRaV4DAL.GetByID(ThongTinKhachID);
        }
        public BaseResultModel UpdateThongTinKhach(ThongTinKhachModel ThongTinKhachModel)
        {
            return _IVaoRaV4DAL.UpdateThongTinKhach(ThongTinKhachModel);
        }
        public BaseResultModel UpdateFaceID(ThongTinKhachModel ThongTinKhachModel)
        {
            return _IVaoRaV4DAL.UpdateFaceID(ThongTinKhachModel);
        }
    }
}
