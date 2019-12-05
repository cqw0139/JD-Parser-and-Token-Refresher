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
using fileRead;
using Microsoft.Win32;
using ApiMethod;
using System.Collections;
using MS_343_JD_Parser;

namespace JDParser

    public partial class MainWindow : Window
    {

        private ArrayList path = new ArrayList();
        private int index = 0;
        private bool contractHub = false;
        private bool threeFourThree = false;

        public MainWindow()
        {
            InitializeComponent();
        }

    // the method which will be called when user click the send button
        private void sendFiles(object sender, RoutedEventArgs e)
        {
        // initialize the apiMethod class for further method related with api  
            apiMethod apiMethod = new apiMethod();
            string contactToken = apiMethod.getAccessToken(apiMethod.AccessURL, apiMethod.ContactRefreshToken);
            bool errorExisted = false;
            if (path.Count != 0)
            {
                for (int i = 0; i < index; i++)
                {
                    try
                    {        
                        if(contractHub == true)
                        {
                            StringBuilder text = fileRead.fileRead.ReadFileToString(path[i]);
                            string[] jsonAndNote = MSJDParser.JDParser.parsing(text, contactToken);
                            int jobId = apiMethod.sendJob(jsonAndNote[0]);
                            apiMethod.addNote(jobId, jsonAndNote[1]);
                            //filePathList.Text += Environment.NewLine + jsonAndNote[0];
                        }
                        else if (threeFourThree == true)
                        {
                            StringBuilder text = fileRead.fileRead.ReadFileToString(path[i]);
                            string json = MS_343_JD_Parser.JDParser.parsing(text, contactToken);
                            //filePathList.Text += Environment.NewLine + json;
                            int jobId = apiMethod.sendJob(json);
                        }
                        else
                        {
                            MessageBox.Show("Unexpected file format ratio button error 2");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorExisted = true;
                        log.Text += path[i].ToString() + " caused some error." + Environment.NewLine;
                        MessageBox.Show(path[i].ToString() + " cannot be sent with some unexpected error" + ex.Message);
                    }
                }
                if (errorExisted == true)
                {
                    log.Text += "All files except the file caused error have been sent successfully." + Environment.NewLine;
                }
                else
                {
                    log.Text += "All files have been sent successfully." + Environment.NewLine;
                }
            }
            else
            {
                MessageBox.Show("No file selected");
            }
        }

    // click "..." button will call this method, it will call C# build-in file select method
        private void selectFile(object sender, RoutedEventArgs e)
        {
            bool ratioButton = false;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;
            dlg.Multiselect = true;

            // Set filter for file extension and default file extension 
            if (contractHub == false && threeFourThree == false)
            {
                MessageBox.Show("Please select the format of file first");
            }
            else if (contractHub == true)
            {
                dlg.DefaultExt = ".docx";
                dlg.Filter = "DOCX Files (*.docx)|*.docx";
                ratioButton = true;
            }
            else if (threeFourThree == true)
            {
                dlg.DefaultExt = ".txt";
                dlg.Filter = "txt files (*.txt)|*.txt";
                ratioButton = true;
            }
            else
            {
                MessageBox.Show("Unexpected file format ratio button error 1");
            }

            index = 0;
            path.Clear();

            filePath.Text = "";
            filePathList.Text = "";

            if (ratioButton == true)
            {

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    foreach (String file in dlg.FileNames)
                    {
                        try
                        {
                            // Open document 
                            path.Add(file);
                            filePath.Text += path[index].ToString();
                            filePathList.Text += path[index].ToString() + " has added." + Environment.NewLine;
                            index++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: Could not read file from disk. error: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void selectPaths(object sender, RoutedEventArgs e)
        {
            
            filePath.Text = path.ToString();
            filePathList.Text = path.ToString() + " has added." + Environment.NewLine;
        }

    //when file format is changed to contract Hub, this method will be called and the previous selected file with other format will be removed
        private void contractHubChecked(object sender, RoutedEventArgs e)
        {
            if (path.Count == 0)
            {
            }
            else
            {
                path.Clear();
                filePathList.Text = "";
                filePathList.Text = "Previously selected file has removed." + Environment.NewLine;
            }
            contractHub = true;
            threeFourThree = false;
        }

    //when file format is changed to 343, this method will be called and the previous file with other format will be removed
        private void threeFourThreeChecked(object sender, RoutedEventArgs e)
        {
            if (path.Count == 0)
            {
            }
            else
            {
                path.Clear();
                filePathList.Text = "";
                filePathList.Text = "Previously selected file has removed." + Environment.NewLine;
            }
            contractHub = false;
            threeFourThree = true;
        }
    }
}
