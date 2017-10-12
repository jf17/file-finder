using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO.Compression;


// create project: 12.10.2017
// need add ref System.Windows.Forms !!!!
// need add ref System.IO.Compression !!!!
// need add ref System.IO.Compression.FileSystem !!!!

namespace SearchRecentCorrection
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string archive_folder; // папка с Архивом 
        string result_folder; // куда сохранять результат поиска ? 
        string template_for_find; // шаблон для поиска до парсинга
        List<string> templates_for_find; // шаблоны для поиска после парсинга 

        List<string> result_name_list;


        public MainWindow()
        {
            InitializeComponent();
            result_name_list = new List<string>();
        }

        private void BT_archive_folder_Click(object sender, RoutedEventArgs e)
        {
            archive_folder = ChDirDialog();
            Label_archive.Text = archive_folder;
        }

        private void BT_result_folder_Click(object sender, RoutedEventArgs e)
        {
            result_folder = ChDirDialog();
            Label_result.Text = result_folder;
        }

        private void BT_Find_Click(object sender, RoutedEventArgs e)
        {
            BT_Find.IsEnabled = false; // пока идёт процесс , блокируем кнопку 


            if (archive_folder!="" && result_folder != "" && TB_templates.Text != "") {
                parse_templates();
                Find();
                write();
                Label_result_find.Text = "Найдено файлов: " + result_name_list.Count;
            }

         

            templates_for_find.Clear();
            result_name_list.Clear();
            BT_Find.IsEnabled = true; // разблокируем кнопку после процесса 
        }


        private void write() { 

         

        string not_need_files = result_folder+"\\result_folder.txt";

           
            StreamWriter fileNOT = new System.IO.StreamWriter(not_need_files);

                foreach (var fullname in result_name_list)
                    fileNOT.WriteLine(fullname);
                fileNOT.Close();
            }


        private void parse_templates() {

            if (TB_templates.Text != "")
            {

                template_for_find = TB_templates.Text;
                template_for_find=template_for_find.ToUpper();
                TB_templates.Text = template_for_find;


                string[] arraySTR;
                arraySTR = template_for_find.Split(' ');
                templates_for_find = arraySTR.ToList();
               
            }

        }


        private void open_archive_contents(string dir)
        {
            using (ZipArchive zip = ZipFile.Open(dir, ZipArchiveMode.Read))
                foreach (ZipArchiveEntry entry in zip.Entries)
                    result_name_list.Add(entry.Name);
        }

        private void Find() {


            List<string> dirs = new List<string>(Directory.EnumerateDirectories(archive_folder));


            foreach (var dir in dirs)
            {

               
         
                DirectoryInfo dirInf = new DirectoryInfo(dir);

                // Для извлечения имени файла используется цикл foreach и свойство files.name
                foreach (var files in dirInf.GetFiles())
                {
                    string file_name =files.Name ;

                    foreach (var template_name in templates_for_find) {
                        if (file_name.Contains(template_name)) {
                            result_name_list.Add(dir + "\\"+ files.Name);
                            open_archive_contents(dir + "\\" + files.Name);
                        }
                            }

                    
                }
            }


        }


        private string ChDirDialog()
        {
            string resultStr="";

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            fbd.Description = "Выберите директорию...";

            fbd.ShowNewFolderButton = false;



            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && Directory.Exists(fbd.SelectedPath))
            {
                resultStr = fbd.SelectedPath;

           
            }
            return resultStr;
        }




    }
}
