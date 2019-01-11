using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using System.Threading;

namespace CurrectTheHomework
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.StandardInput.WriteLine("mkdir exe");
            cmd.StandardInput.WriteLine("mkdir out");
            cmd.StandardInput.WriteLine("mkdir diff");
            cmd.StandardInput.WriteLine("exit");
            cmd.WaitForExit();
            cmd.Close();
            log = File.AppendText("./compile.log");
        }
        StreamWriter log;
        void LogAndConsole(string value)
        {
            log.WriteLine(value);
            Console.WriteLine(value);
        }
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            files.Items.Clear();
            foreach (var f in Directory.EnumerateFiles(@"./cpp/"))
            {
                if (!f.Contains("class"))
                    files.Items.Add(f);
            }
        }

        private void recompile_Click(object sender, RoutedEventArgs e)
        {
            if (files.SelectedItem == null)
                return;
            var fi = new FileInfo((string)files.SelectedItem);
            var compileResult = CompileAndExecCpp(fi, $@".\exe\", @".\out\", File.ReadAllLines(@".\input.txt"),force:true);
            @out.Text = compileResult[1];
            var diffResult = DiffTheAnswer(fi, @".\out\", @".\diff\", new FileInfo(@".\answer.txt").FullName, force: true);
            diff.Text = diffResult[1];

        }
        Regex regex_header = new Regex(@"/\*(?:(?!\*/).)*\*/", RegexOptions.Singleline);
        Regex regex_allcomment = new Regex(@"\/\/.*");
        private void files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var file = (string)(sender as ListView).SelectedItem;
            if (file == null)
                return;
            var fi = new FileInfo((string)files.SelectedItem);
            var str_utf8 = File.ReadAllText(file, Encoding.UTF8);
            var str_big5 = File.ReadAllText(file, Encoding.GetEncoding(950));
            source_utf8.Text = str_utf8 ?? "";
            source_big5.Text = str_big5 ?? "";
            /*
            string header = $"{fi.DirectoryName}\\{Path.GetFileNameWithoutExtension(fi.Name)}class.h";
            string headerCpp = $"{fi.DirectoryName}\\{Path.GetFileNameWithoutExtension(fi.FullName)}class.cpp";
            str_utf8 = File.ReadAllText(header, Encoding.UTF8);
            str_big5 = File.ReadAllText(header, Encoding.GetEncoding(950));
            h_utf8.Text = str_utf8 ?? "";
            h_big5.Text = str_big5 ?? "";
            str_utf8 = File.ReadAllText(headerCpp, Encoding.UTF8);
            str_big5 = File.ReadAllText(headerCpp, Encoding.GetEncoding(950));
            hcpp_utf8.Text = str_utf8 ?? "";
            hcpp_big5.Text = str_big5 ?? "";*/
            #region header
            if (regex_header.IsMatch(source_utf8.Text))
            {
                header_utf8.Text = regex_header.Match(source_utf8.Text).Value;
            }
            else
            {
                header_utf8.Text = "No Header";
            }

            if (regex_header.IsMatch(source_big5.Text))
            {
                header_big5.Text = regex_header.Match(source_big5.Text).Value;
            }
            else
            {
                header_big5.Text = "No Header";
            }
            #endregion

            #region src_comment
            comment_utf8.Text = "";
            if (regex_allcomment.IsMatch(source_utf8.Text))
            {
                foreach(Match m in regex_allcomment.Matches(source_utf8.Text))
                {
                    comment_utf8.Text += m.Value;
                }

            }
            else
            {
                comment_utf8.Text = "No Comment";
            }
            comment_big5.Text = "";
            if (regex_allcomment.IsMatch(source_big5.Text))
            {
                foreach (Match m in regex_allcomment.Matches(source_big5.Text))
                {
                    comment_big5.Text += m.Value;
                }

            }
            else
            {
                comment_big5.Text = "No Comment";
            }
            #endregion

            #region header_comment
            /*
            h_comment_utf8.Text = "";
            if (regex_allcomment.IsMatch(h_comment_utf8.Text))
            {
                foreach (Match m in regex_allcomment.Matches(h_comment_utf8.Text))
                {
                    h_comment_utf8.Text += m.Value;
                }

            }
            else
            {
                h_comment_utf8.Text = "No Comment";
            }
            h_comment_big5.Text = "";
            if (regex_allcomment.IsMatch(h_comment_big5.Text))
            {
                foreach (Match m in regex_allcomment.Matches(h_comment_big5.Text))
                {
                    h_comment_big5.Text += m.Value;
                }

            }
            else
            {
                h_comment_big5.Text = "No Comment";
            }
            #endregion
            #region hcpp_comment
            hcpp_comment_utf8.Text = "";
            if (regex_allcomment.IsMatch(hcpp_utf8.Text))
            {
                foreach (Match m in regex_allcomment.Matches(hcpp_utf8.Text))
                {
                    hcpp_comment_utf8.Text += m.Value;
                }

            }
            else
            {
                hcpp_comment_utf8.Text = "No Comment";
            }
            hcpp_comment_big5.Text = "";
            if (regex_allcomment.IsMatch(hcpp_big5.Text))
            {
                foreach (Match m in regex_allcomment.Matches(hcpp_big5.Text))
                {
                    hcpp_comment_big5.Text += m.Value;
                }

            }
            else
            {
                hcpp_comment_big5.Text = "No Comment";
            }*/
            #endregion

            if (File.Exists($@"./diff/{fi.Name}.diff"))
            {
                diff.Text = File.ReadAllText(($@"./diff/{fi.Name}.diff"));
            }
            else
            {
                diff.Text = "";
            }
            if (File.Exists($@"./out/{fi.Name}.txt"))
            {
                @out.Text = File.ReadAllText(($@"./out/{fi.Name}.txt"));
            }
            else
            {
                @out.Text = "";
            }

        }


        private void recompile_All_Click(object sender, RoutedEventArgs e)
        {
            LogAndConsole("recompile_All_Click");
            Task.Run(() =>
            {
                LogAndConsole($"[{DateTime.Now.ToShortTimeString()}]Compile All Start");
                foreach (string file in files.Items)
                {
                    
                    var fi = new FileInfo(file);
                    var compileResult = CompileAndExecCpp(fi, $@".\exe\", @".\out\",File.ReadAllLines(@".\input.txt"));
                    var diffResult = DiffTheAnswer(fi, @".\out\", @".\diff\", new FileInfo(@".\answer.txt").FullName);
                }
                LogAndConsole($"[{DateTime.Now.ToShortTimeString()}]Compile All End");
            });

        }



        Process process = new Process();
        ExecProgramResult ExecProgram(string path, string args = "", string[] stdin = null)
        {

            LogAndConsole($"[{DateTime.Now.ToShortTimeString()}] #{process.GetHashCode()} Execute {path} {args}");
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            if (stdin != null)
            {
                foreach (var str in stdin)
                {
                    process.StandardInput.WriteLine(str);
                }
            }
            var output = process.StandardOutput.ReadToEnd();
            var errout = process.StandardError.ReadToEnd();
            process.WaitForExit(5000);
            if (!process.HasExited)
            {
                process.Kill();
                LogAndConsole($"[{DateTime.Now.ToShortTimeString()}] #{process.GetHashCode()} Execute TimeOut!");
            }
            else
            {
                LogAndConsole($"[{DateTime.Now.ToShortTimeString()}] #{process.GetHashCode()} Execute Complete!");
            }

            var exit = process.ExitCode;
            process.Close();

            return new ExecProgramResult
            {
                ExitCode = exit,
                StdOut = output,
                StdErr = errout
            };
        }
        string[] CompileAndExecCpp(FileInfo fileinfo, string exeOutPath, string txtOutPath,string[] input, bool force = false)
        {
            var exepath = $"{new DirectoryInfo(exeOutPath).FullName}{fileinfo.Name}.exe";
            var txtpath = $"{txtOutPath}{fileinfo.Name}.txt";
            var compileResult = new ExecProgramResult()
            {
                ExitCode = 0, StdOut = "", StdErr = ""
            };
            var execResult = new ExecProgramResult()
            {
                ExitCode = 0,
                StdOut = "",
                StdErr = ""
            };
            if (force || !File.Exists(exepath))
            {
                string header = $"{fileinfo.DirectoryName}\\{Path.GetFileNameWithoutExtension(fileinfo.Name)}class.h";
                string headerCpp = $"{fileinfo.DirectoryName}\\{Path.GetFileNameWithoutExtension(fileinfo.FullName)}class.cpp";
                compileResult = ExecProgram(
                    @"C:\Windows\System32\cmd.exe",
                    $"/c g++ \"{fileinfo.FullName}\" -o \"{exepath}\"");
                if (compileResult.ExitCode != 0)
                {
                    File.WriteAllText(txtpath, compileResult.StdErr);
                    return new string[] { txtpath, compileResult.StdErr };
                }
            }
            if (force || !File.Exists(txtpath))
            {
                execResult = ExecProgram(@"C:\Windows\System32\cmd.exe", $"/c \"{exepath}\"", input);
                File.WriteAllText(txtpath, execResult.StdOut);
                return new string[] { txtpath, execResult.StdOut };
            }
            return new string[] { txtpath, File.ReadAllText(txtpath) };
        }
        string[] DiffTheAnswer(FileInfo fileinfo, string txtOutPath, string diffOutPath, string answerPath, bool force = false)
        {
            var txtpath = new FileInfo($"{txtOutPath}{fileinfo.Name}.txt").FullName;
            var diffpath = $"{diffOutPath}{fileinfo.Name}.diff";
            var diffOut = "";
            var diffResult = new ExecProgramResult()
            {
                ExitCode = 0, StdOut = "", StdErr = ""
            };
            if (force || !File.Exists(diffpath))
            {
                diffResult = ExecProgram(
                    @"C:\Windows\System32\cmd.exe",
                    $"/c diff -w -y \"{answerPath}\" \"{txtpath}\"");
                if (string.IsNullOrWhiteSpace(diffResult.StdOut))
                    diffOut = "No diff";
                else
                    diffOut = diffResult.StdOut;
                File.WriteAllText(diffpath, diffOut);
                return new string[] { diffpath, diffOut };
            }
            return new string[] { diffpath, File.ReadAllText(diffpath) };
        }
    }

    struct ExecProgramResult
    {
        public int ExitCode;
        public string StdOut;
        public string StdErr;
        public override string ToString()
        {
            return
                $"ExitCode: {ExitCode}\n" +
                $"StdOut: {StdOut}\n" +
                $"StdErr: {StdErr}\n";
        }
    }
}
