using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PcapDecrypt
{
    public partial class MainWindow : Form
    {
        int SelectedPacketSection;
        int SelectedPacketSectionLength;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            foreach (var Packet in Program.PacketList)
            {
                listBox2.Items.Add(Program.PacketList.IndexOf(Packet) + Packet.Data[0].Name + ":" + (PacketCmdS2C)Packet.Data[0].Data);
            }
            textBox2.Text = listBox2.Items.Count.ToString() + "/" + Program.PacketList.Count().ToString();
        }

        private unsafe void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.PacketList[listBox2.SelectedIndex].bytes[0] != 255)
            {
                SelectedPacketSection = listBox1.SelectedIndex;
                SelectedPacketSectionLength = Program.PacketList[listBox2.SelectedIndex].Data[listBox1.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                richTextBox1.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.PacketList[listBox2.SelectedIndex].Data[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var ps in Program.PacketList[listBox2.SelectedIndex].Data)
                {
                    int indexofps = Program.PacketList[listBox2.SelectedIndex].Data.IndexOf(ps);
                    if (indexofps == SelectedPacketSection)
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(richTextBox1, b.ToString("X2") + " ", Color.LightGreen);
                        }
                    }
                    else
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
                        }
                    }
                }
            }
            else
            {
                SelectedPacketSection = listBox1.SelectedIndex;
                SelectedPacketSectionLength = Program.BatchPacketList[listBox3.SelectedIndex].Data[listBox1.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                richTextBox1.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].Data[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var ps in Program.BatchPacketList[listBox3.SelectedIndex].Data)
                {
                    int indexofps = Program.BatchPacketList[listBox3.SelectedIndex].Data.IndexOf(ps);
                    if (indexofps == SelectedPacketSection)
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(richTextBox1, b.ToString("X2") + " ", Color.LightGreen);
                        }
                    }
                    else
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
                        }
                    }
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox1.Items.Clear();
            richTextBox1.Clear();
            foreach (var b in Program.PacketList[listBox2.SelectedIndex].bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
            }

            foreach (var PacketSection in Program.PacketList[listBox2.SelectedIndex].Data)
            {
                listBox1.Items.Add(PacketSection.Name + ":" + PacketSection.Data);
            }

            if (Program.PacketList[listBox2.SelectedIndex].bytes[0] == 255)
            {
                Program.decodeBatch(Program.PacketList[listBox2.SelectedIndex].bytes, 0, false);
                foreach (var Packet in Program.BatchPacketList)
                {
                    listBox3.Items.Add(Program.BatchPacketList.IndexOf(Packet) + Packet.Data[0].Name + ":" + (PacketCmdS2C)Packet.Data[0].Data);
                }
                textBox1.Text = listBox3.Items.Count.ToString() + "/" + Program.BatchPacketList.Count().ToString();
            }
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            richTextBox1.Clear();
            //Console.WriteLine(listBox3.SelectedIndex);
            foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
            }

            foreach (var PacketSection in Program.BatchPacketList[listBox3.SelectedIndex].Data)
            {
                listBox1.Items.Add(PacketSection.Name + ":" + PacketSection.Data);
            }
        }

        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionBackColor = color;
            box.AppendText(text);
            box.SelectionBackColor = box.ForeColor;
        }
    }
}
