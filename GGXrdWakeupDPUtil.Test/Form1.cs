﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ReversalTool2 reversalTool;

        private void button1_Click(object sender, EventArgs e)
        {
            var dummy = reversalTool.GetDummy();

            MessageBox.Show($"Current Dummy : {dummy.CharName}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reversalTool = new ReversalTool2(Dispatcher.CurrentDispatcher);

            reversalTool.AttachToProcess();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reversalTool.PlayReversal();
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            reversalTool.SetInputInSlot(1, textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reversalTool.SetInputInSlot(2, textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            reversalTool.SetInputInSlot(3, textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = true;
            var slotInput = reversalTool.SetInputInSlot(1, textBox1.Text);

            reversalTool.StartReversalLoop(slotInput);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            button7.Enabled = false;
            reversalTool.StopReversalLoop();
        }
    }
}
