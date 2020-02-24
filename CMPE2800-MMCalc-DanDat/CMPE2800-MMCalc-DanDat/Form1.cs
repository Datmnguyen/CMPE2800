using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace CMPE2800_MMCalc_DanDat
{
    public partial class Form1 : Form
    {
        //main database
        List<ChemElement> _ChemDB = new List<ChemElement>();

        //declare bindingsource for data grid view
        public BindingSource _bs = new BindingSource();

        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttnSortName_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttnSortChar_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttnSortAtomic_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Main firing method when user inputs text in formula box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbxFormula_TextChanged(object sender, EventArgs e)
        {
            //build a regex object to support a particular expression
            Regex reg = new Regex(@"[A-Za-z]+\d+|[A-Za-z]+|(?'group'\((([A-Z][a-z]?[0-9]?)+)\))(?'num'\d+)");

            //Regex regBracket = new Regex(@"([(]+[A-Za-z0-9]+[)]+[0-9])+");   
            //Regex regBracket = new Regex(@"(?'group'\((([A-Z][a-z]?[0-9]?)+)\))(?'num'\d+)");
            //Regex reg = new Regex(@"\([A-Za-z0-9]+\)\d+");
            //Regex reg = new Regex(@"(?'group'\((.*)\))(?'num'\d+)");

            //text input
            string sFormula = txtbxFormula.Text;

            //should split new string if there is round brackets!


            //check if there is a match in the test string
            if (reg.IsMatch(sFormula))
            {
                //get the collection of items that match
                MatchCollection mCollection = reg.Matches(sFormula);

                //loop through the
                foreach (Match match in mCollection)
                {
                    //Console.WriteLine($"{match.Value}");                                 
                    //Console.WriteLine("Numbers : {0}", RegSplitNumber(match.ToString()));

                    //smaller regex
                    Regex regSmallerGroup = new Regex(@"([A-Z][a-z]?[0-9]*)?([0-9]*)");

                    //Regex regSmallerGroup = new Regex(@"[A-Z][a-z]\d+|[A-Z]\d+");
                    //Regex regSmaller = new Regex(@"(?'ele'([a-zA-Z]*)(?'num'[0-9]*))");
                    //Regex regSmaller = new Regex(@"[a-zA-Z]+|\d+");

                    //check smaller match
                    if (regSmallerGroup.IsMatch(match.ToString()))
                    {
                        //get the collection of items that match
                        MatchCollection mSmaller = regSmallerGroup.Matches(match.ToString());

                        //show them in the console
                        foreach (Match gp in mSmaller)
                        {
                            if (gp.ToString().Trim().Length > 0)
                                Console.WriteLine($"{gp.Value}"); 
                            
                            //Console.WriteLine("Numbers : {0}", RegSplitNumber(gp.ToString()));                            
                        }
                    }
                }

                //show them in the datagridview

                //check if the entire collection checked? Recursive option?
            }

            //if (regBracket.IsMatch(sFormula))
            //{
            //    //get the collection of items that match
            //    MatchCollection m = regBracket.Matches(sFormula);

            //    //show them in the console
            //    foreach (Match gp in m)
            //    {
            //        //Console.WriteLine($"{gp.Value}, ");

            //        //Console.WriteLine("Numbers : {0}", RegSplitNumber(gp.ToString()));


            //        //Console.WriteLine($"{gp.Value} : {gp.Groups["group"]}, {gp.Groups["num"]}");
            //    }

            //    //show them in the datagridview

            //    //check if the entire collection checked? Recursive option?
            //}
        }


        private int RegSplitNumber(string sInput)
        {
            int iNumb = 1;

            if (sInput.Trim().Length == 1 && char.IsUpper(sInput[0]))
                return iNumb = 1;

            string[] numbers = Regex.Split(sInput, @"\D+");

            foreach (string val in numbers)
            {
                if (!string.IsNullOrEmpty(val))
                {
                    iNumb = int.Parse(val);                    
                }
            }

            return iNumb;
        }

        //private string[] RegSplitElement(string input)
        //{
        //    string[] sEle = new string[]({0});
        //    string[] arString = Regex.Split(input, @"\d+");

        //    foreach (string s in arString)
        //    {
        //        sEle.Append(s);
        //    }

        //    return sEle;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //set datagridview columns
            //dgvMain.ColumnCount = 4;

            //link DGV and BS
            dgvMain.DataSource = _bs;

            //call function to make Chem DB
            BuildChemDatabase();
        }


        private void DisplayDGV(object obj)
        {
            //link BS to new DB
            _bs.DataSource = null;
            _bs.DataSource = obj;

            //link data grid view to new binding source
            dgvMain.DataSource = null;
            dgvMain.DataSource = _bs;

            //set column headers
            DataGridViewCellStyle columnStyle = new DataGridViewCellStyle();
            columnStyle.BackColor = Color.Aqua;
            dgvMain.ColumnHeadersDefaultCellStyle = columnStyle;

            //format last column width
            dgvMain.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvMain.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        /// <summary>
        /// Build the atomic mass dictionary from supplied csv
        /// </summary>
        private void BuildChemDatabase()
        {
            //make a chemelement obj
            //ChemElement element;

            //declare a string array
            string[] filelines = new string[0];

            //read file with try/catch
            try
            {
                filelines = File.ReadAllLines(@"..\..\..\table_simplified.csv");
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //make data DB = dictionary or datatable or list???
            //make a collection
            var stringSeparators = new string[] { "," };
            var sourcelist = from s in filelines
                             select s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                             into table select table;

            //convert LINQ table to list of chemelement type
            var ChemDB = (from e in sourcelist
                         let n = ConvertInt32(e[0].Trim())
                         let m = ConvertDouble(e[3].Trim())
                         select new ChemElement(n, e[1], e[2], m )).ToList();

            //display on DGV
            DisplayDGV(ChemDB);

            //set DGV headers
            dgvMain.Columns[0].Name = "Atomic Number";
            dgvMain.Columns[1].Name = "Name";
            dgvMain.Columns[2].Name = "Symbol";
            dgvMain.Columns[3].Name = "Mass";
        }

        /// <summary>
        /// Function to check and convert string to int
        /// </summary>
        /// <param name="s">string input</param>
        /// <returns></returns>
        public static int? ConvertInt32(string s)
        {
            int result;
            return Int32.TryParse(s, out result) ? result : default;
        }

        /// <summary>
        /// Function to check and convert string to double
        /// </summary>
        /// <param name="s">string input</param>
        /// <returns></returns>
        public static double? ConvertDouble(string s)
        {
            double result;
            return double.TryParse(s, out result) ? result : default;
        }

    }

    

    /// <summary>
    /// Declare a chem element type
    /// </summary>
    public class ChemElement
    {
        /// <summary>
        /// Public property store actomic number of element
        /// </summary>
        public int? _atomicNumer { get; set; }

        /// <summary>
        /// Public property store actomic symbol of element
        /// </summary>
        public string _atomicSymbol { get; set; }

        /// <summary>
        /// Public property store actomic name of element
        /// </summary>
        public string _atomicName { get; set; }

        /// <summary>
        /// Public property store actomic mass of element
        /// </summary>
        public double? _atomicMass { get; set; }

        /// <summary>
        /// Main CTOR of chem element type
        /// </summary>
        /// <param name="n">ele number</param>
        /// <param name="sym">ele symbol</param>
        /// <param name="name">ele name</param>
        /// <param name="mass">ele mass</param>
        public ChemElement(int? n, string sym, string name, double? mass)
        {
            //assign to props
            _atomicNumer = n;
            _atomicSymbol = sym;
            _atomicName = name;
            _atomicMass = mass;
        }

    }
}
