using Com.Gosol.INOUT.DAL.FileDinhKem;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.InOut;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.VaoRa
{
    public interface IFileDinhKemBUS
    {
        public BaseResultModel InsertFileDinhKem(FileDinhKemModel FileDinhKemModel);
        public BaseResultModel UpdateFeatureID(FileDinhKemModel fileDinhKemModel);
    }
    public class FileDinhKemBUS : IFileDinhKemBUS
    {
        private IFileDinhKemDAL _IFileDinhKemDAL;
        public FileDinhKemBUS(IFileDinhKemDAL IFileDinhKemDAL)
        {
            _IFileDinhKemDAL = IFileDinhKemDAL;
        }
        public BaseResultModel InsertFileDinhKem(FileDinhKemModel FileDinhKemModel)
        {
            return _IFileDinhKemDAL.Insert_v4(FileDinhKemModel);
        }
        public BaseResultModel UpdateFeatureID(FileDinhKemModel fileDinhKemModel)
        {
            return _IFileDinhKemDAL.UpdateFeatureID(fileDinhKemModel);
        }
    }
}
