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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LiteLoaderPatchNFixer
{
    public partial class FrmMain : Form
    {
        private string QQDir       = Utils.ReadQQDir();
        private string QQVersion   = Utils.ReadQQVersoin();
        private bool   IsSupported = true;
        public FrmMain() { InitializeComponent(); }

        private void FrmMain_Load( object    sender,
                                   EventArgs e )
        {
            lblQQDir.Text = "QQ路径：" + QQDir;
            //目前暂无不支持版本
            if ( int.Parse( QQVersion.Split( '.' )[ 3 ] ) > 99999 )
            {
                lblQQVersion.Text = "QQ版本：" + QQVersion + "（不受支持）";
                lblQQVersion.ForeColor = Color.Red;
                IsSupported = false;
                btnPatchQQ.Enabled = false;
                btnFixQQCorrupt.Enabled = false;

                radOldVersion.Enabled = false;
                if ( MessageBox.Show( "你目前QQ版本已经高于9.9.9-99999，需要使用新版本修补器，是否打开项目主页？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
                {
                    Process.Start( "explorer.exe", "https://github.com/LiteLoaderQQNT/QQNTFileVerifyPatch" );
                }
            }
            else if ( int.Parse( QQVersion.Split( '.' )[ 3 ] ) > 16183 )
            {
                lblQQVersion.Text = "QQ版本：" + QQVersion + "（仅支持>=1.0版本框架）";
                lblQQVersion.ForeColor = Color.Black;
                IsSupported = true;
                btnPatchQQ.Enabled = true;
                btnFixQQCorrupt.Enabled = false;

                radOldVersion.Enabled = false;
                MessageBox.Show( "你目前QQ版本已经高于9.9.2-16183，仅支持安装>=1.0版本的框架，请注意。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                // 修补器已支持
                // MessageBox.Show( "注意！如果你想支持新版本LiteLoaderQQNT框架（版本号>1.0），请使用新版本修补器！本修补器修补后仅支持旧版本框架使用。建议用过本修补器的用户进入主界面，点恢复初始状态按钮，然后再去使用新版本修补器。", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
                lblQQVersion.Text = "QQ版本：" + QQVersion;
                IsSupported = true;
                btnPatchQQ.Enabled = true;
                btnFixQQCorrupt.Enabled = true;
                radOldVersion.Enabled = true;
            }
        }

        private void btnPatchQQ_Click( object    sender,
                                       EventArgs e )
        {
            if ( !IsSupported )
            {
                MessageBox.Show( "QQ版本不受支持，请使用新版本修补器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }

            MessageBox.Show( "点击确定后将终止QQ进程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
            Process.Start( "cmd", "/c taskkill /F /IM QQ.exe" )?.WaitForExit();

            lblStatus.Text = "状态：备份QQ.exe";
            if ( !File.Exists( QQDir + $"\\QQ.exe.{QQVersion}.bak" ) )
            {
                File.Copy( QQDir + "\\QQ.exe", QQDir + $"\\QQ.exe.{QQVersion}.bak" );
            }
            else
            {
                MessageBox.Show( "注意，已经找到QQ的备份文件，你之前是否已经对QQ进行过修补？重新修补只会失败。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }

            lblStatus.Text = "状态：修补QQ.exe中";
            if ( radNewVersion.Checked )
            {
                Utils.ReplaceBinaryInFile( QQDir + "\\QQ.exe",
                                           new byte[] {0x48, 0x89, 0xCE, 0x48, 0x8B, 0x11, 0x4C, 0x8B, 0x41, 0x08, 0x49, 0x29, 0xD0, 0x48, 0x8B, 0x49, 0x18, 0xE8},
                                           new byte[] {0x48, 0x89, 0xCE, 0x48, 0x8B, 0x11, 0x4C, 0x8B, 0x41, 0x08, 0x49, 0x29, 0xD0, 0x48, 0x8B, 0x49, 0x18, 0xB8, 0x01, 0x00, 0x00, 0x00} );
            }
            else
            {
                Utils.ReplaceBinaryInFile( QQDir + "\\QQ.exe",
                                           new byte[] {0xAD, 0xA0, 0xB4, 0xAF, 0xA2, 0xA9, 0xA4, 0xB3, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                           new byte[] {0xA3, 0xA0, 0xB4, 0xAF, 0xA2, 0xA9, 0xA4, 0xB3, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                           new byte[] {0xB1, 0xA0, 0xA2, 0xAA, 0xA0, 0xA6, 0xA4, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00},
                                           new byte[] {0xA3, 0xA0, 0xA2, 0xAA, 0xA0, 0xA6, 0xA4, 0xEF, 0xAB, 0xB2, 0xAE, 0xAF, 0x00} );

                PatchJson();
            }

            lblStatus.Text = "状态：完毕";
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
            MessageBox.Show( "处理完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }

        private void btnFixQQCorrupt_Click( object    sender,
                                            EventArgs e )
        {
            if ( !IsSupported )
            {
                MessageBox.Show( "QQ版本不受支持，请使用新版本修补器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }

            MessageBox.Show( "点击确定后将终止QQ进程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
            Process.Start( "cmd", "/c taskkill /F /IM QQ.exe" )?.WaitForExit();

            PatchJson();
            if ( MessageBox.Show( "尝试修复完成！是否立即打开QQ？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
            {
                Process.Start( QQDir + "\\QQ.exe" );
            }
        }

        private void btnRestore_Click( object    sender,
                                       EventArgs e )
        {
            MessageBox.Show( "注意：仅支持还原经过本修补器修补后的QQ。点击确定后自动终止所有QQ进程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );

            Process.Start( "cmd", "/c taskkill /F /IM QQ.exe" )?.WaitForExit();

            lblStatus.Text = "状态：还原QQ中";
            try
            {
                File.Copy( QQDir + $"\\QQ.exe.{QQVersion}.bak", QQDir + "\\QQ.exe", true );
            }
            catch ( FileNotFoundException )
            {
                try
                {
                    File.Copy( QQDir + $"\\QQ.exe.bak", QQDir + "\\QQ.exe", true );
                }
                catch ( FileNotFoundException )
                {
                    MessageBox.Show( "还原出错！QQ.exe备份文件未找到，终止。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }
                catch ( IOException )
                {
                    MessageBox.Show( "还原出错！QQ.exe被占用，请先退出所有QQ进程，终止。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }
            }
            catch ( IOException )
            {
                MessageBox.Show( "还原出错！QQ.exe被占用，请先退出所有QQ进程，终止。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            if ( radOldVersion.Checked )
            {
                try
                {
                    File.Copy( QQDir + "\\resources\\app\\backage.json", QQDir + "\\resources\\app\\package.json", true );
                }
                catch ( FileNotFoundException ) { }
                catch ( IOException )
                {
                    MessageBox.Show( "还原出错！resources\\app\\package.json被占用，请先退出所有QQ进程，终止。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }
            }

            lblStatus.Text = "状态：完毕";
            MessageBox.Show( "还原成功！现在QQ处于未修补状态。请注意，如果你使用了>=1.0版本的框架，需要手动恢复resources/app/app_launcher/index.js。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }

        private void btnManualSelectQQDir_Click( object    sender,
                                                 EventArgs e )
        {
            var result = openFileDialog1.ShowDialog();
            if ( result == DialogResult.OK )
            {
                QQDir = new FileInfo( openFileDialog1.FileName ).DirectoryName;
                var versionData = (JObject) JsonConvert.DeserializeObject( File.ReadAllText( QQDir + "\\resources\\app\\package.json" ) );
                QQVersion = versionData[ "version" ].ToString().Replace( "-", "." ).Trim();
                FrmMain_Load( null, null );
            }
        }

        private void radOldVersion_CheckedChanged( object    sender,
                                                   EventArgs e )
        {
            btnFixQQCorrupt.Enabled = true;
        }

        private void radNewVersion_CheckedChanged( object    sender,
                                                   EventArgs e )
        {
            btnFixQQCorrupt.Enabled = false;
        }
    }
}
