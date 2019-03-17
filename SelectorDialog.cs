using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JoyMouse
{
    public partial class SelectorDialog : Form
    {
        public SelectorDialog(params object[] texts)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(texts);
            comboBox1.SelectedIndex = 0;
        }

        public int SelectedItem => comboBox1.SelectedIndex;
    }
}
