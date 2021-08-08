using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CardOrganizer
{
    public class WinAPI
    {
        public static bool Move(IList<Tuple<string, string>> files)
        {
            var sh = CreateBasicStruct();
            sh.wFunc = FO_Func.MOVE;
            sh.fFlags = FILEOP_FLAGS.MULTIDESTFILES | FILEOP_FLAGS.NOCONFIRMMKDIR;
            sh.pFrom = NullTerminate(files.Select(x => x.Item1));
            sh.pTo = NullTerminate(files.Select(x => x.Item2));
            return SHFileOperation(ref sh) == 0;
        }
        
        private static SHFILEOPSTRUCT CreateBasicStruct()
        {
            return new()
            {
                hwnd = IntPtr.Zero,
                fAnyOperationsAborted = false,
                hNameMappings = IntPtr.Zero
            };
        }

        private static string NullTerminate(IEnumerable<string> strings)
        {
            char nullChar = '\0';
            return $"{string.Join(nullChar.ToString(), strings)}{nullChar}{nullChar}";
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In, Out] ref SHFILEOPSTRUCT lpFileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public FILEOP_FLAGS fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        private enum FO_Func : uint
        {
            MOVE = 0x0001,
            COPY = 0x0002,
            DELETE = 0x0003,
            RENAME = 0x0004,
        }

        [Flags]
        private enum FILEOP_FLAGS : ushort
        {
            MULTIDESTFILES = 0x0001,
            CONFIRMMOUSE = 0x0002,
            SILENT = 0x0004,  // don't create progress/report
            RENAMEONCOLLISION = 0x0008,
            NOCONFIRMATION = 0x0010,  // Don't prompt the user.
            WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
                                         // Must be freed using SHFreeNameMappings
            ALLOWUNDO = 0x0040,
            FILESONLY = 0x0080,  // on *.*, do only files
            SIMPLEPROGRESS = 0x0100,  // means don't show names of files
            NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
            NOERRORUI = 0x0400,  // don't put up error UI
            NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
            NORECURSION = 0x1000,  // don't recurse into directories.
            NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
            WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
            NORECURSEREPARSE = 0x8000,  // treat reparse points as objects, not containers
        }
    }
}