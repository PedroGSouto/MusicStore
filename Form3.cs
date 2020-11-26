using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicStoreAlt
{
    public partial class Form3 : Form
    {
        private static String row2mod;
        private static SqlConnection cn2;
        private static int code2;
        private SqlDataAdapter adapter;
        private BindingSource bindingSource1 = new BindingSource();
        DataTable dt;

        public Form3(String row, SqlConnection cn, int code)
        {
            InitializeComponent();
            dataGridView1.DataSource = bindingSource1;
            row2mod = row;
            cn2 = cn;
            code2 = code;
            label2.Text = row;

            SqlCommand forn= new SqlCommand("Select Forn_NIF From MusicStore.Fornecedores_Prod Where Codigo_Produto=@Codigo", cn2);
            forn.Parameters.Clear();
            forn.Parameters.AddWithValue("@Codigo",code2);
            SqlDataReader reader = forn.ExecuteReader();
            ArrayList forns = new ArrayList();
            while (reader.Read())
            {
                forns.Add(reader[0].ToString());
            }
            reader.Close();
            comboBox1.Items.AddRange(forns.ToArray());
                fillDataGrid();
        }

        private void fillDataGrid()
        {
            dt = new DataTable();
            SqlCommand go = new SqlCommand("Select Loja.Loja_NIF, Endereco, ISNULL(q1.Quantidade,0) as Quantidade from MusicStore.Loja left outer join (select * from MusicStore.LojaContem where Produto_Codigo=@Codigo) q1 on Loja.Loja_NIF=q1.Loja_NIF order by q1.Quantidade DESC",cn2);
            go.Parameters.Clear();
            go.Parameters.AddWithValue("@Codigo",code2);
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = go;
            adapter.Fill(dt);
            bindingSource1.DataSource = dt;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void Restock(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1) {
                    MessageBox.Show("Please select a valid Supplier");
                    return;
                }

                if (numericUpDown1.Value==0) {
                    MessageBox.Show("You can't restock 0 products!");
                    return;
                }



                SqlCommand res = new SqlCommand("MusicStore.Restock",cn2);
                res.CommandType = CommandType.StoredProcedure;
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {

                    res.Parameters.Clear();

                    res.Parameters.AddWithValue("@Produto_Codigo", code2);
                    res.Parameters.AddWithValue("@Loja_NIF", int.Parse(row.Cells[0].Value.ToString()));
                    res.Parameters.AddWithValue("@Quantidade", (int)numericUpDown1.Value);

                    res.ExecuteNonQuery();
                }
                fillDataGrid();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
