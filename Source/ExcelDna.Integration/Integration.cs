/*
  Copyright (C) 2005-2009 Govert van Drimmelen

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.


  Govert van Drimmelen
  govert@icon.co.za
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ExcelDna.Integration
{
    // CAUTION: These functions are called _via reflection_ by
    // ExcelDna.Loader.XlLibrary to set up the link between the loader 
    // and the integration library.
    // Signatures, private/public etc. is fragile.

    internal delegate int TryExcelImplDelegate(int xlFunction, out object result, params object[] parameters);
    internal delegate void RegisterMethodsDelegate(List<MethodInfo> methods);
    internal delegate byte[] GetAssemblyBytesDelegate(string assemblyName);

    public static class Integration
    {
        private static TryExcelImplDelegate tryExcelImpl;
        internal static void SetTryExcelImpl(TryExcelImplDelegate d)
        {
            tryExcelImpl = d;
        }

        internal static XlCall.XlReturn TryExcelImpl(int xlFunction, out object result, params object[] parameters)
        {
            if (tryExcelImpl != null)
            {
                return (XlCall.XlReturn)tryExcelImpl(xlFunction, out result, parameters);
            }
            result = null;
            return XlCall.XlReturn.XlReturnFailed;
        }

        private static RegisterMethodsDelegate registerMethods;
        internal static void SetRegisterMethods(RegisterMethodsDelegate d)
        {
            registerMethods = d;
        }

        // This is the only 'externally' exposed member.
        public static void RegisterMethods(List<MethodInfo> methods)
        {
            registerMethods(methods);
        }

        private static GetAssemblyBytesDelegate getAssemblyBytesDelegate;
        internal static void SetGetAssemblyBytesDelegate(GetAssemblyBytesDelegate d)
        {
            getAssemblyBytesDelegate = d;
        }

        internal static byte[] GetAssemblyBytes(string assemblyName)
        {
            return getAssemblyBytesDelegate(assemblyName);
        }

        internal static void Initialize()
        {
			ExcelDnaUtil.Initialize();
            DnaLibrary.Initialize();
        }

        internal static void DeInitialize()
        {
            DnaLibrary.DeInitialize();
        }

        internal static void DnaLibraryAutoOpen()
        {
            DnaLibrary.CurrentLibrary.AutoOpen();
        }

        internal static void DnaLibraryAutoClose()
        {
            DnaLibrary.CurrentLibrary.AutoClose();
        }

        internal static string DnaLibraryGetName()
        {
            return DnaLibrary.CurrentLibrary.Name;
        }
    }

    [Obsolete("Use ExcelDna.Integration.Integration class")]
    public class XlLibrary
    {
        [Obsolete("Use ExcelDna.Integration.Integration.RegisterMethods method")]
        public static void RegisterMethods(List<MethodInfo> methods)
        {
            Integration.RegisterMethods(methods);
        }
    }
}