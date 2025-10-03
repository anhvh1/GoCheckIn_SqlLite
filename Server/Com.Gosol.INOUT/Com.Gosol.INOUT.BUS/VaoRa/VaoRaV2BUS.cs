using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.DAL.VaoRaV2;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Gosol.INOUT.DAL.FileDinhKem;

namespace Com.Gosol.INOUT.BUS.VaoRaV2
{
    public interface IVaoRaV2BUS
    {
        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID);
        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel);
        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath);
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

    }
    public class VaoRaV2BUS : IVaoRaV2BUS
    {
        private IVaoRaV2DAL _IVaoRaV2DAL;
        private IFileDinhKemDAL _IFileDinhKemDAL;
        public VaoRaV2BUS(IVaoRaV2DAL VaoRaV2DAL, IFileDinhKemDAL IFileDinhKemDAL)
        {
            _IVaoRaV2DAL = VaoRaV2DAL;
            _IFileDinhKemDAL = IFileDinhKemDAL;
        }

        public DoiTuongGap DanhSachDoiTuongGap(int CoQuanID)
        {
            return _IVaoRaV2DAL.DanhSachDoiTuongGap(CoQuanID);
        }

        public List<FileDinhKemModel> FileDinhKem_GetBy_ThongTinRaVaoID(int ThongTinRaVaoID)
        {
            return _IFileDinhKemDAL.GetBy_ThongTinRaVaoID(ThongTinRaVaoID);
        }

        public List<ThongTinVaoRaModel> GetListPageBySearch(BasePagingParams p, int? Type, ref int TotalRow, int CoQuanID, int NguoidungID, string serverPath)
        {
            return _IVaoRaV2DAL.GetListPageBySearch(p, Type, ref TotalRow, CoQuanID, NguoidungID, serverPath);
        }

        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath)
        {
            return _IVaoRaV2DAL.Get_By_MaThe(MaThe, LoaiCheckOut, CoQuanID, serverPath);
        }

        public ThongTinVaoRaModel Get_By_ThongTinVaoRaID(int ThongTinVaoRaID, string serverPath)
        {
            return _IVaoRaV2DAL.Get_By_ThongTinVaoRaID(ThongTinVaoRaID, serverPath);
        }

        public List<ThongTinVaoRaModel> Get_For_CheckOut_By_AnhChanDung(int CoQuanID)
        {
            return _IVaoRaV2DAL.Get_For_CheckOut_By_AnhChanDung(CoQuanID);
        }

        public BaseResultModel InsertFilDinhKem(FileDinhKemModel FileDinhKemModel)
        {
            return _IFileDinhKemDAL.Insert(FileDinhKemModel);
        }

        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel)
        {
            return _IVaoRaV2DAL.Ra(TTKhachModel);
        }

        public List<ThongTinVaoRaModel> ThongKe_KhachVaoRa(BasePagingParamsFilter p, ref int TotalRow, int? DonViSuDungID)
        {
            return _IVaoRaV2DAL.ThongKe_KhachVaoRa(p, ref TotalRow, DonViSuDungID);
        }

        public TongHopTheoNgay ThongTinVaoRa_TongHopTheoNgay(int CoQuanID, int NguoidungID)
        {
            return _IVaoRaV2DAL.ThongTinVaoRa_TongHopTheoNgay(CoQuanID, NguoidungID);
        }

        public List<ThongTinVaoRaModel> TruyVetYTe_ToanTinh(BasePagingParamsFilter p, ref int TotalRow)
        {
            return _IVaoRaV2DAL.TruyVetYTe_ToanTinh(p, ref TotalRow);
        }

        public List<ThongTinVaoRaModel> TruyVetYTe_TrongDonVi(BasePagingParamsFilter p, ref int TotalRow)
        {
            return _IVaoRaV2DAL.TruyVetYTe_TrongDonVi(p, ref TotalRow);
        }

        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID)
        {
            return _IVaoRaV2DAL.Vao(VaoRaModel, ref MaThe, ref SoCMND, CoQuanID);
        }


    }
}
