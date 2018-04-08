using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.RegularExpressions;
using static System.Windows.Forms.FolderBrowserDialog;
using System.IO.Packaging;

using System.Security.Principal;
using System.Diagnostics;
using Microsoft.Win32;

namespace Opencv_Template_Initializer
{

    public partial class MainWindow : Window
    {
        public static bool IsAdministrator() {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            if (null != identity) {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return false;
        }

        

        //CheckBox[] chk_project_architecture;
        CheckBox[] chk_project_incl;
        Regex cv_version_local;
        String result_path;
        String vs_template_path;

        int winVerMajor = -1;
        int winVerMinor = -1;
        int winVerBuild = -1;

        const String VS_TEMPLATE_PATH_15 = @"\Visual Studio 2015\Templates\ProjectTemplates";
        const String VS_TEMPLATE_PATH_17 = @"\Visual Studio 2017\Templates\ProjectTemplates";
        const String FILE_SRC = "templates.zip";
        const String FILE_DEST = "OpenCV_Template_VS.zip";
        const String FOLDER_SRC = "templates";

        static bool dirSearchFinded = false;
        int cvVer;


        public void winverParse() {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            Process pro = new Process();
            proInfo.FileName = @"cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardError = true;
            pro.StartInfo = proInfo;
            pro.Start();

            pro.StandardInput.Write("ver" + Environment.NewLine);
            pro.StandardInput.Close();
            string winverData = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            Regex re_winver = new Regex(@"Version (\d+)\.(\d+).(\d+)*");
            Match m = re_winver.Match(winverData);
            if (m.Success) {
                Group Major = m.Groups[1];
                winVerMajor = int.Parse(Major.Value);
                Group Minor = m.Groups[2];
                winVerMinor = int.Parse(Minor.Value);
                Group Build = m.Groups[3];
                winVerBuild = int.Parse(Build.Value);


            } else {
                System.Environment.Exit(-1);
            }

        }

        public String detectSDKVersion(int major, int minor, int build) {
            String sdkVer = "8.1";
            if(major < 6) {
                showMsg("지원하지 않는 운영체제입니다", MessageBoxImage.Error);
                System.Environment.Exit(-1);

            }
            if (major < 10) {
                return sdkVer;
            }
            if (major == 10) {

                try {
                    bool is64 = Environment.Is64BitOperatingSystem;
                    String tmpPath = @"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0";
                    if (!is64) {
                        tmpPath = tmpPath.Replace(@"WOW6432Node\", "");
                    }

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(tmpPath)) {
                        if (key != null) {
                            sdkVer = key.GetValue("ProductVersion").ToString();
                            sdkVer += ".0";
                        }
                    }
                } catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
                  {
                    //react appropriately
                }


            }

            return sdkVer;
        }

        Dictionary<String, CheckBox> chk_project_incl_dict;

        public MainWindow()
        {
            InitializeComponent();

            if (!IsAdministrator()) {
                try {
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                    procInfo.WorkingDirectory = Environment.CurrentDirectory;
                    procInfo.Verb = "runas";
                    Process.Start(procInfo);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message.ToString());
                }

                System.Environment.Exit(0);
                return;
            }

            winverParse();

            //chk_project_architecture = new CheckBox[] { chk_project_x86r, chk_project_x86d, chk_project_x64r, chk_project_x64d};
            chk_project_incl = new CheckBox[] { chk_project_incl_main, chk_project_incl_lena, chk_project_incl_wildlife };

            
            chk_project_incl_dict = new Dictionary<String, CheckBox>();
            chk_project_incl_dict.Add("main.cpp", chk_project_incl_main);
            chk_project_incl_dict.Add("lena.png", chk_project_incl_lena);
            chk_project_incl_dict.Add("Wildlife.mp4", chk_project_incl_wildlife);

            vs_template_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + VS_TEMPLATE_PATH_15;
            txt_vs_path.Text = vs_template_path;



            cv_version_local = new Regex(@"opencv_world(\d*)");
            String result_path = "";
            cvVer = -1;
        }

        public void showMsg(String msg, MessageBoxImage type) {
            MessageBox.Show(msg, "info", MessageBoxButton.OK, type);
        }

        private void btn_cv_path_Click(object sender, RoutedEventArgs e){

            System.Windows.Forms.FolderBrowserDialog ofd_path = new System.Windows.Forms.FolderBrowserDialog();
            ofd_path.ShowDialog();

            dirSearchFinded = false;
            result_path = ofd_path.SelectedPath;
            if (result_path == "") {
                showMsg("경로 선택이 취소되었습니다", MessageBoxImage.Error);
                txt_cv_path.Text = "";
                lbl_cv_version_local.Content = "";
                return;
            }

            // CV Check Routine
            dirSearchFinded = dirSearch(result_path, "opencv_world*");

            // New CV Version Check
            // https://opencv.org/releases.html

            txt_cv_path.Text = result_path;

        }

        public bool dirSearch(string sDir, string findStr) {
            try {

                foreach (string d in Directory.GetDirectories(sDir)) {
                    foreach (string f in Directory.GetFiles(d, findStr)) {

                        Match match = cv_version_local.Match(f);
                        Group g = match.Groups[1];
                        String version = g.Captures[0].Value;
                        lbl_cv_version_local.Content = version;
                        try {
                            cvVer = int.Parse(version);
                            if (cvVer >= 310) {

                                rad_project_x86.IsChecked = false;
                                rad_project_x86.IsEnabled = false;
                                rad_project_x64.IsChecked = true;
                                showMsg("이 OpenCV 버전에서는 32bit 플랫폼을 지원하지 않습니다.", MessageBoxImage.Information);
                            }
                        } catch(Exception e) {

                        }
                        return true;
                    }
                    
                    bool result = dirSearch(d, findStr);
                    if (result) {
                        return true;
                    }
                }

                return false;

            } catch (System.Exception excpt) {
                Console.WriteLine(excpt.Message);
                return false;

            }
        }

        public bool[] convertChkValue(CheckBox[] arr) {
            int array_len = arr.Length;
            bool[] architecture_val = new bool[array_len];
            for(int i=0; i < array_len; i++) {
                architecture_val[i] = (bool)arr[i].IsChecked;
            }
            return architecture_val;
        }

        private void run_execute_init_Click(object sender, RoutedEventArgs e) {


            if (!dirSearchFinded) {
                showMsg("올바른 경로설정이 되어있지 않습니다", MessageBoxImage.Asterisk);
                return;
            }

            int vsVer;
            if (rad_vs_2015.IsChecked == true) {
                vsVer = 140;
            } else {
                vsVer = 141;
            }
            
            try {

                FileManager fm = new FileManager();

                fm.cleanFolder(FOLDER_SRC);
                fm.decompress(FILE_SRC, FOLDER_SRC);
                String osVer = detectSDKVersion(winVerMajor, winVerMinor, winVerBuild);
                WizardHandler wh = new WizardHandler(osVer, vsVer, cvVer, result_path);

                //wh.change_doc2_platform_toolset(convertChkValue(chk_project_architecture));
                wh.change_doc2_platform_toolset((bool)rad_project_x64.IsChecked);
                wh.change_doc2_windows_target_platform();

                wh.change_doc1_incl(chk_project_incl_dict);
                wh.change_windows_path((bool)rad_project_x64.IsChecked);

                fm.compress(FOLDER_SRC, FILE_DEST);
                fm.cleanFolder(FOLDER_SRC);
                fm.copyFile(FILE_DEST, vs_template_path + @"\" + FILE_DEST);
                FileManager.deleteFile(FILE_DEST);

                showMsg("설정이 완료되었습니다", MessageBoxImage.Information);
                showMsg("Visual Studio를 재 시작 하십시오", MessageBoxImage.Information);

            } catch (Exception ex) {
                showMsg("설정중 에러 발생", MessageBoxImage.Error);
                showMsg(ex.StackTrace, MessageBoxImage.Error);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            showMsg("OpenCV Intializer 1.3.0\n\n" +
                "Myoa Engineering\n" +
                "myoatm@gmail.com\n"
                ,MessageBoxImage.Information);
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e) {
            System.Environment.Exit(0);

        }

        private void btn_mgmt_check_update_Click(object sender, RoutedEventArgs e) {
            Mgmt mgmt = new Mgmt();
            int cvVer = mgmt.checkUpdate();

            lbl_cv_version_latest.Content = cvVer.ToString();

        }

        private void btn_mgmt_visit_web_Click(object sender, RoutedEventArgs e) {
            Mgmt mgmt = new Mgmt();
            mgmt.openWeb();

        }

        private void rad_vs_2015_Checked(object sender, RoutedEventArgs e) {
            vs_template_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + VS_TEMPLATE_PATH_15;
            txt_vs_path.Text = vs_template_path;
        }

        private void rad_vs_2017_Checked(object sender, RoutedEventArgs e) {
            vs_template_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + VS_TEMPLATE_PATH_17;
            txt_vs_path.Text = vs_template_path;
        }

        private void btn_vs_path_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.FolderBrowserDialog ofd_path = new System.Windows.Forms.FolderBrowserDialog();
            ofd_path.ShowDialog();

            dirSearchFinded = false;
            String tmp_path = ofd_path.SelectedPath;
            if (tmp_path == "") {
                showMsg("경로 선택이 취소되었습니다", MessageBoxImage.Error);
                return;
            }


            vs_template_path = tmp_path;
            txt_vs_path.Text = vs_template_path;
        }
    }
}
