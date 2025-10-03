using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Com.Gosol.INOUT.Ultilities
{
    
  public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
    public static class GetType
    {
        public static Dictionary<string, string> GetTypes()
        {
            return new Dictionary<string, string>
            {
                    {".txt","text/plain" },
                    {".pdf","application/pdf" },
                    {".doc","application/vnd.ms-word" },
                    {".docx","application/vnd.ms-word" },
                    {".xls","application/vnd.ms-exel" },
                    {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                    {".png","image/png" },
                    {".jpg","image/jpeg" },
                    {".jpeg","image/jpeg" },
                    {".gif","image/gif" },
                    {".csv","image/csv" },
                    {".zip","application/zip" }
            };
        }

    }
}
