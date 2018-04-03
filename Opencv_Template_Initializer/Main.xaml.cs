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
        static bool dirSearchFinded = false;
        int cvVer;

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


            //chk_project_architecture = new CheckBox[] { chk_project_x86r, chk_project_x86d, chk_project_x64r, chk_project_x64d};
            chk_project_incl = new CheckBox[] { chk_project_incl_main, chk_project_incl_lena, chk_project_incl_wildlife };

            
            chk_project_incl_dict = new Dictionary<String, CheckBox>();
            chk_project_incl_dict.Add("main.cpp", chk_project_incl_main);
            chk_project_incl_dict.Add("lena.png", chk_project_incl_lena);
            chk_project_incl_dict.Add("Wildlife.mp4", chk_project_incl_wildlife);
            


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

            Version osVer = Environment.OSVersion.Version;
            float osVerDetail;
            if(osVer.Major < 6) {
                showMsg("now supported.", MessageBoxImage.Error);
                return;
            }

            int vsVer;
            if (rad_vs_2015.IsChecked == true) {
                vsVer = 140;
            } else {
                vsVer = 141;
            }
            
            osVerDetail = osVer.Major + (osVer.Minor * 0.1f);

            try {

                GarbageCollector gc = new GarbageCollector();
                gc.cleanFolder("templates");
                gc.decompress("templates.zip", "templates");

                WizardHandler wh = new WizardHandler(osVerDetail, vsVer, cvVer, result_path);
                //wh.change_doc2_platform_toolset(convertChkValue(chk_project_architecture));
                wh.change_doc2_platform_toolset(true);
                wh.change_doc2_windows_target_platform();

                wh.change_doc1_incl(chk_project_incl_dict);
                wh.change_windows_path((bool)rad_project_x64.IsChecked);

                gc.compress("templates", "OpenCV_Template_VS.zip");
                gc.cleanFolder("templates");


                showMsg("설정이 완료되었습니다", MessageBoxImage.Information);
                showMsg("Visual Studio를 재 시작 하십시오", MessageBoxImage.Information);

            } catch (Exception ex) {
                showMsg("설정중 에러 발생", MessageBoxImage.Error);
                showMsg(ex.StackTrace, MessageBoxImage.Error);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            showMsg("OpenCV Intializer 1.0\n\n" +
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
    }
}
