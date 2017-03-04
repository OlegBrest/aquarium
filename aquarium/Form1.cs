using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace aquarium
{
    public partial class Form1 : Form
    {
        bool stop_this = true;
        List<fish> fishka = new List<fish> { };
        Graphics gr;
        Random rnd = new Random();
        int x_gl = 0;
        int y_gl = 0;
        public Form1()
        {

            InitializeComponent();
            gr = Graphics.FromHwnd(this.pictureBox1.Handle);
            this.x_gl = rnd.Next(this.pictureBox1.Size.Width / 4, this.pictureBox1.Size.Width * 3 / 4);
            this.y_gl = rnd.Next(this.pictureBox1.Size.Height / 4, this.pictureBox1.Size.Height * 3 / 4);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.stop_this = true;

            for (int a = 1; a < (Convert.ToInt32(this.textBox1.Text)); a++)
            {   /*
                for (int x_pos = 0; x_pos < this.pictureBox1.Size.Width; x_pos++)
                {
                    //                   clr = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255), rnd.Next(255));
                    clr = Color.Black;
                    br = new SolidBrush(clr);
                    int pos_y = (int)(Math.Sin((double)(x_pos + (rnd.Next(a))) / 180 * Math.PI) * pictureBox1.Size.Height / 2 + pictureBox1.Size.Height / 2);// = rnd.Next(pictureBox1.Size.Height - 5);
                    gr.FillRectangle(br, x_pos, pos_y, 1, 1);
                }     */

                this.fishka.Add(new fish(this.gr));
            }
            while (this.stop_this)
            {
                this.label1.Text = "Settlers = " + this.fishka.Count.ToString();
                this.label1.Update();
                if (this.fishka.Count == 0) this.stop_this = !this.stop_this;
//                this.pictureBox1.SuspendLayout();
                for (int ind = fishka.Count; ind > 0; ind--)
                {
                    fish cur_fish = this.fishka[ind - 1];
 //                   Thread tr = new Thread(trd_st);
//                    tr.Start(cur_fish);
                    trd_st(cur_fish);  
                    if (cur_fish.repl_curr >= cur_fish.repl_count)
                    {
                        this.fishka.Add(new fish(cur_fish.start_size, cur_fish.start_speed, cur_fish.predator_lvl, rnd.Next(4000, 10000),
                            rnd.Next(3000, 5000), cur_fish.strenght, cur_fish.fld_view, gr,
                            cur_fish.x_coord, cur_fish.y_coord,
                            cur_fish.max_size, cur_fish.max_speed));
                        cur_fish.repl_curr = 0;
                    }
                    if (cur_fish.live_count < 0)
                    {
                        fishka.RemoveAt(ind - 1);
                    }
                }
//                this.pictureBox1.ResumeLayout();
                //                Thread.Sleep(10);
            }

        }
        private void trd_st(object f)
        {
            fish fs = f as fish;
            fs.timer++;
            fs.live_count--;
            if (((int)this.x_gl == (int)fs.x_coord) && ((int)this.y_gl == (int)fs.y_coord))
            {
                this.x_gl = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Width - (int)fs.max_size);
                this.y_gl = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Height - (int)fs.max_size);
                if (this.x_gl > (this.pictureBox1.Size.Width - fs.size)) this.x_gl = (int)(this.pictureBox1.Size.Width - fs.size);
                if (this.y_gl > (this.pictureBox1.Size.Height - fs.size)) this.y_gl = (int)(this.pictureBox1.Size.Height - fs.size);

                this.gr.FillEllipse(Brushes.Black, x_gl, y_gl, 10,10);
            }
            if ((fs.predator_lvl == 0) && ((Math.Sqrt((double)(fs.x_dest * fs.x_dest + fs.y_dest * fs.y_dest)) - (Math.Sqrt((double)(fs.x_coord * fs.x_coord + fs.y_coord * fs.y_coord)))) <= fs.speed))
            {
                fs.x_dest = this.x_gl;
                fs.y_dest = this.y_gl;
            }


            if (((int)fs.x_dest != (int)fs.x_coord) && ((int)fs.y_dest != (int)fs.y_coord))
            {
                fs.mooving(this.pictureBox1.BackColor, Color.Green);
            }
            else
            {
                if (fs.predator_lvl == 0)
                {
                    this.x_gl = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Width - (int)fs.max_size);
                    this.y_gl = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Height - (int)fs.max_size);
                    if (this.x_gl > (this.pictureBox1.Size.Width - fs.size)) this.x_gl = (int)(this.pictureBox1.Size.Width - fs.size);
                    if (this.y_gl > (this.pictureBox1.Size.Height - fs.size)) this.y_gl = (int)(this.pictureBox1.Size.Height - fs.size);

                    fs.x_dest = this.x_gl;
                    fs.y_dest = this.y_gl;
                }
            }

        }

        private void stop_bttn_Click(object sender, EventArgs e)
        {
            this.stop_this = !this.stop_this;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            gr = Graphics.FromHwnd(this.pictureBox1.Handle);
        }
    }

    public class fish : Form
    {
        public float size = 2;       // size of fish  1-smallest , but fastest
        public float start_size = 2;
        public float max_size = 10;
        public double start_speed = 10;
        public double speed = 10;
        public double max_speed = 10;
        public int predator_lvl = 0;  // higher is predator
        public int live_count = 10000; // time to die
        public int repl_count = 7000;   // count to replicate
        public int repl_curr = 0;
        public int strenght = 1;
        public int fld_view = 100;
        Graphics gr;
        public int timer = 0;

        public float x_coord = 300;
        public float y_coord = 300;

        public float x_dest = 0;
        public float y_dest = 0;


        public fish(Graphics gr_par)
        {
            this.speed = this.start_speed / this.size;
            this.gr = gr_par;
            if (this.size > this.max_size) this.size = this.max_size;
            if (this.speed > this.max_speed) this.speed = this.max_speed;
        }
        public fish(float size_new, double speed_new, int pred_new, int live_new, int repl_new,
            int str_new, int fld_new, Graphics gr_par, float x, float y, float max_size_new, double max_speed_new)
        {
            this.start_size = size_new;
            this.size = size_new;
            this.start_speed = speed_new;
            this.predator_lvl = pred_new;
            this.live_count = live_new;
            this.repl_count = repl_new;
            this.strenght = str_new;
            this.fld_view = fld_new;
            this.gr = gr_par;
            this.x_coord = x;
            this.y_coord = y;
            this.max_speed = (max_speed_new > 1 ? max_speed_new : 1);
            this.max_size = (max_size_new > 2 ? max_size_new : 2);

        }

        ~fish() { }

        public void mooving(Color clr_clear, Color clr_show)
        {
            if (this.timer >= (100 / speed))
            {
                double angle = (Math.Atan2((this.y_coord - this.y_dest), (this.x_coord - this.x_dest)));
                double grangl = angle * 180 / Math.PI;
                grangl = grangl < 0 ? 360 + grangl : grangl;
                if (this.size > this.max_size) this.size = this.max_size;
                float x = (float)(this.x_coord - ( Math.Cos(angle)));
                float y = (float)(this.y_coord - ( Math.Sin(angle)));
                if (x < this.size) x = this.size;
                if (y < this.size) y = this.size;


                this.clear(clr_clear);
                this.x_coord = x;
                this.y_coord = y;
                Brush br = new SolidBrush(clr_show);

                this.gr.FillRectangle(br, this.x_coord, this.y_coord, this.size, this.size);


                this.timer = 0;
            }
            repl_curr++;
            this.size = this.size * 1.0002f;
            if (this.size > this.max_size) this.size = this.max_size;
            this.start_size = Math.Abs(this.size / this.live_count);
            if (this.start_size < 2) this.start_size = 2;
            this.speed = this.start_speed / (this.size / 2);
            if (this.speed > this.max_speed) this.speed = this.max_speed;
            if (this.live_count < 0)
            {
                this.clear(clr_clear);
                this.Dispose();
            }

        }
        private void clear(Color clr)
        {
            Brush br = new SolidBrush(clr);
/*            if (this.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                  {       */
                      this.gr.FillRectangle(br, this.x_coord, this.y_coord, this.size, this.size);
//                 });
 //           }
        }
    }
}
