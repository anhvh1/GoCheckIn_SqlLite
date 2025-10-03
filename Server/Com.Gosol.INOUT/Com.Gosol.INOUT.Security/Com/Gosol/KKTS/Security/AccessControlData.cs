namespace Com.Gosol.INOUT.Security
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal sealed class AccessControlData
    {
        private static string PARM_GROUP_STATUS = "@GroupStatus";
        private static string PARM_LOCKED = "@Locked";
        private static string PARM_PASSWORD = "@MatKhau";
        private static string PARM_USER_ID = "@NguoiDungID";
        private static string PARM_USERNAME = "@TenNguoiDung";
        private static string SELECT_ACE_FROM_USER_ID = "Select ChucNangID, PhanQuyen.NhomNguoiDungID, Quyen from NguoiDung inner join NguoiDung_NhomNguoiDung on NguoiDung.NguoiDungID = NguoiDung_NhomNguoiDung.NguoiDungID inner join NhomNguoiDung on NguoiDung_NhomNguoiDung.NhomNguoiDungID = NhomNguoiDung.NhomNguoiDungID inner join PhanQuyen on NhomNguoiDung.NhomNguoiDungID = PhanQuyen.NhomNguoiDungID Group by PhanQuyen.ChucNangID, PhanQuyen.NhomNguoiDungID, Quyen, NguoiDung.NguoiDungID Having NguoiDung.NguoiDungID = @NguoiDungID";
        //private static string SELECT_ACE_FROM_USER_ID = "Select ObjectTypeID, ObjectID, AccessRight From [User] Inner Join UserGroup On [User].UserID = UserGroup.UserID Inner Join [Group] On UserGroup.GroupID = [Group].GroupID Inner Join ObjectACL On [Group].GroupID = EntityID Where ([Group].Status <> @GroupStatus) Group By ObjectID, ObjectTypeID, AccessRight, [User].UserID Having [User].UserID = @UserID";
        private static string SELECT_ACETYPE_FROM_USER_ID = "Select ChucNangID, Quyen from NguoiDung inner join NguoiDung_NhomNguoiDung on NguoiDung.NguoiDungID = NguoiDung_NhomNguoiDung.NguoiDungID inner join NhomNguoiDung on NguoiDung_NhomNguoiDung.NhomNguoiDungID = NhomNguoiDung.NhomNguoiDungID inner join PhanQuyen on NhomNguoiDung.NhomNguoiDungID = PhanQuyen.NhomNguoiDungID Group by PhanQuyen.ChucNangID, Quyen, NguoiDung.NguoiDungID Having NguoiDung.NguoiDungID = @NguoiDungID";
        //private static string SELECT_ACETYPE_FROM_USER_ID = "Select ObjectTypeID, AccessRight From [User] Inner Join UserGroup On [User].UserID = UserGroup.UserID Inner Join [Group] On UserGroup.GroupID = [Group].GroupID Inner Join ObjectTypeACL On [Group].GroupID = EntityID Where ([Group].Status <> @GroupStatus) Group By ObjectTypeID, AccessRight, [User].UserID Having [User].UserID = @UserID";
        private static string SELECT_GROUPS_FROM_USER_ID = "Select NhomNguoiDung.NhomNguoiDungID, NhomNguoiDung.TenNhom from NguoiDung inner join NguoiDung_NhomNguoiDung on NguoiDung.NguoiDungID = NguoiDung_NhomNguoiDung.NguoiDungID inner join NhomNguoiDung on NguoiDung_NhomNguoiDung.NhomNguoiDungID = NhomNguoiDung.NhomNguoiDungID Group by NhomNguoiDung.NhomNguoiDungID, NhomNguoiDung.TenNhom, NguoiDung.NguoiDungID having NguoiDung.NguoiDungID = @NguoiDungID";
        //private static string SELECT_GROUPS_FROM_USER_ID = "Select [Group].GroupID, [Group].Name From [User] Inner Join UserGroup On [User].UserId = UserGroup.UserId Inner Join [Group] On [Group].GroupId = UserGroup.GroupId Where ([Group].Status <> @GroupStatus) And ([User].Locked <> @Locked) Group By [Group].GroupID, [Group].Name, [User].UserId Having [User].UserID=@UserID";
        private static string SELECT_USER = "Select NguoiDungID From [NguoiDung] Where (TenNguoiDung=@TenNguoiDung) And (MatKhau=@MatKhau) and TrangThai = 1";
        //private static string SELECT_USER_INFO = "Select a.*, b.TenCanBo, b.ChuoiNhaThuocID, b.KhoID, c.TenNhaThuoc, c.NhaThuocID, c.TinhID, c.HuyenID, c.XaID, c.MaNhaThuoc, d.TenTinh, b.RoleID, e.RoleName, h.TenKho From [NguoiDung] a join DanhMuc_CanBo b on a.CanBoID = b.CanBoID left join DanhMuc_NhaThuoc c on b.NhaThuocID = c.NhaThuocID left join DanhMuc_Tinh d on c.TinhID = d.TinhID left join Role e on b.RoleID = e.RoleID left join DanhMuc_Kho h on b.KhoID = h.KhoID Where (NguoiDungID= @NguoiDungID)";
        private static string SELECT_USER_INFO = "Select a.*, b.TenCanBo, b.ChuoiNhaThuocID, b.KhoID, c.TenNhaThuoc, c.NhaThuocID,c.ParentID, c.TinhID, c.HuyenID, c.XaID, c.MaNhaThuoc, d.TenTinh, b.RoleID, e.RoleName, h.TenKho,h.IsKhoTong, i.MaChuoiNhaThuoc, i.TenChuoiNhaThuoc, i.LoaiHinhThuc,i.WorkFlowID,j.WorkFlowCode From [NguoiDung] a join DanhMuc_CanBo b on a.CanBoID = b.CanBoID left join DanhMuc_NhaThuoc c on b.NhaThuocID = c.NhaThuocID left join DanhMuc_Tinh d on c.TinhID = d.TinhID left join Role e on b.RoleID = e.RoleID left join DanhMuc_Kho h on b.KhoID = h.KhoID left join DanhMuc_ChuoiNhaThuoc i on c.ChuoiNhaThuocID = i.ChuoiNhaThuocID left join WorkFlow j on i.WorkFlowID = j.WorkFlowID Where (NguoiDungID= @NguoiDungID)";

        private AccessControlData()
        {
            throw new AccessControlExceptions("CanNotCreateClass");
        }

        private static void HashACLKeyValues(Hashtable pairKeyValues, IDataReader dataReader, ACLType aclType)
        {
            string key = "";
            int num = 0;
            if (aclType == ACLType.ObjectInstance)
            {
                key = dataReader.GetInt32(0).ToString() + "$" + dataReader.GetInt32(1).ToString();
                num = dataReader.GetInt32(2);
            }
            else if (aclType == ACLType.ObjectClass)
            {
                key = dataReader.GetInt32(0).ToString();
                num = dataReader.GetInt32(1);
            }
            if (pairKeyValues.ContainsKey(key))
            {
                num |= (int) pairKeyValues[key];
                pairKeyValues[key] = num;
            }
            else
            {
                pairKeyValues.Add(key, num);
            }
        }

        private static void HashInfoKeyValues(Hashtable pairKeyValues, IDataReader dataReader)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                pairKeyValues.Add(dataReader.GetName(i), dataReader.GetValue(i));
            }
        }

        private static Hashtable HashKeyValues(IDbConnection dbConnection, IDbCommand dbCommand, ObjectInfoDelegate objectInfoDelegate, ACLType aclType)
        {
            Hashtable pairKeyValues = new Hashtable();
            try
            {
                dbConnection.Open();
                IDataReader dataReader = dbCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    if (objectInfoDelegate != null)
                    {
                        objectInfoDelegate(pairKeyValues, dataReader, aclType);
                    }
                    else if (aclType == ACLType.ObjectList)
                    {
                        pairKeyValues.Add(dataReader.GetInt32(0), dataReader.GetString(1));
                    }
                    else if (aclType == ACLType.ObjectInfo)
                    {
                        HashInfoKeyValues(pairKeyValues, dataReader);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new AccessControlExceptions("Error at HashKeyValues method in DataAccessManager class. Detail:" + exception.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            return pairKeyValues;
        }

        public static void RequestAccessRight(int userId, out Hashtable acl, out Hashtable aclType)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, SELECT_ACE_FROM_USER_ID);            
            //DatabaseProxy.AddParameter(dbCommand, PARM_USER_ID, userId);
            //acl = HashKeyValues(dbConnection, dbCommand, new ObjectInfoDelegate(AccessControlData.HashACLKeyValues), ACLType.ObjectInstance);
            acl = new Hashtable();
            dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, SELECT_ACETYPE_FROM_USER_ID);            
            DatabaseProxy.AddParameter(dbCommand, PARM_USER_ID, userId);
            aclType = HashKeyValues(dbConnection, dbCommand, new ObjectInfoDelegate(AccessControlData.HashACLKeyValues), ACLType.ObjectClass);
        }

        public static void RequestUserInfo(int userID, out Hashtable userInfo, out Hashtable accessGroup)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, SELECT_USER_INFO);
            DatabaseProxy.AddParameter(dbCommand, PARM_LOCKED, Convert.ToInt32(UserStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, PARM_USER_ID, userID);
            userInfo = HashKeyValues(dbConnection, dbCommand, null, ACLType.ObjectInfo);
            if (userInfo["UserName"] == null)
            {
                userInfo["UserName"] = string.Empty;
            }
            dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, SELECT_GROUPS_FROM_USER_ID);
            DatabaseProxy.AddParameter(dbCommand, PARM_GROUP_STATUS, Convert.ToInt32(GroupStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, PARM_LOCKED, Convert.ToInt32(UserStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, PARM_USER_ID, userID);
            accessGroup = HashKeyValues(dbConnection, dbCommand, null, ACLType.ObjectList);
        }

        public static bool VerifyUser(string userName, string password, out int userID)
        {
            object obj2;
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, SELECT_USER);
            DatabaseProxy.AddParameter(dbCommand, PARM_USERNAME, userName);
            DatabaseProxy.AddParameter(dbCommand, PARM_PASSWORD, password);
            DatabaseProxy.AddParameter(dbCommand, PARM_LOCKED, Convert.ToInt32(UserStatus.Locked));
            try
            {
                dbConnection.Open();
                obj2 = dbCommand.ExecuteScalar();
            }
            catch
            {
                throw new AccessControlExceptions("DatabaseException");
            }
            finally
            {
                dbConnection.Close();
            }
            if (obj2 == null)
            {
                userID = 0;
                return false;
            }
            userID = Convert.ToInt32(obj2);
            return true;
        }

        private delegate void ObjectInfoDelegate(Hashtable pairKeyValues, IDataReader dataReader, ACLType aclType);
    }
}
