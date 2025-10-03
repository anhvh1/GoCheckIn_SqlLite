namespace Com.Gosol.INOUT.Security
{
    using System;
    using System.Collections;
    using System.Data;

    [Serializable]
    public class SecurityProxy
    {
        private string DELETE_ACCESSRIGHT_ACL = "Delete ObjectACL Where ObjectTypeid = @ObjectTypeid And Objectid = @Objectid And Entityid = @Entityid";
        private string DELETE_ACCESSRIGHT_ACLTYPE = "Delete ObjectTypeACL Where ObjectTypeid = @ObjectTypeid And Entityid = @Entityid";
        private string DELETE_USER_GROUP = "Delete UserGroup Where Userid = @Userid And Groupid = @Groupid";
        private string INSERT_ACCESSRIGHT_ACL = "Insert Into ObjectACL (ObjectTypeid, Objectid, Entityid, EntityType, AccessRight) Values(@ObjectTypeid, @Objectid, @Entityid, 1, @AccessRight)";
        private string INSERT_ACCESSRIGHT_ACLTYPE = "Insert Into ObjectTypeACL (ObjectTypeid, Entityid, EntityType, AccessRight) Values(@ObjectTypeid, @Entityid, 1, @AccessRight)";
        private string INSERT_USER_GROUP = "Insert Into UserGroup (Userid, Groupid) Values(@Userid, @Groupid)";
        private string PARM_ACCESSRIGHT = "@AccessRight";
        private string PARM_CHANNEL_LOCKED = "@ChannelLocked";
        private string PARM_ENTITY_id = "@Entityid";
        private string PARM_GROUP_id = "@Groupid";
        private string PARM_GROUP_LOCKED = "@GroupLocked";
        private string PARM_OBJECT_id = "@Objectid";
        private string PARM_OBJECTTYPE_id = "@ObjectTypeid";
        private string PARM_USER_id = "@Userid";
        private string PARM_USER_LOCKED = "@UserLocked";
        private string SELECT_ACCESSRIGHT_ACL = "Select AccessRight From ObjectACL Where ObjectTypeid = @ObjectTypeid And Objectid = @Objectid And Entityid = @Entityid";
        private string SELECT_ACCESSRIGHT_ACLTYPE = "Select AccessRight From ObjectTypeACL Where ObjectTypeid = @ObjectTypeid And Entityid = @Entityid";
        private string SELECT_ACLCHANNEL_GROUPS = "Select [Group].Groupid, [Group].Name From [Group] Inner Join [ObjectACL] On [Group].Groupid = Entityid Where [Group].Status <> @GroupLocked And ObjectTypeid = @ObjectTypeid And Objectid = @Objectid";
        private string SELECT_ACLTYPE_GROUPS = "Select [Group].Groupid, [Group].Name From [Group] Inner Join [ObjectTypeACL] On [Group].Groupid = Entityid Where [Group].Status <> @GroupLocked And ObjectTypeid = @ObjectTypeid";
        private string SELECT_ALL_CHANNELS = "Select Channelid, [Channel].Name From Channel Where Status <> @ChannelLocked Order By [Channel].Name, [Channel].Priority";
        private string SELECT_ALL_GROUPS = "Select [Group].Groupid, [Group].Name From [Group] Where ([Group].Status <> @GroupLocked)";
        private string SELECT_ALL_OBJECTS = "Select [ObjectType].ObjectTypeid, [ObjectType].Name From [ObjectType]";
        private string SELECT_ALL_USERS = "Select [User].Userid, [User].UserName From [User] Where ([User].Locked <> @UserLocked)";
        private string SELECT_GROUPS_BY_USER_id = "Select [Group].Groupid, [Group].Name From [User] Inner Join UserGroup On [User].Userid = UserGroup.Userid Inner Join [Group] On [Group].Groupid = UserGroup.Groupid Where ([Group].Status <> @GroupLocked) And ([User].Locked <> @UserLocked) Group By [Group].Groupid, [Group].Name, [User].Userid Having [User].Userid=@Userid";
        private string SELECT_USERS_BY_GROUP_id = "Select [User].Userid, [User].UserName From [User] Inner Join UserGroup On [User].Userid = UserGroup.Userid Inner Join [Group] On [Group].Groupid = UserGroup.Groupid Where ([Group].Status <> @GroupLocked) And ([User].Locked <> @UserLocked) Group By [User].Userid, [User].UserName, [Group].Groupid Having [Group].Groupid=@Groupid";
        private string UPDATE_ACCESSRIGHT_ACL = "Update ObjectACL Set AccessRight = @AccessRight Where ObjectTypeid = @ObjectTypeid And Objectid = @Objectid And Entityid = @Entityid";
        private string UPDATE_ACCESSRIGHT_ACLTYPE = "Update ObjectTypeACL Set AccessRight = @AccessRight Where ObjectTypeid = @ObjectTypeid And Entityid = @Entityid";

        public bool DeleteACL(int objectTypeid, int objectid, int entityid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.DELETE_ACCESSRIGHT_ACL);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECT_id, objectid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool DeleteACLType(int objectTypeid, int entityid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.DELETE_ACCESSRIGHT_ACLTYPE);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool DeleteUserGroup(int userid, int groupid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.DELETE_USER_GROUP);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_id, userid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_id, groupid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        private bool ExecuteNonQuery(IDbConnection connection, IDbCommand command)
        {
            object obj2;
            try
            {
                connection.Open();
                obj2 = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DatabaseProxyException("Error at DeleteRecord method in ExecuteNonQuery class. Detail: " + exception.Message);
            }
            finally
            {
                connection.Close();
            }
            if (obj2 == null)
            {
                return false;
            }
            return true;
        }

        public AccessLevel GetAccessRight(int objectTypeid, int entityid)
        {
            object noAccess;
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ACCESSRIGHT_ACLTYPE);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            try
            {
                dbConnection.Open();
                noAccess = dbCommand.ExecuteScalar();
            }
            catch (Exception exception)
            {
                throw new DatabaseProxyException("Error at GetAccessRight method in SecurityProxy class. Detail:" + exception.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            if (noAccess == null)
            {
                noAccess = AccessLevel.NoAccess;
            }
            return (AccessLevel) noAccess;
        }

        public AccessLevel GetAccessRight(int objectTypeid, int objectid, int entityid)
        {
            object noAccess;
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ACCESSRIGHT_ACL);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECT_id, objectid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            try
            {
                dbConnection.Open();
                noAccess = dbCommand.ExecuteScalar();
            }
            catch (Exception exception)
            {
                throw new DatabaseProxyException("Error at GetAccessRight method in SecurityProxy class. Detail:" + exception.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            if (noAccess == null)
            {
                noAccess = AccessLevel.NoAccess;
            }
            return (AccessLevel) noAccess;
        }

        public IList GetACLChannelGroups(int objectTypeid, int objectid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ACLCHANNEL_GROUPS);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_LOCKED, Convert.ToInt32(GroupStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECT_id, objectid);
            return this.GetObjects(dbConnection, dbCommand, idNameType.ACLChannelGroup);
        }

        public IList GetACLTypeGroups(int objectTypeid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ACLTYPE_GROUPS);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_LOCKED, Convert.ToInt32(GroupStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            return this.GetObjects(dbConnection, dbCommand, idNameType.ACLTypeGroup);
        }

        public IList GetAllChannels()
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ALL_CHANNELS);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_CHANNEL_LOCKED, 0);
            return this.GetObjects(dbConnection, dbCommand, idNameType.Channel);
        }

        public IList GetAllGroups()
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ALL_GROUPS);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_LOCKED, Convert.ToInt32(GroupStatus.Locked));
            return this.GetObjects(dbConnection, dbCommand, idNameType.Group);
        }

        public IList GetAllObjects()
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand command = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ALL_OBJECTS);
            return this.GetObjects(dbConnection, command, idNameType.ObjectType);
        }

        public IList GetAllUsers()
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_ALL_USERS);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_LOCKED, Convert.ToInt32(UserStatus.Locked));
            return this.GetObjects(dbConnection, dbCommand, idNameType.User);
        }

        public IList GetGroups(int userid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_GROUPS_BY_USER_id);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_LOCKED, Convert.ToInt32(UserStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_LOCKED, Convert.ToInt32(GroupStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_id, userid);
            return this.GetObjects(dbConnection, dbCommand, idNameType.Group);
        }

        private IList GetObjects(IDbConnection connection, IDbCommand command, idNameType type)
        {
            IList list = new ArrayList();
            try
            {
                connection.Open();
                IDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    list.Add(new idNameInfo(id, name, type));
                }
            }
            catch (Exception exception)
            {
                throw new DatabaseProxyException("Error at GetObjects method in SecurityProxy class. Detail:" + exception.Message);
            }
            finally
            {
                connection.Close();
            }
            return list;
        }

        public IList GetUsers(int groupid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.SELECT_USERS_BY_GROUP_id);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_LOCKED, Convert.ToInt32(UserStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_LOCKED, Convert.ToInt32(GroupStatus.Locked));
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_id, groupid);
            return this.GetObjects(dbConnection, dbCommand, idNameType.User);
        }

        public bool InsertACL(int objectTypeid, int objectid, int entityid, int access)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.INSERT_ACCESSRIGHT_ACL);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ACCESSRIGHT, access);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECT_id, objectid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool InsertACLType(int objectTypeid, int entityid, int access)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.INSERT_ACCESSRIGHT_ACLTYPE);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ACCESSRIGHT, access);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool InsertUserGroup(int userid, int groupid)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.INSERT_USER_GROUP);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_USER_id, userid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_GROUP_id, groupid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool UpdateACL(int objectTypeid, int objectid, int entityid, int access)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.UPDATE_ACCESSRIGHT_ACL);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ACCESSRIGHT, access);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECT_id, objectid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }

        public bool UpdateACLType(int objectTypeid, int entityid, int access)
        {
            IDbConnection dbConnection = DatabaseProxy.CreateDBConnection();
            IDbCommand dbCommand = DatabaseProxy.CreateDBCommand(dbConnection, this.UPDATE_ACCESSRIGHT_ACLTYPE);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ACCESSRIGHT, access);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_ENTITY_id, entityid);
            DatabaseProxy.AddParameter(dbCommand, this.PARM_OBJECTTYPE_id, objectTypeid);
            return this.ExecuteNonQuery(dbConnection, dbCommand);
        }
    }
}
