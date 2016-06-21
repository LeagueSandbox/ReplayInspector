using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
                packetListListBox.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                index++;
            }
            _originalPacketList = Program.PacketList;
            label5.Text = "Showing " + packetListListBox.Items.Count + " packets. Total: " + Program.PacketList.Count();
            label6.Text = "";
            foreach (var PacketCommand in Enum.GetValues(typeof(PacketCmdS2C)))
            {
                packetHeaderComboBox.Items.Add(PacketCommand.ToString());
            }
            packetHeaderComboBox.SelectedIndex = 0;
            Console.WriteLine("GUI loaded");
        }

        private unsafe void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.PacketList[packetListListBox.SelectedIndex].Bytes[0] != 255)
            {
                SelectedPacketSection = packetFieldsListBox.SelectedIndex;
                SelectedPacketSectionLength = Program.PacketList[packetListListBox.SelectedIndex].Payload[packetFieldsListBox.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                packetHexRichTextBox.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.PacketList[listBox2.SelectedIndex].Payload[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var PacketField in Program.PacketList[packetListListBox.SelectedIndex].Payload)
                {
                    int indexOfPacketField = Program.PacketList[packetListListBox.SelectedIndex].Payload.IndexOf(PacketField);
                    if (indexOfPacketField == SelectedPacketSection)
                    {
                        foreach (var b in PacketField.Bytes)
                        {
                            AppendText(packetHexRichTextBox, b.ToString("X2") + " ", Color.LightGreen);
                        }
                    }
                    else
                    {
                        foreach (var b in PacketField.Bytes)
                        {
                            AppendText(packetHexRichTextBox, b.ToString("X2") + " ", packetHexRichTextBox.BackColor);
                        }
                    }
                }
            }
            else
            {
                SelectedPacketSection = packetFieldsListBox.SelectedIndex;
                SelectedPacketSectionLength = Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Payload[packetFieldsListBox.SelectedIndex].Length;
                //Console.WriteLine("Selected index" + SelectedPacketSection);
                packetHexRichTextBox.Clear();

                /*Console.WriteLine("Length: " + SelectedPacketSectionLength);
                foreach (var b in Program.BatchPacketList[listBox3.SelectedIndex].Payload[listBox1.SelectedIndex].Bytes)
                {
                    Console.WriteLine("Bytes: " + b.ToString("X2"));
                }
                Console.WriteLine();*/

                foreach (var ps in Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Payload)
                {
                    int indexofps = Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Payload.IndexOf(ps);
                    if (indexofps == SelectedPacketSection)
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(packetHexRichTextBox, b.ToString("X2") + " ", Color.LightGreen);
                        }
                    }
                    else
                    {
                        foreach (var b in ps.Bytes)
                        {
                            AppendText(packetHexRichTextBox, b.ToString("X2") + " ", packetHexRichTextBox.BackColor);
                        }
                    }
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(listBox2.SelectedIndex);
            Program.PacketList[packetListListBox.SelectedIndex] = Program.CreatePacket(Program.PacketList[packetListListBox.SelectedIndex].Bytes, Program.PacketList[packetListListBox.SelectedIndex].Time);
            batchPacketListListBox.Items.Clear();
            label6.Text = "";
            packetFieldsListBox.Items.Clear();
            packetHexRichTextBox.Clear();
            packetASCIITextBox.Clear();
            foreach (var b in Program.PacketList[packetListListBox.SelectedIndex].Bytes)
            {
                AppendText(packetHexRichTextBox, b.ToString("X2") + " ", packetHexRichTextBox.BackColor);
                if (b >= 0x20 && b <= 0x7E)
                {
                    packetASCIITextBox.Text += Encoding.ASCII.GetString(new byte[] { b });
                }
                else
                {
                    packetASCIITextBox.Text += ".";
                }
            }

            foreach (var PacketSection in Program.PacketList[packetListListBox.SelectedIndex].Payload)
            {
                packetFieldsListBox.Items.Add(PacketSection.Name + ":" + PacketSection.Payload);
            }

            if (Program.PacketList[packetListListBox.SelectedIndex].Bytes[0] == 0xFF)
            {
                Program.decodeBatch(Program.PacketList[packetListListBox.SelectedIndex].Bytes, 0, false);
                int index = 0;
                foreach (var Packet in Program.BatchPacketList)
                {
                    batchPacketListListBox.Items.Add(index + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                label6.Text = "Showing " + batchPacketListListBox.Items.Count.ToString() + " packets. Total: " + Program.BatchPacketList.Count().ToString();
            }
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.BatchPacketList[batchPacketListListBox.SelectedIndex] = Program.CreatePacket(Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Bytes, Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Time);
            packetFieldsListBox.Items.Clear();
            packetHexRichTextBox.Clear();
            packetASCIITextBox.Clear();
            //Console.WriteLine(listBox3.SelectedIndex);
            foreach (var b in Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Bytes)
            {
                AppendText(packetHexRichTextBox, b.ToString("X2") + " ", packetHexRichTextBox.BackColor);
                if (b >= 0x20 && b <= 0x7E)
                {
                    packetASCIITextBox.Text += Encoding.ASCII.GetString(new byte[] { b });
                }
                else
                {
                    packetASCIITextBox.Text += ".";
                }
            }

            foreach (var PacketSection in Program.BatchPacketList[batchPacketListListBox.SelectedIndex].Payload)
            {
                packetFieldsListBox.Items.Add(PacketSection.Name + ":" + PacketSection.Payload);
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
            Program.filtering = filterPacketsCheckBox.Checked;
            packetListListBox.Items.Clear();
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
                    packetListListBox.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                label5.Text = "Showing " + packetListListBox.Items.Count + " packets. Total: " + Program.PacketList.Count();
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
                    packetListListBox.Items.Add(index + " " + time + " cmd: " + (PacketCmdS2C)Packet.Bytes[0]);
                    index++;
                }
                label5.Text = "Showing " + packetListListBox.Items.Count + " packets. Total: " + Program.PacketList.Count();
                label6.Text = "";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Program.filteringSearchInBatch = searchInBatchPacketsCheckBox.Checked;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCommand = packetHeaderComboBox.Items[packetHeaderComboBox.SelectedIndex];
            PacketCmdS2C selectedCommandValue = (PacketCmdS2C)Enum.Parse(typeof(PacketCmdS2C), selectedCommand.ToString());
            Program.filter = (byte)selectedCommandValue;
        }

        private void packetHeaderComboBox_TextUpdate(object sender, EventArgs e)
        {
            if (packetHeaderComboBox.Text.StartsWith("0x"))
            {
                char[] hex = packetHeaderComboBox.Text.Skip(2).ToArray();
                string s = "";
                foreach (var c in hex) {
                    s += c;
                }
                if (s.Length > 0)
                {
                    Program.filter = byte.Parse(s, NumberStyles.HexNumber);
                }
            }
            else if (packetHeaderComboBox.Text.Length > 0)
            {
                Program.filter = byte.Parse(packetHeaderComboBox.Text);
            }
        }
    }
}
