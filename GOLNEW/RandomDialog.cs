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
    public partial class RandomDialog : Form
    {
        public RandomDialog()
        {
            InitializeComponent();
        }

        public int RandomSeedNumber
        {
            get
            {
                return (int)RandomSeed.Value;
            }
            set
            {
                RandomSeed.Value = value;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            RandomSeed.Value = random.Next(0, 2000000);
        }
    }
}
