using System;
using System.Drawing;
using System.Linq;
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
            Console.WriteLine("Trying to add " + Program.PacketList.Count + " packets to the packet list");
            int index = 0;
            foreach (var Packet in Program.PacketList)
            {
                listBox2.Items.Add(index + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                index++;
            }
            textBox2.Text = listBox2.Items.Count + "/" + Program.PacketList.Count();
        }

        private unsafe void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.PacketList[listBox2.SelectedIndex].Bytes[0] != 255)
            {
                SelectedPacketSection = listBox1.SelectedIndex;
                SelectedPacketSectionLength = Program.PacketList[listBox2.SelectedIndex].Payload[listBox1.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                richTextBox1.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.PacketList[listBox2.SelectedIndex].Payload[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var ps in Program.PacketList[listBox2.SelectedIndex].Payload)
                {
                    int indexofps = Program.PacketList[listBox2.SelectedIndex].Payload.IndexOf(ps);
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
                SelectedPacketSectionLength = Program.BatchPacketList[listBox3.SelectedIndex].Payload[listBox1.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                richTextBox1.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].Payload[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var ps in Program.BatchPacketList[listBox3.SelectedIndex].Payload)
                {
                    int indexofps = Program.BatchPacketList[listBox3.SelectedIndex].Payload.IndexOf(ps);
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
            //Console.WriteLine(listBox2.SelectedIndex);
            Program.PacketList[listBox2.SelectedIndex] = Program.CreatePacket(Program.PacketList[listBox2.SelectedIndex].Bytes);
            listBox3.Items.Clear();
            listBox1.Items.Clear();
            richTextBox1.Clear();
            foreach (var b in Program.PacketList[listBox2.SelectedIndex].Bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
            }

            foreach (var PacketSection in Program.PacketList[listBox2.SelectedIndex].Payload)
            {
                listBox1.Items.Add(PacketSection.Name + ":" + PacketSection.Payload);
            }

            if (Program.PacketList[listBox2.SelectedIndex].Bytes[0] == 0xFF)
            {
                Program.decodeBatch(Program.PacketList[listBox2.SelectedIndex].Bytes, 0, false);
                int index = 0;
                foreach (var Packet in Program.BatchPacketList)
                {
                    listBox3.Items.Add(index + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                textBox1.Text = listBox3.Items.Count.ToString() + "/" + Program.BatchPacketList.Count().ToString();
            }
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.BatchPacketList[listBox3.SelectedIndex] = Program.CreatePacket(Program.BatchPacketList[listBox3.SelectedIndex].Bytes);
            listBox1.Items.Clear();
            richTextBox1.Clear();
            //Console.WriteLine(listBox3.SelectedIndex);
            foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].Bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
            }

            foreach (var PacketSection in Program.BatchPacketList[listBox3.SelectedIndex].Payload)
            {
                listBox1.Items.Add(PacketSection.Name + ":" + PacketSection.Payload);
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
