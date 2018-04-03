using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Opencv_Template_Initializer {
    class Mgmt {

        const String OPEN_CV_WEB = @"https://opencv.org/releases.html";
        //const String REGEX_CV_VER = @"<div class='release-list latest'>[\w\W]*<div class='release-col-1'>[\w\W]*<div class='release-name' itemprop='softwareVersion'>([\w\W]*?)<\/div>";
        const String REGEX_CV_VER_HTML  = @"release-list latest([\w\W]*?)<\/div>";
        const String REGEX_CV_VER_NUMBER = @"(\d+\.\d+\.\d+)";

        Regex re_cv_ver_html = new Regex(REGEX_CV_VER_HTML);
        Regex re_cv_ver_number = new Regex(REGEX_CV_VER_NUMBER);

        public int checkUpdate() {

            int cvVer = -1;

            try {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(OPEN_CV_WEB);

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream s = resp.GetResponseStream()) {
                    using (StreamReader sr = new StreamReader(s)) {
                        String data = sr.ReadToEnd();
                        Match m = re_cv_ver_html.Match(data);
                        if (m.Success) {
                            String ver_html = m.Groups[1].Value;
                            Match m2 = re_cv_ver_number.Match(ver_html);
                            if (m2.Success) {
                                String ver = m2.Groups[1].Value;
                                ver = ver.Replace(".", "");
                                cvVer = int.Parse(ver);
                            }




                        }
                    }
                }

            }catch(Exception ase) {

            }


            return cvVer;
        }
        public void openWeb() {
            System.Diagnostics.Process.Start("iexplore", OPEN_CV_WEB);
        }

    }

}

