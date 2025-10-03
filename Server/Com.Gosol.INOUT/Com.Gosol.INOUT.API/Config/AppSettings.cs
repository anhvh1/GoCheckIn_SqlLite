using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.API.Config
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string ClientID { get; set; }
        public double NumberDateExpire { get; set; }
        public string AudienceSecret { get; set; }
        public string ApiUrl { get; set; }
        public string Domain { get; set; }

        // aiface api
        public string GroupID { get; set; }
        public string OfficeID { get; set; }
        public string username_api { get; set; }
        public string password_api { get; set; }

        public string Add_Office { get; set; }
        public string Add_FaceInfo { get; set; }
        public string Add_FaceFeaturce { get; set; }
        public string Login { get; set; }
        public string Update_Index_Org_Token { get; set; }
        public string Annotation_Emdb { get; set; }
        public string Remove_Feature { get; set; }

        public string Version { get; set; }

        #region v2
        public string ApiKey { get; set; }
        public string GetToken { get; set; }
        public string AddFaceInfo_v2 { get; set; }
        public string CheckFaceImage { get; set; }
        public string VerifyFace { get; set; }
        public string DeleteFaceInfo { get; set; }
        public string AddFeature_v2 { get; set; }
        public string DeleteFeature_v2 { get; set; }
        #endregion
    }
}
