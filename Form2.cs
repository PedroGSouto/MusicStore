using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MusicStoreAlt
{
    public partial class Form2 : Form
    {
        private static int currentDB2;
        private static SqlConnection cn2;

        public Form2(int currentDB, SqlConnection cn)
        {
            InitializeComponent();
            currentDB2 = currentDB;
            cn2 = cn;
            newChanger();

        }

        private void MusicStore_Load(object sender, EventArgs e)
        {
            SqlCommand getForns = new SqlCommand("Select For_NIF From MusicStore.Fornecedor",cn2);
            SqlDataReader reader = getForns.ExecuteReader();

            while (reader.Read())
            {
                comboBox3.Items.Add(reader[0].ToString());
            }
            reader.Close();
        }


        private void newChanger()
        {
            switch (currentDB2)
            {
                case (6):
                    label1.Text = "Novo Acessório";
                    panel1.Visible = true;
                    Equipment_Options.Visible = false;
                    Instrument_Options.Visible = false;
                    Func_Options.Visible = false;
                    break;

                case (4):
                    label1.Text = "Novo Equipamento";
                    panel1.Visible = true;
                    Instrument_Options.Visible = false;
                    Equipment_Options.Visible = true;
                    Func_Options.Visible = false;
                    break;

                case (3):
                    label1.Text = "Novo Instrumento";
                    panel1.Visible = true;
                    Equipment_Options.Visible = false;
                    Instrument_Options.Visible = true;
                    Func_Options.Visible = false;
                    break;

                case (5):
                    label1.Text = "Novo Software";
                    panel1.Visible = true;
                    Instrument_Options.Visible = false;
                    Equipment_Options.Visible = false;
                    Func_Options.Visible = false;
                    break;
                case (2):
                    label1.Text = "Novo Funcionário";
                    List<String> nif = new List<String>();
                    List<String> end = new List<String>();
                    SqlCommand GetStoreNIF = new SqlCommand("Select Loja_NIF,Endereco From MusicStore.Loja", cn2);
                    SqlDataReader reader = GetStoreNIF.ExecuteReader();
                    while (reader.Read())
                    {
                        nif.Add(reader[0].ToString());
                        end.Add(reader[1].ToString());
                    }

                    for (int i = 0; i < nif.Count; i++)
                    {
                        comboBox2.Items.Add(end[i] + ", " + nif[i]);
                    }
                    reader.Close();
                    panel1.Visible = false;
                    Equipment_Options.Visible = false;
                    Instrument_Options.Visible = false;
                    Func_Options.Visible = true;
                    break;

            }


        }

        private void Instrument_Options_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private bool isFilled() {
            foreach (Panel p in this.Controls.OfType<Panel>()) {
                if (p.Visible) {
                    foreach (TextBox tb in p.Controls.OfType<TextBox>()) {
                        if (tb.Text.Equals("")) {
                            return false;
                        }
                    }
                    foreach (ComboBox cb in p.Controls.OfType<ComboBox>()) {
                        if (cb.SelectedIndex == -1) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isFilled())
            {
                MessageBox.Show("Please fill the form!");
                return;
            }

            switch (currentDB2) {
                case (2):
                    try
                    {
                        SqlCommand com2 = new SqlCommand("MusicStore.NewFuncionario", cn2);
                        com2.CommandType = CommandType.StoredProcedure;

                        com2.Parameters.Clear();

                        com2.Parameters.AddWithValue("@Nome", textBox1.Text);
                        com2.Parameters.AddWithValue("@Fun_NIF", int.Parse(textBox6.Text));
                        com2.Parameters.AddWithValue("@Endereco", textBox9.Text);
                        com2.Parameters.AddWithValue("@Ordenado", decimal.Parse(textBox10.Text));
                        com2.Parameters.AddWithValue("@Telefone", int.Parse(textBox11.Text));
                        com2.Parameters.AddWithValue("@Loja_NIF", int.Parse(comboBox2.SelectedItem.ToString().Substring(comboBox2.SelectedItem.ToString().Length-9)));

                        com2.ExecuteNonQuery();
                        MessageBox.Show("Employee Added!");
                        this.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;


                case (3):
                    try
                    {
                        SqlCommand com3 = new SqlCommand("MusicStore.NewInstrumento", cn2);
                        com3.CommandType = CommandType.StoredProcedure;
                        
                        SqlCommand maxCodeCommand = new SqlCommand("SELECT MAX(Codigo) FROM MusicStore.Produto", cn2);
                        int maxCode = (Int32)maxCodeCommand.ExecuteScalar() + 1;
                       
                        com3.Parameters.Clear();

                        com3.Parameters.AddWithValue("@Codigo", maxCode);
                        com3.Parameters.AddWithValue("@Nome", textBox3.Text);
                        com3.Parameters.AddWithValue("@Marca", textBox2.Text);
                        com3.Parameters.AddWithValue("@Classe", textBox4.Text);
                        com3.Parameters.AddWithValue("@Tipo_Som", comboBox1.SelectedItem.ToString());
                        com3.Parameters.AddWithValue("@Tipo_Voz", textBox7.Text);
                        com3.Parameters.AddWithValue("@Preco_Base", decimal.Parse(textBox5.Text));
                        com3.Parameters.AddWithValue("@Desconto", decimal.Parse(textBox12.Text));
                        com3.Parameters.AddWithValue("@Forn_NIF", int.Parse(comboBox3.Text));

                        com3.ExecuteNonQuery();
                        MessageBox.Show("Intrument Inserted!");
                        this.Close();
                            
                    } 
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case (4):
                    try
                    {
                    SqlCommand com4 = new SqlCommand("MusicStore.NewEquipamento", cn2);
                    com4.CommandType = CommandType.StoredProcedure;

                    SqlCommand maxCodeCommand = new SqlCommand("SELECT MAX(Codigo) FROM MusicStore.Produto", cn2);
                    int maxCode = (Int32)maxCodeCommand.ExecuteScalar() + 1;

                    com4.Parameters.Clear();

                    com4.Parameters.AddWithValue("@Codigo", maxCode);
                    com4.Parameters.AddWithValue("@Nome", textBox3.Text);
                    com4.Parameters.AddWithValue("@Marca", textBox2.Text);
                    com4.Parameters.AddWithValue("@Funcao", textBox8.Text);
                    com4.Parameters.AddWithValue("@Preco_Base", decimal.Parse(textBox5.Text));
                    com4.Parameters.AddWithValue("@Desconto", decimal.Parse(textBox12.Text));
                    com4.Parameters.AddWithValue("@Forn_NIF", int.Parse(comboBox3.Text));

                        com4.ExecuteNonQuery();
                    MessageBox.Show("Equipment Inserted!");
                    this.Close();

                    } catch (Exception ex)
                    {
                     MessageBox.Show(ex.ToString());
                    }
                    break;


                case (5):

                    try
                    {
                        SqlCommand com5 = new SqlCommand("MusicStore.NewSoftware", cn2);
                        com5.CommandType = CommandType.StoredProcedure;

                        SqlCommand maxCodeCommand = new SqlCommand("SELECT MAX(Codigo) FROM MusicStore.Produto", cn2);
                        int maxCode = (Int32)maxCodeCommand.ExecuteScalar() + 1;

                        com5.Parameters.Clear();

                        com5.Parameters.AddWithValue("@Codigo", maxCode);
                        com5.Parameters.AddWithValue("@Nome", textBox3.Text);
                        com5.Parameters.AddWithValue("@Marca", textBox2.Text);
                        com5.Parameters.AddWithValue("@Preco_Base", decimal.Parse(textBox5.Text));
                        com5.Parameters.AddWithValue("@Desconto", decimal.Parse(textBox12.Text));
                        com5.Parameters.AddWithValue("@Forn_NIF", int.Parse(comboBox3.Text));

                        com5.ExecuteNonQuery();
                        MessageBox.Show("Software Inserted!");
                        this.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case (6):
                    try
                    {
                        SqlCommand com6 = new SqlCommand("MusicStore.NewAcessorio", cn2);
                        com6.CommandType = CommandType.StoredProcedure;

                        SqlCommand maxCodeCommand = new SqlCommand("SELECT MAX(Codigo) FROM MusicStore.Produto", cn2);
                        int maxCode = (Int32)maxCodeCommand.ExecuteScalar() + 1;

                        com6.Parameters.Clear();

                        com6.Parameters.AddWithValue("@Codigo", maxCode);
                        com6.Parameters.AddWithValue("@Nome", textBox3.Text);
                        com6.Parameters.AddWithValue("@Marca", textBox2.Text);
                        com6.Parameters.AddWithValue("@Preco_Base", decimal.Parse(textBox5.Text));
                        com6.Parameters.AddWithValue("@Desconto", decimal.Parse(textBox12.Text));
                        com6.Parameters.AddWithValue("@Forn_NIF", int.Parse(comboBox3.Text));

                        com6.ExecuteNonQuery();
                        MessageBox.Show("Accessory Inserted!");
                        this.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;
    
            }
        }
    }
}
