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

namespace Gelo_Sao_Mateus_EMISSOR_NF
{
    public partial class Form1 : Form
    {
        private string scriptName;
        string[] selectedSplit = {};
        List<ObservaçãoCNPJ> observationsByCNPJ = new List<ObservaçãoCNPJ> { };

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedValue = (string)comboBox.SelectedItem;

            selectedSplit = selectedValue.Split('-');

            textBoxNomeFantasia.Text = selectedSplit[2].Trim();
            textBoxInscricaoEstadual.Text = selectedSplit[1].Trim();
            textBoxCnpj.Text = selectedSplit[0].Trim();
            textBoxLogradouro.Text = selectedSplit[4].Trim();
            textBoxNumero.Text = selectedSplit[5].Trim();
            textBoxBairro.Text = selectedSplit[3].Trim();
            textBoxValor3.Text = selectedSplit[6].Trim();

            ObservaçãoCNPJ test = observationsByCNPJ.Find(x => x.CNPJ == selectedSplit[0].Trim());

            if (test != null)
            {
                obsTextBox.Text= test.OBS.ToString().Replace("\"", "").Replace("[", "").Replace("]", "").Trim();
            } else
            {
                obsTextBox.Text = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scriptName = "C:\\Users\\PC\\Documents\\Scripts\\emit_c#.bat";
            
            string jsonString = File.ReadAllText("clients.json");
            Data data = JsonSerializer.Deserialize<Data>(jsonString);
            foreach(Cliente cliente in data.Companies)
            {
                selectEmpresa.Items.Add($"{cliente.CNPJ} - {cliente.IE} - {cliente.NOME_FANTASIA} - {cliente.BAIRRO} - {cliente.LOGRADOURO} - {cliente.NUMERO} - {cliente.VALOR}");

                observationsByCNPJ.Add(new ObservaçãoCNPJ(cliente.CNPJ, cliente.OBS));
            }
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedSplit.Length != 0)
            {
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.FileName = scriptName;
                    processInfo.Arguments = $"\"{textBoxCnpj.Text}-{textBoxNomeFantasia.Text}-{textBoxInscricaoEstadual.Text}-{textBoxLogradouro.Text}-{textBoxNumero.Text}-{textBoxBairro.Text}-{textBoxQuantidade3.Text}-{textBoxValor3.Text}-{textBoxQuantidade5.Text}-{textBoxValor5.Text}\"";
                    Process process = Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
