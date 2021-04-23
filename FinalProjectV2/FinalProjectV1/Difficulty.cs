using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProjectV1
{
    public partial class Difficulty : Form
    {
        public Difficulty()
        {
            InitializeComponent();
            cbDifficulty.SelectedIndex = 1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Game game = new Game(this.cbDifficulty.SelectedIndex);
            game.FormClosing += delegate { this.Show(); };
            game.Show();
            this.Hide();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHow_Click(object sender, EventArgs e)
        {
            HowToPlay howToPlay = new HowToPlay();
            howToPlay.Show();
        }
    }
}
