using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PcapDecrypt
{
    public partial class MainWindow : Form
    {
        int SelectedPacketSection;
        int SelectedPacketSectionLength;
        private List<Packets.Packet> _originalPacketList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Loading GUI, please wait...");
            Console.WriteLine("Trying to add " + Program.PacketList.Count + " packets to the packet list");
            int index = 0;
            foreach (var Packet in Program.PacketList)
            {
                var tSent = TimeSpan.FromMilliseconds(Packet.Time);
                var time = tSent.ToString("mm\\:ss\\.ffff");
                listBox2.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                index++;
            }
            _originalPacketList = Program.PacketList;
            label5.Text = "Showing " + listBox2.Items.Count + " packets. Total: " + Program.PacketList.Count();
            label6.Text = "";
            foreach (var PacketCommand in Enum.GetValues(typeof(PacketCmdS2C)))
            {
                comboBox1.Items.Add(PacketCommand.ToString());
            }
            comboBox1.SelectedIndex = 0;
            Console.WriteLine("GUI loaded");
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

                foreach (var PacketField in Program.PacketList[listBox2.SelectedIndex].Payload)
                {
                    int indexOfPacketField = Program.PacketList[listBox2.SelectedIndex].Payload.IndexOf(PacketField);
                    if (indexOfPacketField == SelectedPacketSection)
                    {
                        foreach (var b in PacketField.Bytes)
                        {
                            AppendText(richTextBox1, b.ToString("X2") + " ", Color.LightGreen);
                        }
                    }
                    else
                    {
                        foreach (var b in PacketField.Bytes)
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
            Program.PacketList[listBox2.SelectedIndex] = Program.CreatePacket(Program.PacketList[listBox2.SelectedIndex].Bytes, Program.PacketList[listBox2.SelectedIndex].Time);
            listBox3.Items.Clear();
            label6.Text = "";
            listBox1.Items.Clear();
            richTextBox1.Clear();
            textBox1.Clear();
            foreach (var b in Program.PacketList[listBox2.SelectedIndex].Bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
                textBox1.Text += Encoding.ASCII.GetString(new byte[] { b });
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
                label6.Text = "Showing " + listBox3.Items.Count.ToString() + " packets. Total: " + Program.BatchPacketList.Count().ToString();
            }
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.BatchPacketList[listBox3.SelectedIndex] = Program.CreatePacket(Program.BatchPacketList[listBox3.SelectedIndex].Bytes, Program.BatchPacketList[listBox3.SelectedIndex].Time);
            listBox1.Items.Clear();
            richTextBox1.Clear();
            textBox1.Clear();
            //Console.WriteLine(listBox3.SelectedIndex);
            foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].Bytes)
            {
                AppendText(richTextBox1, b.ToString("X2") + " ", richTextBox1.BackColor);
                textBox1.Text += Encoding.ASCII.GetString(new byte[] { b });
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Program.filtering = checkBox1.Checked;
            listBox2.Items.Clear();
            if (Program.filtering)
            {
                List<Packets.Packet> temp = new List<Packets.Packet>();
                foreach (var Packet in Program.PacketList)
                {
                    if (Program.filteringSearchInBatch && Packet.Bytes[0] == 0xFF)
                    {
                        try
                        {
                            Program.decodeBatch(Packet.Bytes, 0, false);
                            foreach (var PacketInBatch in Program.BatchPacketList)
                            {
                                if (PacketInBatch.Bytes[0] == Program.filter)
                                {
                                    temp.Add(Packet);
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                    if (Packet.Bytes[0] == Program.filter)
                    {
                        temp.Add(Packet);
                    }
                }
                Program.PacketList = temp;

                int index = 0;
                foreach (var Packet in Program.PacketList)
                {
                    var tSent = TimeSpan.FromMilliseconds(Packet.Time);
                    var time = tSent.ToString("mm\\:ss\\.ffff");
                    listBox2.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                label5.Text = "Showing " + listBox2.Items.Count + " packets. Total: " + Program.PacketList.Count();
                label6.Text = "";
            }
            else
            {
                Program.PacketList = _originalPacketList;

                int index = 0;
                foreach (var Packet in Program.PacketList)
                {
                    var tSent = TimeSpan.FromMilliseconds(Packet.Time);
                    var time = tSent.ToString("mm\\:ss\\.ffff");
                    listBox2.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                label5.Text = "Showing " + listBox2.Items.Count + " packets. Total: " + Program.PacketList.Count();
                label6.Text = "";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Program.filteringSearchInBatch = checkBox2.Checked;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCommand = comboBox1.Items[comboBox1.SelectedIndex];
            PacketCmdS2C selectedCommandValue = (PacketCmdS2C)Enum.Parse(typeof(PacketCmdS2C), selectedCommand.ToString());
            Program.filter = (byte)selectedCommandValue;
        }
    }
}
