using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Opencv_Template_Initializer {


    class WizardHandler {

        // MyTemplate.vstemplate
        // OpenCV_Template.vcxproj
        // OpenCV_Template.vcxproj.filters

        XmlDocument doc1, doc2;

        String PLATFORM_TOOLSET;
        String TOOLS_VERSION;

        String WINDOWS_TARGET_PLATFORM;

        String CV_PATH;
        String CV_TOOLS_VER;
        String CV_PATH_LIB_X64 = @"{0}\build\x64\{1}\lib;%(AdditionalLibraryDirectories)";
        String CV_PATH_LIB_X86 = @"{0}\build\x86\{1}\lib;%(AdditionalLibraryDirectories)";
        String CV_PATH_HEADER = @"{0}\build\include;%(AdditionalIncludeDirectories)";
        String CV_LIB_NAME = "opencv_world{0}";
        String CV_LIB_NAME_ENDS_WITH = ".lib;%(AdditionalDependencies)";

        String archi_x64 = "x64";
        String archi_x86 = "Win32";
        String archi_Release = "Release";
        String archi_Debug = "Debug";

        //bool[] chkOption;

        String CV_VER;

        String XML_FILE_DOC1 = @"templates\MyTemplate.vstemplate";
        String XML_FILE_DOC2 = @"templates\OpenCV_Template.vcxproj";


        public WizardHandler(float osVer, int vsVer, int cvVer, String cv_path) {

            if (osVer <= 6.1f) {
                WINDOWS_TARGET_PLATFORM = "7";
            } else if (osVer < 10) {
                WINDOWS_TARGET_PLATFORM = "8.1";

            } else {
                WINDOWS_TARGET_PLATFORM = "10.0.10240.0";
            }

            if (vsVer == 141) {
                PLATFORM_TOOLSET = "v141";
                TOOLS_VERSION = "15.0";
                CV_TOOLS_VER = "v15";
            } else {
                PLATFORM_TOOLSET = "v140";
                TOOLS_VERSION = "14.0";
                CV_TOOLS_VER = "v14";

            }

            CV_PATH = cv_path;
            CV_PATH_LIB_X64 = CV_PATH_LIB_X64.Replace("{0}", cv_path);
            CV_PATH_LIB_X86 = CV_PATH_LIB_X86.Replace("{0}", cv_path);

            CV_PATH_HEADER = CV_PATH_HEADER.Replace("{0}", cv_path);
            CV_VER = cvVer.ToString();
            CV_LIB_NAME = CV_LIB_NAME.Replace("{0}", CV_VER);

            //chkOption = _chkOption;

            //string tmp = File.ReadAllText(@"\templates\MyTemplate.vstemplate");
            //doc1 = new XmlDocument();
            //doc1.LoadXml("MyTemplate.vstemplate");

            //tmp = File.ReadAllText();
            try {
                /*
                doc1 = new XmlDocument();
                doc1.Load(@"templates\MyTemplate.vstemplate");

                doc2 = new XmlDocument();
                doc2.Load(@"templates\OpenCV_Template.vcxproj");
                */
            } catch (Exception a) {

            }

            


        }

        ~WizardHandler() {
            
        }

        public void change_windows_path(bool setX64) {
            String win_path = Environment.GetEnvironmentVariable("Path");
            String new_path = CV_PATH + @"\build\" + (setX64 == true ? @"x64\" : @"x86\") + CV_TOOLS_VER + @"\bin";
            if (win_path.IndexOf(new_path) >= 0) {
                // already exist
                return;
            }
            Environment.SetEnvironmentVariable("Path", win_path + ";" + new_path, EnvironmentVariableTarget.Machine);

        }

        public void change_doc2_platform_toolset(bool setX64) {
            //XmlDocument doc = doc2;
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILE_DOC2);


            XmlNodeList cursorList;
            XmlNode cursor;

            XmlNode root = doc.DocumentElement;
            root.Attributes.GetNamedItem("ToolsVersion").ChildNodes[0].Value = TOOLS_VERSION;



            foreach (XmlNode node in root.ChildNodes) {
                if (node.Name == "PropertyGroup" && node.Attributes.GetNamedItem("Label") != null && node.Attributes.GetNamedItem("Label").Value == "Configuration") {
                    cursor = node.ChildNodes[2];
                    cursor.FirstChild.Value = PLATFORM_TOOLSET;

                }

                if (node.Name == "ItemDefinitionGroup") {
                    String platform = node.Attributes.GetNamedItem("Condition").Value;
                    bool isDebug = platform.IndexOf(archi_Debug) >= 0;
                    bool isX64 = platform.IndexOf(archi_x64) >= 0;

                    XmlNode node_clCompile = node.ChildNodes[0];
                    XmlNode node_link = node.ChildNodes[1];

                    XmlElement elem_clCompile1 = doc.CreateElement("AdditionalIncludeDirectories", doc.DocumentElement.NamespaceURI);

                    XmlElement elem_link1 = doc.CreateElement("AdditionalLibraryDirectories", doc.DocumentElement.NamespaceURI);
                    XmlElement elem_link2 = doc.CreateElement("AdditionalDependencies", doc.DocumentElement.NamespaceURI);

                    // [0] == x86d. Checking Architecture and The Option was checked.
                    //if (!isDebug && !isX64 && chkOption[0]) {
                    if (!isDebug && !isX64 && !setX64) {
                        elem_clCompile1.InnerText = CV_PATH_HEADER;
                        elem_link1.InnerText = CV_PATH_LIB_X86;
                        elem_link2.InnerText = CV_LIB_NAME + CV_LIB_NAME_ENDS_WITH;

                        node_clCompile.AppendChild(elem_clCompile1);
                        node_link.AppendChild(elem_link1);
                        node_link.AppendChild(elem_link2);

                    } else if (isDebug && !isX64 && !setX64) {
                        elem_clCompile1.InnerText = CV_PATH_HEADER;
                        elem_link1.InnerText = CV_PATH_LIB_X86;
                        elem_link2.InnerText = CV_LIB_NAME + "d" + CV_LIB_NAME_ENDS_WITH;

                        node_clCompile.AppendChild(elem_clCompile1);
                        node_link.AppendChild(elem_link1);
                        node_link.AppendChild(elem_link2);

                    } else if (!isDebug && isX64 && setX64) {
                        elem_clCompile1.InnerText = CV_PATH_HEADER;
                        elem_link1.InnerText = CV_PATH_LIB_X64;
                        elem_link2.InnerText = CV_LIB_NAME + CV_LIB_NAME_ENDS_WITH;

                        node_clCompile.AppendChild(elem_clCompile1);
                        node_link.AppendChild(elem_link1);
                        node_link.AppendChild(elem_link2);

                    } else if (isDebug && isX64 && setX64) {
                        elem_clCompile1.InnerText = CV_PATH_HEADER;
                        elem_link1.InnerText = CV_PATH_LIB_X64;
                        elem_link2.InnerText = CV_LIB_NAME + "d" + CV_LIB_NAME_ENDS_WITH;

                        node_clCompile.AppendChild(elem_clCompile1);
                        node_link.AppendChild(elem_link1);
                        node_link.AppendChild(elem_link2);

                    }


                }
            }

            doc.Save(XML_FILE_DOC2);
        }
        public void change_doc2_windows_target_platform() {
            //XmlDocument doc = doc2;
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILE_DOC2);


            XmlNodeList cursorList;
            XmlNode cursor;

            XmlNode root = doc.DocumentElement;

            foreach (XmlNode node in root.ChildNodes) {
                if (node.Name == "PropertyGroup" && node.Attributes.GetNamedItem("Label") != null && node.Attributes.GetNamedItem("Label").Value == "Globals") {
                    cursor = node.ChildNodes[3];
                    cursor.FirstChild.Value = WINDOWS_TARGET_PLATFORM;
                
                }

            }

            doc.Save(XML_FILE_DOC2);
        }

        public void change_doc1_incl(Dictionary<String, CheckBox> dict) {
            // XmlDocument doc = doc1;
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILE_DOC1);

            XmlNodeList cursorList;
            XmlNode cursor;
            XmlNode root = doc.DocumentElement;

            foreach (XmlNode node in root.ChildNodes) {
                if (node.Name == "TemplateContent") {
                    cursor = node.FirstChild;

                    foreach(String key in dict.Keys) {
                        if ((bool)dict[key].IsChecked) {
                            XmlElement elem_ProjectItem = doc.CreateElement("ProjectItem", doc.DocumentElement.NamespaceURI);
                            elem_ProjectItem.SetAttribute("ReplaceParameters", "false");
                            elem_ProjectItem.SetAttribute("TargetFileName", key);
                            elem_ProjectItem.InnerText = key;
                            cursor.AppendChild(elem_ProjectItem);
                        } else {
                            FileManager.deleteFile(@"templates\" + key);
                        }
                    }
                }


            }


            doc.Save(XML_FILE_DOC1);

        }

    }


}
