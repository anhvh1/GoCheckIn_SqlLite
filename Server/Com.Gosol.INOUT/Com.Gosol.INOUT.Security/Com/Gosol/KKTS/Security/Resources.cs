namespace Com.Gosol.INOUT.Security
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, CompilerGenerated, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        internal static string CanNotCreateClass
        {
            get
            {
                return ResourceManager.GetString("CanNotCreateClass", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string DatabaseException
        {
            get
            {
                return ResourceManager.GetString("DatabaseException", resourceCulture);
            }
        }

        internal static string NoCommandText
        {
            get
            {
                return ResourceManager.GetString("NoCommandText", resourceCulture);
            }
        }

        internal static string NoConnectionString
        {
            get
            {
                return ResourceManager.GetString("NoConnectionString", resourceCulture);
            }
        }

        internal static string NoParameterName
        {
            get
            {
                return ResourceManager.GetString("NoParameterName", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Com.Gosol.CMS.Security.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }
    }
}
