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
        int main_handl = 0;

        public Form1()
        {

            InitializeComponent();
            this.DoubleBuffered = true;
            gr = Graphics.FromHwnd(this.pictureBox1.Handle);
            this.x_gl = rnd.Next(this.pictureBox1.Size.Width / 4, this.pictureBox1.Size.Width * 3 / 4);
            this.y_gl = rnd.Next(this.pictureBox1.Size.Height / 4, this.pictureBox1.Size.Height * 3 / 4);

            // thread for starting draw
            Thread drawer = new Thread(draw_it);
            drawer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.stop_this = true;

            for (int a = 1; a < (Convert.ToInt32(this.textBox1.Text)); a++)
            {
                this.fishka.Add(new fish(2, 10, 0, 10000, 4000, 1, 100, 300, 300, 500, 100, 15, 15, Color.Green));
                this.fishka.Add(new fish(2, 12, 1, 20000, 20000, 2, 150, 200, 200, 300, 300, 15, 16, Color.Blue));
                this.fishka.Add(new fish(2, 14, 2, 30000, 30000, 3, 200, 500, 500, 500, 300, 15, 17, Color.Red));
            }
            if (main_handl != 0)
            {
                this.stop_this = false;
                Thread.Sleep(100);
                this.stop_this = true;
            }
            Thread tr_main = new Thread(main);
            tr_main.Start();
            Thread tr_vic = new Thread(vic_finder);
            tr_vic.Start();

        }

        private void draw_it()
        {
            Color clr_clear = this.pictureBox1.BackColor;
            while (this.stop_this)
            {
               // gr.Clear(clr_clear);
                for (int ind = fishka.Count; ind > 0; ind--)
                {
                    try
                    {
                        fish cur_fish = this.fishka[ind - 1];

                        if (cur_fish.live_count <= 0)
                        {
                            this.clear(clr_clear, cur_fish, gr);
                        }
                        else
                        {
                            this.clear(clr_clear, cur_fish, gr);
                            cur_fish.x_coord = cur_fish.x_coord_next;
                            cur_fish.y_coord = cur_fish.y_coord_next;
                            Brush br = new SolidBrush(cur_fish.fish_clr);                                            
                            gr.FillRectangle(br, cur_fish.x_coord, cur_fish.y_coord, cur_fish.size, cur_fish.size);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                
            }
        }

        private void clear(Color clr, fish crnt_fish, Graphics gr)
        {
            Brush br = new SolidBrush(clr);
            this.gr.FillRectangle(br, crnt_fish.x_coord, crnt_fish.y_coord, crnt_fish.size, crnt_fish.size);
        }

        private void vic_finder()
        {
            while (this.stop_this)
            {
                // trying to find targets
                for (int ind = fishka.Count; ind > 0; ind--)
                {
                    try
                    {
                        fish cur_fish = this.fishka[ind - 1];
                        cur_fish.victim_finded = null;
                        for (int ind2 = fishka.Count; ind2 > 0; ind2--)
                        {
                            fish next_fish = this.fishka[ind2 - 1];

                            float delta_x = (next_fish.x_coord - cur_fish.x_coord);
                            float delta_y = (next_fish.y_coord - cur_fish.y_coord);
                            float range = (float)Math.Sqrt(delta_x * delta_x + delta_y * delta_y);
                            double angle = (Math.Atan2(delta_y, delta_x)) + Math.PI;
                            //                            double grangl = angle * 180 / Math.PI;
                            //                            grangl = grangl < 0 ? 360 + grangl : grangl;
                            if (cur_fish.fld_view >= range)
                            {
                                if (cur_fish.victim_finded == null) cur_fish.target_range = range;

                                // find victim
                                if ((cur_fish.predator_lvl > next_fish.predator_lvl) && (cur_fish.victim_finded != false))
                                {
                                    if (cur_fish.target_range < range) cur_fish.target_range = range;
                                    cur_fish.x_dest = next_fish.x_coord;
                                    cur_fish.y_dest = next_fish.y_coord;
                                    cur_fish.victim_finded = true;
                                }
                                // if is not victim
                                if (cur_fish.predator_lvl < next_fish.predator_lvl)
                                {
                                    if (cur_fish.target_range < range) cur_fish.target_range = range;
                                    cur_fish.victim_finded = false;
                                    float x = (float)(cur_fish.x_coord + cur_fish.max_speed * 5 * (Math.Cos(angle)));
                                    float y = (float)(cur_fish.y_coord + cur_fish.max_speed * 5 * (Math.Sin(angle)));
                                    if (x > (this.pictureBox1.Size.Width - cur_fish.size)) x = (int)(this.pictureBox1.Size.Width - cur_fish.size);
                                    if (y > (this.pictureBox1.Size.Height - cur_fish.size)) y = (int)(this.pictureBox1.Size.Height - cur_fish.size);
                                    if (x < cur_fish.size) x = cur_fish.size;
                                    if (y < cur_fish.size) y = cur_fish.size;
                                    cur_fish.x_dest = x;
                                    cur_fish.y_dest = y;
                                }

                                // if catch it
                                if ((range < cur_fish.speed) && (cur_fish.predator_lvl > next_fish.predator_lvl) && (cur_fish.id_victim == ind2))
                                {
                                    cur_fish.repl_curr++;
                                    cur_fish.strenght++;
                                    next_fish.live_count -= cur_fish.strenght;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                }

            }
            Thread.CurrentThread.Abort();
        }


        private void main()
        {
            while (this.stop_this)
            {
                Invoke((MethodInvoker)delegate
              {
                  this.label1.Text = "Settlers (0)= " + this.fishka.FindAll(fishka => fishka.predator_lvl == 0).Count.ToString() +
                  " (1)= " + this.fishka.FindAll(fishka => fishka.predator_lvl == 1).Count.ToString() + " (2)= " + this.fishka.FindAll(fishka => fishka.predator_lvl == 2).Count.ToString();
                  this.label1.Update();
              });
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
                        this.fishka.Add(new fish(cur_fish.start_size, cur_fish.start_speed, cur_fish.predator_lvl,
                            rnd.Next((int)(cur_fish.live_start * 0.99), (int)(cur_fish.live_start * 1.01)),
                            rnd.Next((int)(cur_fish.start_repl * 0.99), (int)(cur_fish.start_repl * 1.01)),
                            cur_fish.strenght_start, cur_fish.fld_view, cur_fish.x_coord, cur_fish.y_coord, cur_fish.x_dest, cur_fish.y_dest,
                            cur_fish.max_size, cur_fish.max_speed, cur_fish.fish_clr));
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
            Thread.CurrentThread.Abort();
            return;
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
            }
            if ((fs.predator_lvl == 0) && (Math.Abs((Math.Sqrt((double)(fs.x_dest * fs.x_dest + fs.y_dest * fs.y_dest)) - (Math.Sqrt((double)(fs.x_coord * fs.x_coord + fs.y_coord * fs.y_coord))))) <= fs.speed))
            {
                fs.x_dest = this.x_gl;
                fs.y_dest = this.y_gl;
            }

            // if not target for predator try to random point
            if ((fs.predator_lvl > 0) && (fs.victim_finded == null) && (Math.Sqrt((fs.x_dest - fs.x_coord) * (fs.x_dest - fs.x_coord) + (fs.y_dest - fs.y_coord) * (fs.y_dest - fs.y_coord))) < fs.speed) 
            {
                fs.x_dest = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Width - (int)fs.max_size);
                fs.y_dest = rnd.Next((int)fs.max_size, this.pictureBox1.Size.Height - (int)fs.max_size);
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

        public int live_start = 10000;
        public int start_repl = 7000;

        public int predator_lvl = 0;  // higher is predator 
        public int live_count = 10000; // time to die
        public int repl_count = 7000;   // count to replicate
        public int repl_curr = 0;
        public int strenght = 1;
        public int strenght_start = 1;
        public int fld_view = 100;   // field of view
        public int timer = 0;

        /// <summary>
        ///  id of target
        /// </summary>
        public int? id_victim = null;

        /// <summary>
        /// victim_finded: null - target not finded ; true - target is victim ; false - this fish is victim 
        /// </summary>
        public bool? victim_finded = null;

        /// <summary>
        /// target range
        /// </summary>
        public float target_range = 0;

        public float x_coord = 300;
        public float y_coord = 300;

        public float x_coord_next = 300;
        public float y_coord_next = 300;

        public float x_dest = 0;
        public float y_dest = 0;

        public Color fish_clr = Color.Green;

        public fish()
        {
            this.speed = this.start_speed / this.size;
            if (this.size > this.max_size) this.size = this.max_size;
            if (this.speed > this.max_speed) this.speed = this.max_speed;
        }

        public fish(float size_new, double speed_new, int pred_new, int live_new, int repl_new,
            int str_new, int fld_new, float x, float y, float x_dest_new, float y_dest_new, float max_size_new, double max_speed_new, Color clr_new)
        {
            this.start_size = size_new;
            this.size = size_new;
            this.start_speed = speed_new;
            this.predator_lvl = pred_new;
            this.start_repl = repl_new;
            this.live_start = live_new;
            this.live_count = live_new;
            this.repl_count = repl_new;
            this.strenght_start = str_new;
            this.fld_view = fld_new;
            this.x_coord = x;
            this.y_coord = y;
            this.x_coord_next = x;
            this.y_coord_next = y;
            this.x_dest = x_dest_new;
            this.y_dest = y_dest_new;
            this.max_speed = (max_speed_new > 1 ? max_speed_new : 1);
            this.max_size = (max_size_new > 2 ? max_size_new : 2);
            this.fish_clr = clr_new;

        }

        ~fish() { }

        public void mooving(Color clr_clear, Color clr_show)
        {
            if (this.timer >= (100 / speed))
            {
                double angle = (Math.Atan2((this.y_coord - this.y_dest), (this.x_coord - this.x_dest)));
                //                double grangl = angle * 180 / Math.PI;
                //                grangl = grangl < 0 ? 360 + grangl : grangl;
                if (this.size > this.max_size) this.size = this.max_size;
                float x = (float)(this.x_coord - (Math.Cos(angle)));
                float y = (float)(this.y_coord - (Math.Sin(angle)));
                if (x < this.size) x = this.size;
                if (y < this.size) y = this.size;

                this.x_coord_next = x;
                this.y_coord_next = y;
                this.timer = 0;
            }
            repl_curr++;
            this.size = this.start_size + this.max_size * (((float)(this.live_start-this.live_count)) / ((float)this.live_start));
            if (this.size > this.max_size) this.size = this.max_size;
            this.start_size = Math.Abs(this.size / this.live_count);
            if (this.start_size < 2) this.start_size = 2;
            this.speed = this.start_speed / (this.size / 2);
            if (this.speed > this.max_speed) this.speed = this.max_speed;
        }
    }
}
