using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteLoaderPatchNFixer
{
    public partial class FrmMain : Form
    {
        private readonly string QQDir     = Utils.ReadQQDir();
        private readonly string QQVersion = Utils.ReadQQVersoin();
        public FrmMain() { InitializeComponent(); }

        private void FrmMain_Load( object    sender,
                                   EventArgs e )
        {
            lblQQDir.Text = "QQ路径：" + QQDir;
            lblQQVersion.Text = "QQ版本：" + QQVersion;
        }

        private void btnPatchQQ_Click( object    sender,
                                       EventArgs e )
        {
            lblStatus.Text = "状态：备份QQ.exe";
            if ( !File.Exists( QQDir + $"\\QQ.exe.{QQVersion}.bak" ) )
            {
                File.Copy( QQDir + "\\QQ.exe", QQDir + $"\\QQ.exe.{QQVersion}.bak" );
            }

            lblStatus.Text = "状态：修补QQ.exe中";
            Utils.ReplaceBinaryInFile( QQDir + "\\QQ.exe",
                                       new byte[] {0xAD, 0xA0, 0xB4, 0xAF, 0xA2, 0xA9, 0xA4, 0xB3, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                       new byte[] {0xA3, 0xA0, 0xB4, 0xAF, 0xA2, 0xA9, 0xA4, 0xB3, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                       new byte[] {0xB1, 0xA0, 0xA2, 0xAA, 0xA0, 0xA6, 0xA4, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                       new byte[] {0xA3, 0xA0, 0xA2, 0xAA, 0xA0, 0xA6, 0xA4, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00} );
            PatchJson();
            if ( MessageBox.Show( "修补完成！是否立即打开QQ？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
            {
                Process.Start( QQDir + "\\QQ.exe" );
            }
        }

        private void PatchJson()
        {
            lblStatus.Text = "状态：处理json文件中";
            //package处理

            //当前的package
            var realPackage = File.ReadAllText( QQDir + "\\resources\\app\\package.json" );

            if ( realPackage.Contains( "LiteLoader" ) )
            {
                //如果package不是原始的版本，先替换字符串，再写入
                File.WriteAllText( QQDir + "\\resources\\app\\backage.json",
                                   realPackage
                                       .Replace( "LiteLoader", "./app_launcher/index.js" )
                                       .Replace( "\r\n", "\n" ) );
            }
            else
            {
                //如果package是原始版本，直接复制一份即可
                //package→backage，现在backage是QQ的原始版本
                File.Copy( QQDir + "\\resources\\app\\package.json", QQDir + "\\resources\\app\\backage.json", true );
            }

            //package才是应用启动会读取到的，确保包含LiteLoader
            File.WriteAllText( QQDir + "\\resources\\app\\package.json",
                               realPackage
                                   .Replace( "./app_launcher/index.js", "LiteLoader" )
                                   .Replace( "\r\n", "\n" ) );

            //launcher处理
            File.Copy( QQDir + "\\resources\\app\\launcher.json", QQDir + "\\resources\\app\\bauncher.json", true );

            //替换bauncher的内容
            var realLauncher = File.ReadAllText( QQDir + "\\resources\\app\\bauncher.json" );
            File.WriteAllText( QQDir + "\\resources\\app\\bauncher.json",
                               realLauncher
                                   .Replace( "package", "backage" ) );

            lblStatus.Text = "状态：完毕";
        }

        private void btnFixQQCorrupt_Click( object    sender,
                                            EventArgs e )
        {
            PatchJson();
            if ( MessageBox.Show( "尝试修复完成！是否立即打开QQ？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
            {
                Process.Start( QQDir + "\\QQ.exe" );
            }
        }
    }
}
