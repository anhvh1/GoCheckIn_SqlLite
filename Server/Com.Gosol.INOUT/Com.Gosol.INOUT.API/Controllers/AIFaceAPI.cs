using Com.Gosol.INOUT.API.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.API.Controllers
{
    public class AIFaceAPI
    {
        private IOptions<AppSettings> _AppSettings;
        public AIFaceAPI(IOptions<AppSettings> Settings)
        {
            _AppSettings = Settings;
        }
        public apiResult Add_Office(office item)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.Add_Office + "/" + (item.id != null ? item.id.Value.ToString() : _AppSettings.Value.OfficeID.ToString()));
                //var client = new RestClient("http://137.59.46.37/ai_api/office/add/12");

                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("username-api", _AppSettings.Value.username_api);
                request.AddHeader("password-api", _AppSettings.Value.password_api);
                request.AddParameter("application/json", "{\"name\": \"" + item.name + "\",\n    \"address\": \"" + item.address + "\",\n    \"city\": \"" + item.city + "\",\n    \"district\": \"" + item.district + "\",\n    \"ward\": \"" + item.ward + "\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Add_FaceInfo(faceinfo item)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.Add_FaceInfo);
                //var client = new RestClient("http://137.59.46.37/ai_api/faceinfo/add");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("username-api", _AppSettings.Value.username_api);
                request.AddHeader("password-api", _AppSettings.Value.password_api);
                //request.AddParameter("application/json", "{\n    \"full_name\": \""+ item.full_name + "\",\n    \"status\": \""+ item.status + "\", \n    \"organization_id\": "+ item.organization_id + ",\n    \"national_id\": " + "123411567899" + "\n    \"office_id\": " + item.office_id + "\n}", ParameterType.RequestBody);
                request.AddParameter("application/json", "{\n    \"full_name\": \"" + item.full_name + "\",\n    \"status\": \"" + item.status + "\",\n    \"national_id\": \"123411567899\",\n    \"organization_id\": " + item.organization_id + ",\n    \"office_id\": " + item.office_id + "\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Add_Feature(int faceid, string base64image, string xtokens)
        {
            try
            {
                //var client = new RestClient(_AppSettings.Value.Add_FaceFeaturce + faceid);    
                //client.Timeout = -1;
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("username-api", _AppSettings.Value.username_api);
                //request.AddHeader("password-api", _AppSettings.Value.password_api);
                //request.AddParameter("application/json", "{\n    \"is_cropped_face\" : \"false\",\n    \"is_captured_from_camera\" : \"false\",\n    \"embd\": \"\",\n    \"image\": \""+ base64image + "\"\n}", ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);

                var client = new RestClient(_AppSettings.Value.Add_FaceFeaturce + faceid);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("X-Tokens", xtokens);
                request.AddHeader("Content-Type", "text/plain");
                request.AddParameter("text/plain", "{\r\n    \"is_cropped_face\" : \"false\",\r\n    \"is_captured_from_camera\" : \"false\",\r\n    \"embd\": \"\",\r\n    \"image\": \"" + base64image + "\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Login()
        {
            try
            {
                string user_name = _AppSettings.Value.username_api;
                string password = _AppSettings.Value.password_api;
                var client = new RestClient(_AppSettings.Value.Login);
                //var client = new RestClient("http://137.59.46.37/frsip_api/camera/login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "text/html");
                request.AddParameter("text/html", "{\r\n    \"user_name\":\"" + user_name + "\",\r\n    \"password\":\"" + password + "\"\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Update_Index(int office_id, string xtokens)
        {
            try
            {
                var organization_id = _AppSettings.Value.OfficeID;
                var client = new RestClient(_AppSettings.Value.Update_Index_Org_Token + organization_id);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "text/html");
                request.AddHeader("X-Tokens", xtokens);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Annotation_Embd(int office_id, string base64image)
        {
            try
            {
                var organization_id = _AppSettings.Value.OfficeID;
                var client = new RestClient(_AppSettings.Value.Annotation_Emdb);
                //var client = new RestClient("http://137.59.46.37/ai_api/annotation_embd");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("username-api", _AppSettings.Value.username_api);
                request.AddHeader("password-api", _AppSettings.Value.password_api);
                request.AddParameter("application/json", "{\n    \"organization_id\": " + organization_id + ",\n    \"office_id\": " + office_id + ",\n    \"camera_id\": -1,\n    \"image\": \"" + base64image + "\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult Remove_Feature(int feature_id)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.Remove_Feature + feature_id);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("username-api", _AppSettings.Value.username_api);
                request.AddHeader("password-api", _AppSettings.Value.password_api);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult>(response.Content);
                }
                else
                {
                    return new apiResult();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region v2
        public apiResult_v2 GetToken()
        {
            try
            {

                //var client = new RestClient("https://aisol.vn/face/Token");
                var apikey = _AppSettings.Value.ApiKey;
                var client = new RestClient(_AppSettings.Value.GetToken);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("api-key", apikey);
                IRestResponse response = client.Execute(request);

                //var apikey = new RestClient(_AppSettings.Value.ApiKey);
                //var client = new RestClient(_AppSettings.Value.GetToken);
                //client.Timeout = -1;
                //var request = new RestRequest(Method.POST);
                //request.AddHeader("Content-Type", "application/json");
                //var body = @"{" + "\n" + @"    ""api-key"": """ + apikey + @"""" + "\n" + @"}";
                //request.AddParameter("application/json", body, ParameterType.RequestBody);
                //IRestResponse response = client.Execute(request);
                //Console.WriteLine(response.Content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult_v2 CheckFaceImage(string token, string base64image)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.CheckFaceImage);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("access-token", token);
                var body = @"{
                " + "\n" +
                @"    ""face_image"": """+ base64image + @"""
                " + "\n" +
                @"}";
                //var body = @"{""access-token"": """+ token + @""",""face_image"": """ + base64image + @"""}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
 
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult_v2 AddFace_v2(faceinfo item, string token)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.AddFaceInfo_v2);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("access-token", token);
                //request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("access-token", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJvcmdfaWQiOjI2LCJwYWNrYWdlX2FwcCI6MCwiZXhwIjoxNjM4MzYyODkwfQ.lVIuteWDCQ_3jLDUfp_CtB1t8JoBHPwccpRWAehvRis");
                var body = @"{
                " + "\n" +
                @"    ""full_name"": """ + item.full_name + @""",
                " + "\n" +
                @"    ""avatar"": """ + item.avatar + @""",
                " + "\n" +
                @"    ""gender"": ""3"",
                " + "\n" +
                @"    ""dob"": ""4"",
                " + "\n" +
                @"    ""occupattion"": ""5"",
                " + "\n" +
                @"    ""email"": ""6"",
                " + "\n" +
                @"    ""fax"": ""7"",
                " + "\n" +
                @"    ""national_id"": ""8"",
                " + "\n" +
                @"    ""address"": ""9"",
                " + "\n" +
                @"    ""classification"": ""10"",
                " + "\n" +
                @"    ""created_by"": ""11"",
                " + "\n" +
                @"    ""allow_sms"": ""12"",
                " + "\n" +
                @"    ""type"": ""13""
                " + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult_v2 AddFeature_v2(int faceid, string base64image, string token)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.AddFaceInfo_v2);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("access-token", token);
                var body = @"{
                " + "\n" +
                //@"    ""access-token"": """+ token + @""",
                //" + "\n" +
                @"    ""face_info_id"": """ + faceid + @""",
                " + "\n" +
                @"    ""face_image"": """+ base64image + @""",
                " + "\n" +
                //@"    ""is_avatar"": ""4"",
                //" + "\n" +
                //@"    ""created_by"": ""5"",
                //" + "\n" +
                //@"    ""modified_by"": ""6""
                //" + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult_v2 RemoveFeature_v2(int feature_id, string token)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.DeleteFeature_v2);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{
                " + "\n" +
                @"    ""access-token"": """ + token + @""",
                " + "\n" +
                @"    ""feature_id"": """ + feature_id + @"""
                " + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public apiResult_v2 DeleteFaceInfo(int face_info_id, string token)
        {
            try
            {
                var client = new RestClient(_AppSettings.Value.DeleteFaceInfo);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{
                " + "\n" +
                @"    ""access-token"": """ + token + @""",
                " + "\n" +
                @"    ""face_info_id"": """ + face_info_id + @"""
                " + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request); 

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<apiResult_v2>(response.Content);
                }
                else
                {
                    return new apiResult_v2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

    public class apiResult
    {
        public string status { get; set; }
        public string message { get; set; }
        public string org_id { get; set; }
        public int office_id { get; set; }
        public int id { get; set; }
        public string session_key { get; set; }
        public faceinfo user { get; set; }
        public List<predict_face> result { get; set; }
    }

    public class apiResult_v2
    {
        public string status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public int face_info_id { get; set; }
        public string full_name { get; set; }
        public string avatar { get; set; }

        public int id { get; set; }
        public faceinfo user { get; set; }
        public List<predict_face> result { get; set; }
    }
    public class predict_face
    {
        public string annotation { get; set; }
        public decimal similarity { get; set; }
    }
    public class liveFace
    {
        public string value { get; set; }
        public decimal live_score { get; set; }
    }
    public class qualityChecks
    {
        public string eyesClosed { get; set; }
        public string maskPresent { get; set; }
    }
    public class office
    {
        public string name { get; set; } = "";
        public string address { get; set; } = "";
        public string city { get; set; } = "";
        public string district { get; set; } = "";
        public string ward { get; set; } = "";
        public int? id { get; set; } = null;

    }
    public class faceinfo
    {
        public int? id { get; set; }
        public string full_name { get; set; }
        public int? organization_id { get; set; }
        public int? office_id { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string national_id { get; set; }
        public string phone { get; set; }
        public string type { get; set; }
        public string address { get; set; }
        public string status { get; set; }
        public string classification { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public string avatar { get; set; }
    }

    public class feature
    {
        public string name { get; set; } = "";
        public string address { get; set; } = "";
        public string city { get; set; } = "";
        public string district { get; set; } = "";
        public string ward { get; set; } = "";
        public int? id { get; set; } = null;

    }
}
