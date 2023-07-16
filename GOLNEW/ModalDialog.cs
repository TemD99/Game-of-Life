using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLNEW
{
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }

      

      

        public int Miliseconds
        {
            get
            {
                return (int)numericUpDownMili.Value;
            }
            set
            {
                numericUpDownMili.Value = value;
            }
        }

        public int CellHeight
        {
            get
            {
                return (int)numericUpDownHeight.Value;
            }
            set
            {
                numericUpDownHeight.Value = value;
            }
        }
        public int CellWidth
        {
            get
            {
                return (int)numericUpDownWidth.Value;
            }
            set
            {
                numericUpDownWidth.Value = value;
            }
        }
    }
}
