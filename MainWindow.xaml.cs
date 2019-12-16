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
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;

namespace AddTagsIOM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReadWrite RWclass, RWclass1;
        List<string> restList = new List<string>();
        List<string> deviceList = new List<string>();
        readonly Regex regex = new Regex(@"^(-?[1-9]+\d*([.]\d+)?)$|^(-?0[.]\d*[1-9]+)$|^0$|^0.0$");
        readonly Regex regex1 = new Regex(@"^.?[CS]");
        readonly Regex regex2 = new Regex(@"^.?[IRP]");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rightsCombo.Items.Add("R/W");
            rightsCombo.Items.Add("W");
            rightsCombo.Items.Add("R");
            rightsCombo.SelectedIndex = 0;

            typeCombo.Items.Add("Boolean");
            typeCombo.Items.Add("Float");
            typeCombo.Items.Add("String");
            typeCombo.SelectedIndex = 0;

            typeComboRest.Items.Add("Boolean");
            typeComboRest.Items.Add("Float");
            typeComboRest.Items.Add("String");
            typeComboRest.SelectedIndex = 0;
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            string path="";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName.Contains(".txt"))
                {
                    path = ofd.FileName;
                }
                else
                    MessageBox.Show("Err. Wrong file.");
            }

            RWclass = new ReadWrite(path);
            RWclass1 = new ReadWrite(path);
            restList = RWclass.tagList;
            deviceList = RWclass1.tagList;
            numOfTags.Content = string.Format("file contains {0} tags", RWclass.ammount);
            addTagDeviceBtn.IsEnabled = true;
            addTagRestBtn.IsEnabled = true;
            autoTagName.IsEnabled= true;
            autoTagNameRest.IsEnabled = true;
            autoTagType.IsEnabled = true;
            autoTagTypeRest.IsEnabled = true;
        }

        private bool CheckScanrate() 
        {
            if ((regex.IsMatch(scanRate.Text)) && (regex.IsMatch(scanRateRest.Text)))
            {
                return true;
            }
            else 
            {
                MessageBox.Show("Error. Scan rate textbox must be a number.");
                return false;
            }
        }
        private void CreateRestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckScanrate())
            {
                try
                {
                    MergeRestStrings(restList);
                    RWclass.WriteCSV(restList, "REST");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Err. Did you load tag file?\n" + ex.ToString());
                }
            }
        }

        private void CreateDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckScanrate())
            {
                try
                {
                    MergeDeviceStrings(deviceList);
                    RWclass.WriteCSV(deviceList, "Device");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Err. Did you load tag file?\n" + ex.ToString());
                }
            }
        }


        private void AddTagDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckScanrate())
            {
                try
                {
                    RWclass1.AddTag(AddDeviceItem(), "Device");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Err. Did you load tag file?\n" + ex.ToString());
                }
            }
        }

        private void AddTagRestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckScanrate())
            {
                try
                {
                    RWclass.AddTag(AddRestItem(), "REST");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Err. Did you load tag file?\n" + ex.ToString());
                }
            }           
        }

        private void AutoTagName_Click(object sender, RoutedEventArgs e) => ChangeBtns();
        private void AutoTagType_Click(object sender, RoutedEventArgs e) => ChangeBtns();
        private void AutoTagNameRest_Click(object sender, RoutedEventArgs e) => ChangeBtns();
        private void AutoTagTypeRest_Click(object sender, RoutedEventArgs e) => ChangeBtns();

        private void ChangeBtns() 
        { 
            if (autoTagType.IsChecked == true || autoTagName.IsChecked == true)
            {
                createDeviceBtn.IsEnabled = true;
                addTagDeviceBtn.IsEnabled = false;
            } else
            {
                createDeviceBtn.IsEnabled = false;
                addTagDeviceBtn.IsEnabled = true;
            }
            if(autoTagTypeRest.IsChecked == true || autoTagNameRest.IsChecked == true)
            {
                createRestBtn.IsEnabled = true;
                addTagRestBtn.IsEnabled = false;
            } else 
            {
                createRestBtn.IsEnabled = false;
                addTagRestBtn.IsEnabled = true;
            }
        }

        private void MergeRestStrings(List<string> list) 
        {
            string datatype;
            List<string> elements = new List<string>();
            string[] arrElem;

            for (int i = 0; i < list.Count() - 1; i++)
            {
                if (autoTagTypeRest.IsChecked == true)
                {
                    arrElem = list[i].Split('.');
                    elements = arrElem.ToList<string>();
                    datatype = GetDataType(elements.Last());
                }
                else
                    datatype = typeComboRest.Text;

                restList[i] = string.Format("\"Channel0.{3}.{0}\",{2},{1},0.000000,0,1,1", list[i], datatype, scanRateRest.Text, deviceName.Text);
            }
            restList.Insert(0, ";");
            restList.Insert(1 , "; IOTItem");
            restList.Insert(2, ";");
            restList.Insert(3, "Server Tag, Scan Rate,Data Type, Deadband, Send Every Scan, Enabled, Use Scan Rate,");

        }

        private void MergeDeviceStrings(List<string> list)
        {
            string datatype;
            List<string> elements = new List<string>();
            string[] arrElem;
            for (int i = 0; i < list.Count() - 1; i++)
            {
                if (autoTagType.IsChecked == true)
                {
                    arrElem = list[i].Split('.');
                    elements = arrElem.ToList<string>();
                    datatype = GetDataType(elements.Last());
                }
                else
                    datatype = typeComboRest.Text;


                deviceList[i] = string.Format("\"{0}\",\"ns=4;s=|var|{1}.{0}\",{2},1,{3},{4},,,,,,,,,,\"\",", list[i], tagAddress.Text, datatype, rightsCombo.Text, scanRate.Text);
            }
            deviceList.Insert(0, "Tag Name,Address,Data Type,Respect Data Type,Client Access,Scan Rate,Scaling,Raw Low,Raw High,Scaled Low,Scaled High,Scaled Data Type,Clamp Low,Clamp High,Eng Units,Description,Negate Value");
        }

        private string GetDataType(string str)
        {
            string datatype="";
            if (regex1.IsMatch(str))
                datatype = "Boolean";
            else if (regex2.IsMatch(str))
                datatype = "Float";
            else
                datatype = "Default";

            return datatype;
        }


        private string AddRestItem()
        {
            return (string.Format("\"Channel0.{3}.{0}\",{2},{1},0.000000,0,1,1\n", manualTagNameRest.Text, typeComboRest.Text, scanRateRest.Text, deviceName.Text));
        }

        private string AddDeviceItem()
        {
            return (string.Format("\"{0}\",\"ns=4;s=|var|{1}.{0}\",{2},1,{3},{4},,,,,,,,,,\"\",\n", manualTagName.Text, tagAddress.Text, typeCombo.Text, rightsCombo.Text, scanRate.Text));
        }
    }
}
