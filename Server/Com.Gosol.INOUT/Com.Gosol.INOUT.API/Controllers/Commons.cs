
using Com.Gosol.INOUT.BUS.VaoRa;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.API.Controllers
{
    public class Commons
    {
        /// <summary>
        /// save base64 to file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="TenFileHeThong"></param>
        /// <returns></returns>
        public bool SaveBase64ToFile(FileModel file, string TenFileHeThong, string pathSaveFile)
        {
            try
            {
                if (file.Base64.Length > 0)
                {
                    var b64 = file.Base64;
                    b64 = b64.Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(b64);
                    //var folderName = Path.Combine("Upload", "FileDinhKemDuyetKeKhai");
                    var folderName = Path.Combine(pathSaveFile);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var fullPath = Path.Combine(pathToSave, TenFileHeThong);
                    System.IO.File.WriteAllBytes(fullPath, bytes);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public string ConvertFileToBase64(string pathFile)
        {
            try
            {
                var at = System.IO.File.GetAttributes(pathFile);

                byte[] fileBit = System.IO.File.ReadAllBytes(pathFile);
                var file = System.IO.Path.Combine(pathFile);

                string AsBase64String = Convert.ToBase64String(fileBit);
                return AsBase64String;
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw ex;
            }
        }

        public string GetServerPath(HttpContext httpCT)
        {
            return httpCT.Request.Scheme + "://" + httpCT.Request.Host.Value;
        }
        public string? GenQRCode(string qrCodeText)
        {
            try
            {
                //QRCodeGenerator _qrCode = new QRCodeGenerator();
                //QRCodeData _qrCodeData = _qrCode.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);

                //QRCode qrCode = new QRCode(_qrCodeData);
                //Bitmap qrCodeImage = qrCode.GetGraphic(20);
                //return "data:image/png;base64," + Convert.ToBase64String(BitmapToBytesCode(qrCodeImage));


                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);

                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20); // 20 là pixel per module

                return "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public static Byte[] BitmapToBytesCode(Bitmap image)
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        return stream.ToArray();
        //    }
        //}

        public async Task<FileDinhKemModel> InsertFileAnhAsync(IFormFile source, FileDinhKemModel AnhNhanVien, IHostingEnvironment _host, IFileDinhKemBUS _FileDinhKemBUS)
        {
            try
            {
                FileDinhKemModel fileInfo = new FileDinhKemModel();
                string TenFileGoc = AnhNhanVien.TenFileGoc + ".jpg";
                fileInfo.TenFileHeThong = AnhNhanVien.NguoiTaoID + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + TenFileGoc;
                fileInfo.NguoiTaoID = AnhNhanVien.NguoiTaoID;
                fileInfo.LoaiFile = AnhNhanVien.LoaiFile;
                fileInfo.NgayTao = DateTime.Now;
                fileInfo.ThongTinVaoRaID = AnhNhanVien.ThongTinVaoRaID;
                fileInfo.LaCanBo = AnhNhanVien.LaCanBo;
                string folderPath = "AnhNhanDien";
                fileInfo.FileUrl = GetUrlFile(fileInfo.TenFileHeThong, folderPath);
          
                var ResultFile = _FileDinhKemBUS.InsertFileDinhKem(fileInfo);
                int FileID = Utils.ConvertToInt32(ResultFile.Data, 0);
                fileInfo.FileDinhKemID = FileID;
                if (ResultFile.Status > 0)
                {
                    //Add file vào thư mục server
                    try
                    {
                        if (AnhNhanVien.Base64File != null && AnhNhanVien.Base64File.Length > 0)
                        {
                            //byte[] bytes = Convert.FromBase64String(AnhNhanVien.Base64File);
                            //Image image;
                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    image = Image.FromStream(ms);
                            //    image.Save(_host.ContentRootPath + "\\" + fileInfo.FileUrl);
                            //}

                            byte[] bytes = Convert.FromBase64String(AnhNhanVien.Base64File);

                            // Tạo full path để lưu file
                            string filePath = Path.Combine(_host.ContentRootPath, fileInfo.FileUrl);

                            // Lưu trực tiếp ra file (không cần Image)
                            await File.WriteAllBytesAsync(filePath, bytes);

                            return fileInfo;
                        }

                        BinaryReader binaryFile = new BinaryReader(source.OpenReadStream());
                        byte[] byteArrFile = binaryFile.ReadBytes((int)source.OpenReadStream().Length);
                        CheckAndCreateFolder(_host, folderPath);
                        using (FileStream output = File.Create(GetSavePathFile(_host, fileInfo.TenFileHeThong, folderPath)))
                        {
                            output.Write(byteArrFile);
                        }

                        return fileInfo;
                    }
                    catch (Exception ex)
                    {
                        //int AnhID = Utils.ConvertToInt32(ResultFile.Data, 0);
                        //if (AnhID > 0)
                        //{
                        //    List<AnhNhanVienModel> listID = new List<AnhNhanVienModel>();
                        //    AnhNhanVienModel AnhNhanVienModel = new AnhNhanVienModel();
                        //    AnhNhanVienModel.AnhID = AnhID;
                        //    listID.Add(AnhNhanVienModel);
                        //    _CaiDatChamCongBUS.DeleteAnhNhanVien(listID);
                        //}

                        return new FileDinhKemModel();
                        throw ex;
                    }
                }
                else
                {
                    return new FileDinhKemModel();
                }
            }
            catch (Exception ex)
            {
                return new FileDinhKemModel();
                throw;
            }

        }

        public void CheckAndCreateFolder(IHostingEnvironment _host, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            string path = _host.ContentRootPath + "\\" + sysCF.GetByKey("SavePathFile").ConfigValue + "\\" + folderPath;
            bool isFolder = Directory.Exists(path);
            if (!isFolder)
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetSavePathFile(IHostingEnvironment _host, string filename, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            return _host.ContentRootPath + "\\" + sysCF.GetByKey("SavePathFile").ConfigValue + "\\" + folderPath + "\\" + filename;
        }

        public string GetUrlFile(string filename, string folderPath = "")
        {
            var sysCF = new SystemConfigDAL();
            var pathfile = sysCF.GetByKey("SavePathFile").ConfigValue;

            return pathfile + "\\" + folderPath + "\\" + filename;
        }
    }
    public class FileModel
    {
        public string TenFile { get; set; }
        public string Base64 { get; set; }
    }
}
