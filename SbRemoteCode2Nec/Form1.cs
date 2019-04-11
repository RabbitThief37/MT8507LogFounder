using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

using Excel = Microsoft.Office.Interop.Excel;

namespace SbRemoteCode2Nec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Excel Files (*.xls?)|*.xls?|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("어이..엑셀 파일을 골라야 처리를 해 주지?");
                return;
            }

            Excel.Application _excel;
            Excel.Workbook _excelWorkbook;
            Excel.Worksheet _excelSheet;

            _excel = new Excel.Application();

            _excelWorkbook = _excel.Workbooks.Open(this.openFileDialog1.FileName);
            _excel.Visible = true;
            _excel.UserControl = true;
            _excelSheet = _excelWorkbook.Sheets[4];

            for(int i = 3; i < 67; i++)
            {
                string cellValue = _excelSheet.Cells[i, 4].Value.ToString();
                byte btCode = Convert.ToByte(cellValue, 16);

                byte btValue = 0x01;
                byte btReverse = 0x80;
                byte btReverseCode = 0;
                byte btNotCode = 0;

                for(int j = 0; j < 8; j++ )
                {
                    if( (btCode & btValue) > (byte)0x00)
                    {
                        btReverseCode += btReverse;
                    }
                    else
                    {
                        btNotCode += btReverse;
                    }

                    btValue <<= 1;
                    btReverse >>= 1;
                }

                
                string necCode = string.Format("0x00FF{0:X02}{1:X02}", btReverseCode, btNotCode);
                _excelSheet.Cells[i, 6].Value = necCode;
            }

            _excelWorkbook.Save();

            _excelWorkbook.Close(0);
            _excel.Application.Quit();

            Close();
        }
    }
}
