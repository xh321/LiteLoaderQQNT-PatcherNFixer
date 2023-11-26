using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using PeNet;

namespace LiteLoaderPatchNFixer
{
    internal class FunctionHelper
    {
        public static PeNet.Header.Pe.ImageSectionHeader FindFunction(PeFile pe, ulong function_rva)
        {
            var sections = pe.ImageSectionHeaders;

            if (sections == null)
            {
                return null;
            }

            foreach (var section in sections)
            {
                if (section.VirtualAddress <= function_rva &&
                    function_rva < section.VirtualAddress + section.VirtualSize)
                {
                    return section;
                }
            }

            return null;
        }

        public static byte[] GetFunctionSignature(PeFile pe, string containing_name, uint size)
        {
            var exports = pe.ExportedFunctions;
            var base_address = pe.ImageNtHeaders.OptionalHeader.ImageBase;

            foreach (var export in exports)
            {
                var name = export.Name ?? "";

                if (name.Contains(containing_name))
                {
                    var function_rva = export.Address;
                    var section = FindFunction(pe, function_rva);
                    var function_offset = function_rva - section.VirtualAddress;

                    if (section.SizeOfRawData - function_offset < size)
                    {
                        size = section.SizeOfRawData - function_offset;
                    }

                    return pe.RawFile.AsSpan(
                         section.PointerToRawData + function_offset,
                         size).ToArray();
                }
            }

            return null;
        }

        private static List<int> SearchSignature(Span<byte> data, Span<byte> signature)
        {
            var ret = new List<int>();

            if (data.Length < signature.Length)
            {
                return ret;
            }

            for (int i = 0; i < data.Length - signature.Length; i++)
            {
                var temp = data.Slice(i, signature.Length);

                if (temp.SequenceCompareTo(signature) == 0)
                {
                    ret.Add(i);
                }
            }

            return ret;
        }

        public static ulong? GetFunctionRva(PeFile pe, byte[] signature)
        {
            var data = pe.RawFile.ToArray();
            var offset_list = SearchSignature(data, signature);
            var sections = pe.ImageSectionHeaders;

            var offset = offset_list[0];

            if (sections == null)
            {
                return null;
            }

            foreach (var section in sections)
            {
                if (section.PointerToRawData <= offset &&
                    offset < section.PointerToRawData + section.SizeOfRawData)
                {
                    return (ulong)offset - section.PointerToRawData + section.VirtualAddress;
                }
            }

            return null;
        }
    }

    public static class Utils
    {
        public static string ReadQQDir()
        {
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            var path1 = softwareKey.OpenSubKey("WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            var path2 = softwareKey.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            if (path1 != null)
            {
                return new FileInfo(path1.GetValue("UninstallString").ToString()).DirectoryName;
            }
            else
            {
                return path2 == null ? "" : new FileInfo(path2.GetValue("UninstallString").ToString()).DirectoryName;
            }
        }

        public static string ReadQQVersoin()
        {
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            var path1 = softwareKey.OpenSubKey("WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            var path2 = softwareKey.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            if (path1 != null)
            {
                return path1.GetValue("DisplayVersion").ToString();
            }
            else
            {
                return path2 == null ? "" : path2.GetValue("DisplayVersion").ToString();
            }
        }

        public static void ReplaceBinaryInFile(string fileName,
                                                byte[] oldBytes1,
                                                byte[] newBytes1,
                                                byte[] oldBytes2,
                                                byte[] newBytes2)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);

            int index1 = IndexOfBytes(fileBytes, oldBytes1);
            int index2 = IndexOfBytes(fileBytes, oldBytes2);

            if (index1 < 0 || index2 < 0)
            {
                // Not found
                return;
            }

            if (index1 > index2)
            {
                (index1, index2) = (index2, index1);
            }

            byte[] newFileBytes =
                new byte[fileBytes.Length + newBytes1.Length - oldBytes1.Length + newBytes2.Length - oldBytes2.Length];

            Buffer.BlockCopy(fileBytes, 0, newFileBytes, 0, index1);
            Buffer.BlockCopy(newBytes1, 0, newFileBytes, index1, newBytes1.Length);
            Buffer.BlockCopy(fileBytes,
                              index1 + oldBytes1.Length,
                              newFileBytes,
                              index1 + newBytes1.Length,
                              index2 - index1 - oldBytes1.Length);
            Buffer.BlockCopy(newBytes2, 0, newFileBytes, index2 + newBytes1.Length - oldBytes1.Length, newBytes2.Length);
            Buffer.BlockCopy(fileBytes,
                              index2 + oldBytes2.Length,
                              newFileBytes,
                              index2 + newBytes1.Length - oldBytes1.Length + newBytes2.Length,
                              fileBytes.Length - index2 - oldBytes2.Length);

            File.WriteAllBytes(fileName, newFileBytes);
        }

        public static int IndexOfBytes(byte[] searchBuffer,
                                        byte[] bytesToFind)
        {
            for (int i = 0; i < searchBuffer.Length - bytesToFind.Length; i++)
            {
                bool success = true;

                for (int j = 0; j < bytesToFind.Length; j++)
                {
                    if (searchBuffer[i + j] != bytesToFind[j])
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}