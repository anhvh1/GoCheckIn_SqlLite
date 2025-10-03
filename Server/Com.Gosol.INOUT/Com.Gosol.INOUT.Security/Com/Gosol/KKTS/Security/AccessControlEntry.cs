namespace Com.Gosol.INOUT.Security
{
    using System;

    [Serializable]
    internal class AccessControlEntry
    {
        private int _accessRight;
        private ACLType _aclType;
        private int _entityid;
        private Com.Gosol.INOUT.Security.EntityType _entityType;

        public AccessControlEntry(Com.Gosol.INOUT.Security.EntityType entityType, int entityid, int accessRight) : this(entityType, entityid, accessRight, ACLType.ObjectInstance)
        {
        }

        public AccessControlEntry(Com.Gosol.INOUT.Security.EntityType entityType, int entityid, int accessRight, ACLType aclType)
        {
            this._entityType = entityType;
            if (aclType == ACLType.ObjectClass)
            {
                this._entityid = -1;
            }
            else if (aclType == ACLType.ObjectInstance)
            {
                this._entityid = entityid;
            }
            this._aclType = aclType;
            this._accessRight = accessRight;
        }

        public int AccessRight
        {
            get
            {
                return this._accessRight;
            }
        }

        public ACLType ACEType
        {
            get
            {
                return this._aclType;
            }
        }

        public int Entityid
        {
            get
            {
                return this._entityid;
            }
        }

        public Com.Gosol.INOUT.Security.EntityType EntityType
        {
            get
            {
                return this._entityType;
            }
        }
    }
}
