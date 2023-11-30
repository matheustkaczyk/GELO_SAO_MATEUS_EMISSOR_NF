using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Diagnostics;
using System.Xml.Linq;
using System.Threading;
using System.Text.Json;

namespace Gelo_Sao_Mateus_EMISSOR_NF
{
    public partial class Form1 : Form
    {
        private string scriptName;
        string[] selectedSplit = {};
        List<ObservaçãoCNPJ> observationsByCNPJ = new List<ObservaçãoCNPJ> { };
        bool isEditable = false;
        bool shouldPrint = false;
        List<Cliente> clients;

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedValue = (string)comboBox.SelectedItem;

            selectedSplit = selectedValue.Split('-');

            textBoxCnpj.Text = selectedSplit[0].Trim();
            textBoxInscricaoEstadual.Text = selectedSplit[1].Trim();
            textBoxNomeFantasia.Text = selectedSplit[2].Trim();
            textBoxBairro.Text = selectedSplit[3].Trim();
            textBoxLogradouro.Text = selectedSplit[4].Trim();
            textBoxNumero.Text = selectedSplit[5].Trim();
            textBoxValor3.Text = selectedSplit[6].Trim();

            ObservaçãoCNPJ foundCNPJ = observationsByCNPJ.Find(x => x.CNPJ == selectedSplit[0].Trim());

            if (foundCNPJ != null && foundCNPJ.OBS.ToString() != "[]")
            {
                string parsedFoundOBS = foundCNPJ.OBS.ToString().Replace("\"", "").Replace("[", "").Replace("]", "").Trim();
                double foundDays = double.Parse(parsedFoundOBS.Split(' ')[2].ToString());

                DateTime localDate = DateTime.Now;

                obsTextBox.Text = $"" +
                    $"{parsedFoundOBS}\n" +
                    $"{localDate.AddDays((foundDays))}";
            } else
            {
                obsTextBox.Text = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scriptName = "C:\\Users\\PC\\Documents\\Scripts\\emit_c#.bat";

            deSerializeJson();
            renderClients(this.clients);
            editInputs();
            scriptExists();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Arquivo script|*.bat";
            openFileDialog1.Title = "Selecione o script python";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                scriptName = openFileDialog1.FileName;

                if (string.IsNullOrEmpty(scriptName))
                {
                    scriptName = "C:\\Users\\PC\\Documents\\Scripts\\emit_c#.bat";
                }
            }

            scriptExists();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedSplit.Length != 0)
            {
                try
                {
                    Thread newThread = new Thread(() => startProcess());
                    newThread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void startProcess()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = scriptName;
            processInfo.Arguments = $"\"{textBoxCnpj.Text}-{textBoxNomeFantasia.Text}-{textBoxInscricaoEstadual.Text}-{textBoxLogradouro.Text}-{textBoxNumero.Text}-{textBoxBairro.Text}-{textBoxQuantidade3.Text}-{textBoxValor3.Text}-{textBoxQuantidade5.Text}-{textBoxValor5.Text}-{checkBox2.Checked}\"";
            Process.Start(processInfo);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                isEditable = true;
            } else
            {
                isEditable = false;
            }

            editInputs();
        }

        private void editInputs()
        {
            if (isEditable)
            {
                textBoxNomeFantasia.ReadOnly = false;
                textBoxInscricaoEstadual.ReadOnly = false;
                textBoxCnpj.ReadOnly = false;
                textBoxLogradouro.ReadOnly = false;
                textBoxNumero.ReadOnly = false;
                textBoxBairro.ReadOnly = false;
                textBoxValor3.ReadOnly = false;
                button3.Enabled= true;
            } else
            {
                textBoxNomeFantasia.ReadOnly = true;
                textBoxInscricaoEstadual.ReadOnly = true;
                textBoxCnpj.ReadOnly = true;
                textBoxLogradouro.ReadOnly = true;
                textBoxNumero.ReadOnly = true;
                textBoxBairro.ReadOnly = true;
                textBoxValor3.ReadOnly = true;
            }


        }

        private void saveClient()
        {
            string currentClientCNPJ = selectedSplit[0].Trim();

            int clientIndex = this.clients.FindIndex(c => c.CNPJ == currentClientCNPJ);

            Cliente updatedClient = new Cliente();

            updatedClient.CNPJ = textBoxCnpj.Text;
            updatedClient.IE = textBoxInscricaoEstadual.Text;
            updatedClient.NOME_FANTASIA = textBoxNomeFantasia.Text;
            updatedClient.BAIRRO = textBoxBairro.Text;
            updatedClient.LOGRADOURO = textBoxLogradouro.Text;
            updatedClient.NUMERO = textBoxNumero.Text;
            updatedClient.VALOR = textBoxValor3.Text;

            clients[clientIndex] = updatedClient;

            renderClients(this.clients);
        }

        private void scriptExists()
        {
            if (File.Exists(scriptName))
            {
                label18.Visible = true;
                label19.Visible = false;
            } else
            {
                label18.Visible = false;
                label19.Visible = true;
            }
        }

        private void deSerializeJson()
        {
            string jsonString = File.ReadAllText("clients.json");
            Data data = JsonSerializer.Deserialize<Data>(jsonString);

            clients = data.Companies;
        }

        private void serializeJson(List<Cliente> clientsData)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(clientsData, options);

            Console.WriteLine(jsonString);
        }

        private void renderClients(List<Cliente> clients)
        {
            foreach (Cliente cliente in clients)
            {
                selectEmpresa.Items.Add($"{cliente.CNPJ} - {cliente.IE} - {cliente.NOME_FANTASIA} - {cliente.BAIRRO} - {cliente.LOGRADOURO} - {cliente.NUMERO} - {cliente.VALOR}");

                observationsByCNPJ.Add(new ObservaçãoCNPJ(cliente.CNPJ, cliente.OBS));
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                shouldPrint = true;
            }
            else
            {
                isEditable = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveClient();
        }
    }
}
