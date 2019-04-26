using System;
using System.Windows.Forms;
//pc
namespace MemoryGame
{
    public partial class SuperMemory : Form
    {
        public SuperMemory()
        {
            InitializeComponent();
            Game.setGame(this);
        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            Game.startGame((Button)sender);
        }

        public void setScore(int score)
        {
            label_score.Text = "Score : " + score;
        }

        public void setResult(String result)
        {
            label_result.Text = ""+result;
        }
    }
}