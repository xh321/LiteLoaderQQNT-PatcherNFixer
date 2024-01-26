using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LiteLoaderPatchNFixer
{
    public static class Utils
    {
        public static string ReadQQDir()
        {
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey( "SOFTWARE", true );
            var path1 = softwareKey.OpenSubKey( "WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ" );
            var path2 = softwareKey.OpenSubKey( "Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ" );
            if ( path1 != null )
            {
                return new FileInfo( path1.GetValue( "UninstallString" ).ToString() ).DirectoryName;
            }
            else
            {
                return path2 == null ? "" : new FileInfo( path2.GetValue( "UninstallString" ).ToString() ).DirectoryName;
            }
        }

        public static string ReadQQVersoin()
        {
            RegistryKey softwareKey = Registry.LocalMachine.OpenSubKey( "SOFTWARE", true );
            var path1 = softwareKey.OpenSubKey( "WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ" );
            var path2 = softwareKey.OpenSubKey( "Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ" );
            if ( path1 != null )
            {
                return path1.GetValue( "DisplayVersion" ).ToString();
            }
            else
            {
                return path2 == null ? "" : path2.GetValue( "DisplayVersion" ).ToString();
            }
        }

        public static void ReplaceBinaryInFile( string fileName,
                                                byte[] oldBytes1,
                                                byte[] newBytes1,
                                                byte[] oldBytes2,
                                                byte[] newBytes2 )
        {
            byte[] fileBytes = File.ReadAllBytes( fileName );

            int index1 = IndexOfBytes( fileBytes, oldBytes1 );
            int index2 = IndexOfBytes( fileBytes, oldBytes2 );

            if ( index1 < 0 || index2 < 0 )
            {
                // Not found
                MessageBox.Show( "修补失败！对应序列未找到，可能是QQ版本不兼容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            if ( index1 > index2 )
            {
                ( index1, index2 ) = ( index2, index1 );
            }

            byte[] newFileBytes =
                new byte[ fileBytes.Length + newBytes1.Length - oldBytes1.Length + newBytes2.Length - oldBytes2.Length ];

            Buffer.BlockCopy( fileBytes, 0, newFileBytes, 0, index1 );
            Buffer.BlockCopy( newBytes1, 0, newFileBytes, index1, newBytes1.Length );
            Buffer.BlockCopy( fileBytes,
                              index1 + oldBytes1.Length,
                              newFileBytes,
                              index1 + newBytes1.Length,
                              index2 - index1 - oldBytes1.Length );
            Buffer.BlockCopy( newBytes2, 0, newFileBytes, index2 + newBytes1.Length - oldBytes1.Length, newBytes2.Length );
            Buffer.BlockCopy( fileBytes,
                              index2 + oldBytes2.Length,
                              newFileBytes,
                              index2 + newBytes1.Length - oldBytes1.Length + newBytes2.Length,
                              fileBytes.Length - index2 - oldBytes2.Length );

            File.WriteAllBytes( fileName, newFileBytes );
        }


        public static void ReplaceBinaryInFile( string fileName,
                                                byte[] oldBytes1,
                                                byte[] newBytes1 )
        {
            byte[] oldFileBytes = File.ReadAllBytes( fileName );
            int index1 = IndexOfBytes( oldFileBytes, oldBytes1 );

            if ( index1 < 0 )
            {
                // Not found
                MessageBox.Show( "修补失败！对应序列未找到，可能是QQ已经被修补或者版本不兼容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            byte[] newFileBytes = new byte[ oldFileBytes.Length ];

            Buffer.BlockCopy( oldFileBytes, 0, newFileBytes, 0, index1 );
            Buffer.BlockCopy( newBytes1, 0, newFileBytes, index1, newBytes1.Length );
            Buffer.BlockCopy( oldFileBytes,
                              index1 + oldBytes1.Length + Math.Abs( newBytes1.Length - oldBytes1.Length ),
                              newFileBytes,
                              index1 + newBytes1.Length,
                              oldFileBytes.Length - index1 - oldBytes1.Length - Math.Abs( newBytes1.Length - oldBytes1.Length ) );


            File.WriteAllBytes( fileName, newFileBytes );
        }

        public static int IndexOfBytes( byte[] searchBuffer,
                                        byte[] bytesToFind )
        {
            for ( int i = 0; i < searchBuffer.Length - bytesToFind.Length; i++ )
            {
                bool success = true;

                for ( int j = 0; j < bytesToFind.Length; j++ )
                {
                    if ( searchBuffer[ i + j ] != bytesToFind[ j ] )
                    {
                        success = false;
                        break;
                    }
                }

                if ( success )
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
