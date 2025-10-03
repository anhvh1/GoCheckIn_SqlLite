namespace Com.Gosol.INOUT.Security
{
    using System;

    [Serializable]
    public class idNameInfo
    {
        private int _id;
        private string _name;
        private idNameType _type;

        public idNameInfo(int id, string name, idNameType type)
        {
            this._id = id;
            this._name = name;
            this._type = type;
        }

        public int id
        {
            get
            {
                return this._id;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public idNameType Type
        {
            get
            {
                return this._type;
            }
        }
    }
}
