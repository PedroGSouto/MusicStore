using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace MusicStoreAlt
{
    public partial class Form1 : Form
    {
        static int currentDB = 0;
        private SqlConnection cn;
        private SqlDataAdapter adapter;
        private BindingSource bindingSource1 = new BindingSource();
        private BindingSource bindingSource2 = new BindingSource();
        private ArrayList changedRows = new ArrayList();
        DataTable dt;
        DataTable dt2;
        DataTable dt3;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cn = getConnection();
            EnableSubmenu(1);
            dataGridView1.DataSource = bindingSource1;
            dataGridView3.DataSource = bindingSource2;
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Context);
            this.dataGridView3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Context2);
            this.RestockRow.Click += new System.EventHandler(this.RestockRow_Click);
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
        }

        private SqlConnection getConnection()
        {
            return new SqlConnection("Data Source=LAPTOP-SQO3I51O;Initial Catalog=MusicStoreProject;Integrated Security=True");
        }

        private bool VerifyConnection()
        {
            if (cn == null)
                cn = getConnection();

            if (cn.State != ConnectionState.Open)
                cn.Open();

            return cn.State == ConnectionState.Open;
        }

        private void fillDataGrid() {

            if (!VerifyConnection()) {
                return;
            }
            changedRows.Clear();
            dt = new DataTable();
            switch (currentDB) {
                //Replace with stored Procedure and transaction
                case (1):
                    adapter = new SqlDataAdapter("Select Pes_NIF as NIF,Nome,Telefone,Endereco from MusicStore.Cliente join MusicStore.Pessoa on Cliente.Cli_NIF = Pessoa.Pes_NIF", cn);
                    SqlCommand com1 = new SqlCommand("UPDATE MusicStore.Pessoa SET Nome = @Nome, Endereco=@Endereco, Telefone=@Telefone where Pes_NIF=@NIF; ", cn);
                    adapter.UpdateCommand = com1;
                    adapter.Fill(dt);
                    break;
                case (2):
                    adapter = new SqlDataAdapter("Select Pes_NIF as NIF,Nome,Telefone,Endereco,Loja_NIF,Ordenado from MusicStore.Funcionario join MusicStore.Pessoa on Funcionario.Fun_NIF = Pessoa.Pes_NIF", cn);
                    SqlCommand com2 = new SqlCommand("MusicStore.UpdateFuncionarios", cn);
                    com2.CommandType = CommandType.StoredProcedure;
                    adapter.UpdateCommand = com2;
                    adapter.Fill(dt);
                    break;
                case (3):
                    adapter = new SqlDataAdapter("Select Codigo,Nome,Marca,Classe,Tipo_Som,Tipo_Voz,Preco as PrecoBase,Desconto,Preco * (1-Desconto) as PrecoAtual from MusicStore.Produto join MusicStore.Instrumento on Produto.Codigo = Instrumento.Prod_Cod join MusicStore.Prod_Preco on Produto.Codigo=Prod_Preco.Prod_Cod", cn);
                    SqlCommand com3 = new SqlCommand("MusicStore.UpdateInstrumentos", cn);
                    com3.CommandType = CommandType.StoredProcedure;
                    adapter.UpdateCommand = com3;
                    adapter.Fill(dt);
                    break;
                case (4):
                    adapter = new SqlDataAdapter("Select Codigo,Nome,Marca,Funcao, Preco as PrecoBase,Desconto,Preco * (1-Desconto) as PrecoAtual from MusicStore.Produto join MusicStore.Equipamento on Produto.Codigo = Equipamento.Prod_Cod join MusicStore.Prod_Preco on Produto.Codigo=Prod_Preco.Prod_Cod", cn);
                    SqlCommand com4 = new SqlCommand("MusicStore.UpdateEquipamentos", cn);
                    com4.CommandType = CommandType.StoredProcedure;
                    adapter.UpdateCommand = com4;
                    adapter.Fill(dt);
                    break;
                case (5):
                    adapter = new SqlDataAdapter("Select Codigo,Nome,Marca, Preco as PrecoBase,Desconto,Preco * (1-Desconto) as PrecoAtual from MusicStore.Produto join MusicStore.Software on Produto.Codigo = Software.Prod_Cod join MusicStore.Prod_Preco on Produto.Codigo=Prod_Preco.Prod_Cod", cn);
                    SqlCommand com5 = new SqlCommand("MusicStore.UpdateSoftware", cn);
                    com5.CommandType = CommandType.StoredProcedure;
                    adapter.UpdateCommand = com5;
                    adapter.Fill(dt);
                    break;
                case (6):
                    adapter = new SqlDataAdapter("Select Codigo,Nome,Marca, Preco as PrecoBase,Desconto,Preco * (1-Desconto) as PrecoAtual from MusicStore.Produto join MusicStore.Acessorios on Produto.Codigo = Acessorios.Prod_Cod  join MusicStore.Prod_Preco on Produto.Codigo=Prod_Preco.Prod_Cod", cn);
                    SqlCommand com6 = new SqlCommand("MusicStore.UpdateAcessorios", cn);
                    com6.CommandType = CommandType.StoredProcedure;
                    adapter.UpdateCommand = com6;
                    adapter.Fill(dt);
                    break;
                case (7):
                    adapter = new SqlDataAdapter("Select N_Compra, Pes_NIF as NIF,Pessoa.Nome, Online_Enc as Online, Data_Enc, Total from MusicStore.Compra join MusicStore.Pessoa on Cliente_NIF=Pes_NIF", cn);
                    adapter.Fill(dt);
                    break;
                case (8):
                    adapter = new SqlDataAdapter("Select * from MusicStore.Compra join MusicStore.Envolve on N_Compra=Comp_N_Compra join MusicStore.Produto on Produto_Codigo = Produto.Codigo order by Data_Enc", cn);
                    adapter.Fill(dt);
                    break;
            }
            bindingSource1.DataSource = dt;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            dataGridView1.ReadOnly = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void EnableSubmenu(int on) {

            if (on == 1)
            {
                Equipment_Button.Visible = true;
                Accessory_Button.Visible = true;
                Software_Button.Visible = true;
                Instrument_Button.Visible = true;
                dataGridView1.Enabled = false;

                New_Button.Enabled = false;
                Edit_Button.Enabled = false;
                Delete_Button.Enabled = false;
                Reload_Button.Enabled = false;

            }
            else {
                Equipment_Button.Visible = false;
                Accessory_Button.Visible = false;
                Software_Button.Visible = false;
                Instrument_Button.Visible = false;
                dataGridView1.Enabled = true;

                New_Button.Enabled = true;
                Edit_Button.Enabled = true;
                Delete_Button.Enabled = true;
                Reload_Button.Enabled = true;
            }
        }

        private void ButtonSwitch(int on) {

            if (on == 1)
            {
                New_Button.Visible = true;
                Edit_Button.Visible = true;
                Delete_Button.Visible = true;
                Reload_Button.Visible = true;
                Save_Button.Visible = false;
                Cancel_Button.Visible = false;
            }
            else {
                New_Button.Visible = false;
                Edit_Button.Visible = false;
                Delete_Button.Visible = false;
                Reload_Button.Visible = false;
                Save_Button.Visible = true;
                Cancel_Button.Visible = true;
            }
           
        }

        private void Produtos_Button_Click(object sender, EventArgs e)
        {
            EnableSubmenu(1);
            dataGridView1.ContextMenuStrip = RestockRow;
            panel4.Visible = false;
            dataGridView1.Visible = true;
            ButtonSwitch(1);
            chart1.Visible = false;
        }

        private void Clientes_Button_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;

            dataGridView1.ContextMenuStrip = new ContextMenuStrip();
            panel4.Visible = false;
            EnableSubmenu(0);
            label2.Text = "Clientes";
            ButtonSwitch(1);
            currentDB = 1;
            fillDataGrid();
            chart1.Visible = false;
            New_Button.Enabled = false;
            Edit_Button.Enabled = false;
            Delete_Button.Enabled = false;
            Reload_Button.Enabled = false;

        }

        private void Funcionarios_Button_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView1.ContextMenuStrip = new ContextMenuStrip();
            panel4.Visible = false;
            EnableSubmenu(0);
            label2.Text = "Funcionários";
            ButtonSwitch(1);
            currentDB = 2;
            fillDataGrid();
            chart1.Visible = false;
        }

        private void Edit_Button_Click(object sender, EventArgs e)
        {
            ButtonSwitch(0);
            dataGridView1.ReadOnly = false;
            switch (currentDB) {
                case (2):
                    dataGridView1.Columns[0].ReadOnly = true;
                    break;
                case (3):
                    dataGridView1.Columns[0].ReadOnly = true;
                    dataGridView1.Columns[8].ReadOnly = true;
                    break;
                case (4):
                    dataGridView1.Columns[0].ReadOnly = true;
                    dataGridView1.Columns[6].ReadOnly = true;
                    break;
                case (5):
                    dataGridView1.Columns[0].ReadOnly = true;
                    dataGridView1.Columns[5].ReadOnly = true;
                    break;
                case (6):
                    dataGridView1.Columns[0].ReadOnly = true;
                    dataGridView1.Columns[5].ReadOnly = true;
                    break;
            
            }
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            ButtonSwitch(1);

            try {
                switch (currentDB) {
                    case (1):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@NIF", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Endereco", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Telefone", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    case (2):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@NIF", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Telefone", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Endereco", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Ordenado", dataGridView1[5, j].Value);
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    case (3):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@Codigo", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Marca", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Classe", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Tipo_Som", dataGridView1[4, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Tipo_Voz", dataGridView1[5, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Preco_Base", dataGridView1[6, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Desconto", dataGridView1[7, j].Value);
                            
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    case (4):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@Codigo", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Marca", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Funcao", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Preco_Base", dataGridView1[4, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Desconto", dataGridView1[5, j].Value);
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    case (5):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@Codigo", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Marca", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Preco_Base", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Desconto", dataGridView1[4, j].Value);
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    case (6):
                        foreach (int j in changedRows)
                        {
                            adapter.UpdateCommand.Parameters.Clear();
                            adapter.UpdateCommand.Parameters.AddWithValue("@Codigo", dataGridView1[0, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Nome", dataGridView1[1, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Marca", dataGridView1[2, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Preco_Base", dataGridView1[3, j].Value);
                            adapter.UpdateCommand.Parameters.AddWithValue("@Desconto", dataGridView1[4, j].Value);
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        break;
                    default:
                        adapter.Update((DataTable)bindingSource1.DataSource);
                        break;
                }

                fillDataGrid();
                MessageBox.Show("Database Updated!");
            }
            catch(Exception ex) {
                fillDataGrid();
                MessageBox.Show(ex.ToString());
            }
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            ButtonSwitch(1);

            fillDataGrid();
        }

        private void Reload_Button_Click(object sender, EventArgs e)
        {
            fillDataGrid();
        }

        private void Delete_Button_Click(object sender, EventArgs e)
        {
            if (currentDB==1) {
                MessageBox.Show("You cannot delete clients!");
                return;
            }

            DialogResult result1 = MessageBox.Show("Quer mesmo apagar " + dataGridView1.SelectedRows.Count + " (" + label2.Text+")", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result1 == DialogResult.No) {
                return;
            }

            SqlCommand del;

            switch (currentDB) {

                case (2):
                   del = new SqlCommand("MusicStore.DeleteFuncionario", cn);
                   del.CommandType = CommandType.StoredProcedure;


                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        del.Parameters.Clear();
                        del.Parameters.AddWithValue("@Fun_NIF", int.Parse(row.Cells[0].Value.ToString()));
                        del.ExecuteNonQuery();
                    }
                    
                    break;
                case (3):
                    del = new SqlCommand("MusicStore.DeleteInstrumento", cn);
                    del.CommandType = CommandType.StoredProcedure;


                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        del.Parameters.Clear();
                        del.Parameters.AddWithValue("@Codigo", int.Parse(row.Cells[0].Value.ToString()));
                        del.ExecuteNonQuery();
                    }
                    break;
                case (4):
                    del = new SqlCommand("MusicStore.DeleteEquipamento", cn);
                    del.CommandType = CommandType.StoredProcedure;


                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        del.Parameters.Clear();
                        del.Parameters.AddWithValue("@Codigo", int.Parse(row.Cells[0].Value.ToString()));
                        del.ExecuteNonQuery();
                    }
                    break;
                case (5):
                    del = new SqlCommand("MusicStore.DeleteSoftware", cn);
                    del.CommandType = CommandType.StoredProcedure;


                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        del.Parameters.Clear();
                        del.Parameters.AddWithValue("@Codigo", int.Parse(row.Cells[0].Value.ToString()));
                        del.ExecuteNonQuery();
                    }
                    break;
                case (6):
                    del = new SqlCommand("MusicStore.DeleteAcessorio", cn);
                    del.CommandType = CommandType.StoredProcedure;


                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        del.Parameters.Clear();
                        del.Parameters.AddWithValue("@Codigo", int.Parse(row.Cells[0].Value.ToString()));
                        del.ExecuteNonQuery();
                    }
                    break;
                

            }
            fillDataGrid();


        }

        private void New_Button_Click(object sender, EventArgs e)
        {
            Form2 newform = new Form2(currentDB,cn);
            newform.ShowDialog();
            fillDataGrid();
        }

        private void Instrument_Button_Click(object sender, EventArgs e)
        {
            EnableSubmenu(0);
            label2.Text ="Instrumentos";
            currentDB = 3;
            fillDataGrid();
        }

        private void Equipment_Button_Click(object sender, EventArgs e)
        {   
            EnableSubmenu(0);
            label2.Text = "Equipamento";
            currentDB = 4;
            fillDataGrid();

        }

        private void Software_Button_Click(object sender, EventArgs e)
        {
            EnableSubmenu(0);
            label2.Text = "Software";
            currentDB = 5;
            fillDataGrid();

        }

        private void Accessory_Button_Click(object sender, EventArgs e)
        {
            EnableSubmenu(0);
            label2.Text = "Acessórios";
            currentDB = 6;
            fillDataGrid();

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changedRows.Add(e.RowIndex);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            
        }

        private void Encomendas_Button_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            dataGridView1.ContextMenuStrip = new ContextMenuStrip();
            panel4.Visible = false;
            EnableSubmenu(0);
            New_Button.Enabled = false;
            Edit_Button.Enabled = false;
            Delete_Button.Enabled = false;
            Reload_Button.Enabled = false;
            label2.Text = "Encomendas";
            currentDB = 7;
            chart1.Visible = false;
            fillDataGrid();
        }

        private void Vendas_Button_Click(object sender, EventArgs e)
        {
            EnableSubmenu(0);
            chart1.Visible = true; 
            panel4.Visible = false;
            New_Button.Enabled = false;
            Edit_Button.Enabled = false;
            Delete_Button.Enabled = false;
            Reload_Button.Enabled = false;
            dataGridView1.Visible = false;
            chart1.Visible = true;
            label2.Text = "Vendas";
            currentDB = 8;
            fillDataGrid();
            loadChart();
        }

        private void loadChart() {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            DataRow row1 = dt.Rows[0];
            DateTime date1 = (DateTime)row1[2];
            int fyear = date1.Year;
            int fmonth = date1.Month;
            int missedm;
            decimal lucro = 0;

            foreach (DataRow row in dt.Rows) {
                DateTime date = (DateTime)row[2];
                if (date.Month == fmonth && date.Year == fyear){
                    lucro += (decimal)row[4];
                }
                else {
                    if ((date.Month-fmonth==1 && date.Year == fyear) || (date.Month-fmonth==-11 && date.Year == fyear+1))
                    {
                        chart1.Series["Lucro"].Points.AddXY(fmonth+"/"+fyear%100,lucro);
                        lucro = 0;
                        fyear = date.Year;
                        fmonth = date.Month;
                        lucro += (decimal)row[4];
                    }
                    else {
                        chart1.Series["Lucro"].Points.AddXY(fmonth + "/" + fyear%100, lucro);
                        lucro = 0;

                        missedm = date.Month - fmonth + (date.Year - fyear) * 12;
                        for (int j = fmonth+1;j<fmonth+missedm;j++) {
                            chart1.Series["Lucro"].Points.AddXY(((j-1)%12)+1 + "/" + (fyear+j/12)%100, 0);
                        }
                        fyear = date.Year;
                        fmonth = date.Month;
                        lucro += (decimal)row[4];
                    }
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VerifyConnection();
            dt3 = new DataTable();
            dt3.Columns.Add("Codigo", typeof(String));
            dt3.Columns.Add("Nome", typeof(String));
            dt3.Columns.Add("Marca", typeof(String));
            dt3.Columns.Add("Quantidade", typeof(String));
            dt3.Columns.Add("Total", typeof(String));
            dataGridView3.Refresh();
            EnableSubmenu(0);
            dataGridView1.Visible = false;
            panel4.Visible = true;

            List<String> nif = new List<String>();
            SqlCommand GetClientNIF = new SqlCommand("Select Cli_NIF From MusicStore.Cliente", cn);
            SqlDataReader reader = GetClientNIF.ExecuteReader();
            
            while (reader.Read())
            {
                nif.Add(reader[0].ToString());
            }
            reader.Close();

            comboBox1.Items.Clear();
            List<String> nif2 = new List<String>();
            List<String> end = new List<String>();
            SqlCommand GetStoreNIF = new SqlCommand("Select Loja_NIF,Endereco From MusicStore.Loja", cn);
            reader = GetStoreNIF.ExecuteReader();
            while (reader.Read())
            {
                nif2.Add(reader[0].ToString());
                end.Add(reader[1].ToString());
            }

            for (int i = 0; i < nif2.Count; i++)
            {
                comboBox1.Items.Add(end[i] + ", " + nif2[i]);
            }
            reader.Close();

            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
            acsc.AddRange(nif.ToArray());
            textBox1.AutoCompleteCustomSource = acsc;
            textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox1.AutoCompleteCustomSource.Contains(textBox1.Text))
            {
                SqlCommand GetClient = new SqlCommand("Select Pes_NIF, Nome, Endereco, Telefone From MusicStore.Pessoa Where Pes_NIF=@NIF", cn);
                GetClient.Parameters.Clear();
                GetClient.Parameters.AddWithValue("@NIF", int.Parse(textBox1.Text));
                SqlDataReader reader = GetClient.ExecuteReader();
                while (reader.Read())
                {
                    textBox2.Text = reader[1].ToString();
                    textBox3.Text = reader[3].ToString();
                    textBox4.Text = reader[2].ToString();

                }
                reader.Close();
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                

            }
            else {
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox4.ReadOnly = false;
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.SelectedIndex = 1;
                comboBox1.SelectedIndex = -1;
                label9.Visible = false;
                comboBox1.Visible = false;
            }
            else {
                label9.Visible = true;
                comboBox1.Visible = true;
            }
        }

        private void Context(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                var hti = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void Context2(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = dataGridView3.HitTest(e.X, e.Y);
                dataGridView3.ClearSelection();
                dataGridView3.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void RestockRow_Click(object sender, EventArgs e)
        {
            Int32 rowToRestock = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            int code = int.Parse(dataGridView1.Rows[rowToRestock].Cells[0].Value.ToString());
            Form3 newform = new Form3("Produto " + dataGridView1.Rows[rowToRestock].Cells[0].Value.ToString() + ": " + dataGridView1.Rows[rowToRestock].Cells[1].Value.ToString() + " | Marca: " + dataGridView1.Rows[rowToRestock].Cells[2].Value.ToString(), cn, code);
            newform.ShowDialog();
            dataGridView1.ClearSelection();
        }

        private void Remove_Click(object sender, EventArgs e) {
            Int32 rowToRem = dataGridView3.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            label11.Text = (decimal.Parse(label11.Text) - decimal.Parse(dataGridView3.Rows[rowToRem].Cells[4].Value.ToString())).ToString();
            dt3.Rows.Remove(dt3.Rows[rowToRem]);
            dataGridView3.Refresh();
           
        }

        private bool isFilled() {

            foreach (TextBox tb in panel4.Controls.OfType<TextBox>()) {
                if (string.IsNullOrWhiteSpace(tb.Text)){
                    return false;
                }
            }
            if (numericUpDown1.Value == 0)
            {
                return false;
            }
            if (!checkBox1.Checked && comboBox1.SelectedIndex == -1) {
                return false;
            }
            return true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isFilled())
            {
                foreach (DataGridViewRow rowToDel in dataGridView3.Rows) {
                    if (rowToDel.Cells[0].Value.ToString().Equals(textBox5.Text)) {
                        MessageBox.Show("That product was already added!");
                        return;
                    }
                }



                /*********************************/

                DataRow row = dt3.NewRow();

                SqlCommand GetPInfo = new SqlCommand("Select Codigo, Nome, Marca, Preco, Desconto From (Select Codigo, Nome, Marca From MusicStore.Produto where Codigo=@Codigo) p1 join MusicStore.Prod_Preco on p1.Codigo=Prod_Cod", cn);
                GetPInfo.Parameters.Clear();
                GetPInfo.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                SqlDataReader reader = GetPInfo.ExecuteReader();

                while (reader.Read()) {

                    row["Codigo"] = reader[0].ToString();
                    row["Nome"] = reader[1].ToString();
                    row["Marca"] = reader[2].ToString();
                    row["Quantidade"] = numericUpDown1.Value;
                    row["Total"] = (decimal)reader[3] * (decimal)reader[4] * numericUpDown1.Value;
                    label11.Text = (decimal.Parse(label11.Text) + decimal.Parse(row["Total"].ToString())).ToString();
                }

                reader.Close();

                 

                dt3.Rows.Add(row);



                /*********************************/

                bindingSource2.DataSource = dt3;
                dataGridView3.DataSource = bindingSource2;
                for (int i = 0; i < dataGridView3.Columns.Count; i++)
                {
                    dataGridView3.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            }
            else {

                MessageBox.Show("Please fill all the info");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt3.Clear();
            label11.Text = "0";

            if (!string.IsNullOrWhiteSpace(textBox5.Text) && comboBox1.SelectedIndex != -1) {
                SqlCommand sumqt = new SqlCommand("Select Quantidade From MusicStore.LojaContem Where Produto_Codigo=@Codigo And Loja_NIF=@Loja_NIF", cn);
                sumqt.Parameters.Clear();
                sumqt.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                sumqt.Parameters.AddWithValue("@Loja_NIF", int.Parse(comboBox1.Text.Substring(comboBox1.Text.Length - 9)));
                int q;
                if (sumqt.ExecuteScalar() == null)
                {
                    q = 0;
                }
                else
                {
                    q = (int)sumqt.ExecuteScalar();
                }
                numericUpDown1.Maximum = q;
            }
            else if (checkBox1.Checked && !string.IsNullOrWhiteSpace(textBox5.Text)) {
                SqlCommand sumqt = new SqlCommand("Select Sum(Quantidade) From MusicStore.LojaContem Where Produto_Codigo=@Codigo", cn);
                sumqt.Parameters.Clear();
                sumqt.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                int q;
                if (sumqt.ExecuteScalar() == null)
                {
                    q = 0;
                }
                else
                {
                    q = (int)sumqt.ExecuteScalar();
                }
                numericUpDown1.Maximum = q;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (isValid())
            {
                int lojaNif;
                int NC;

                SqlCommand maxNC = new SqlCommand("SELECT MAX(N_Compra) FROM MusicStore.Compra", cn);
                NC = (int)maxNC.ExecuteScalar() + 1;

                if (checkBox1.Checked)
                {
                    lojaNif = -1;
                }
                else {
                    lojaNif = int.Parse(comboBox1.Text.Substring(comboBox1.Text.Length - 9));
                }

                SqlCommand finalize = new SqlCommand("MusicStore.FinalizeBuy", cn);
                finalize.CommandType = CommandType.StoredProcedure;
                foreach (DataGridViewRow row in dataGridView3.Rows)
                {

                    finalize.Parameters.Clear();
                    finalize.Parameters.AddWithValue("@Pes_NIF", int.Parse(textBox1.Text));
                    finalize.Parameters.AddWithValue("@Nome", textBox2.Text);
                    finalize.Parameters.AddWithValue("Telefone", int.Parse(textBox3.Text));
                    finalize.Parameters.AddWithValue("@Endereco", textBox4.Text);
                    finalize.Parameters.AddWithValue("@Codigo", int.Parse(row.Cells[0].Value.ToString()));
                    finalize.Parameters.AddWithValue("Quantidade", int.Parse(row.Cells[3].Value.ToString()));
                    finalize.Parameters.AddWithValue("@Total", decimal.Parse(row.Cells[4].Value.ToString()));
                    finalize.Parameters.AddWithValue("@Loja_NIF", lojaNif);
                    finalize.Parameters.AddWithValue("@NC", NC);
                    finalize.ExecuteNonQuery();
                }

                MessageBox.Show("Purchase Successful!");
                textBox1.Text="";
                textBox2.Text="";
                textBox3.Text="";
                textBox4.Text="";
                textBox5.Text="";
                numericUpDown1.Value = 0;
                label11.Text = "0";
                comboBox1.SelectedIndex = -1;
                checkBox1.Checked = false;
                
                dt3.Clear();
                dataGridView3.Refresh();
                //Application.Restart();

            }
            else {
                MessageBox.Show("Ups! Something Went Wrong!\nPlease verify if your data is valid!");
            }

            
        }

        private bool isValid() {

            try
            {
                if (!(int.Parse(textBox1.Text) > 100000000 && int.Parse(textBox1.Text) < 1000000000))
                {
                    return false;
                }
                if (!(int.Parse(textBox3.Text) > 100000000 && int.Parse(textBox3.Text) < 1000000000))
                {
                    return false;
                }
                if (dataGridView3.Rows.Count == 0) {
                    return false;
                }

                int.Parse(textBox5.Text);
                return true;
                
            }
            catch (Exception ex) {
                return false;
            }

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!textBox5.Text.Equals(""))
            {
                SqlCommand validCodes = new SqlCommand("Select Count(Codigo) From MusicStore.Produto Where Codigo=@Codigo", cn);
                validCodes.Parameters.Clear();
                validCodes.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                if ((int)validCodes.ExecuteScalar() == 0)
                {
                    MessageBox.Show("Invalid Code!");
                    textBox5.Clear();
                    numericUpDown1.Value = 0;
                }
                else {
                    if (comboBox1.SelectedIndex != -1)
                    {
                        SqlCommand sumqt = new SqlCommand("Select Quantidade From MusicStore.LojaContem Where Produto_Codigo=@Codigo And Loja_NIF=@Loja_NIF", cn);
                        sumqt.Parameters.Clear();
                        sumqt.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                        sumqt.Parameters.AddWithValue("@Loja_NIF", int.Parse(comboBox1.Text.Substring(comboBox1.Text.Length - 9)));
                        int q;
                        if (sumqt.ExecuteScalar() == null)
                        {
                            q = 0;
                        }
                        else
                        {
                            q = (int)sumqt.ExecuteScalar();
                        }
                        numericUpDown1.Maximum = q;
                    }
                    else if (checkBox1.Checked)
                    {
                        SqlCommand sumqt = new SqlCommand("Select Sum(Quantidade) From MusicStore.LojaContem Where Produto_Codigo=@Codigo", cn);
                        sumqt.Parameters.Clear();
                        sumqt.Parameters.AddWithValue("@Codigo", int.Parse(textBox5.Text));
                        int q;
                        if (sumqt.ExecuteScalar() == null)
                        {
                            q = 0;
                        }
                        else
                        {
                            q = (int)sumqt.ExecuteScalar();
                        }
                        numericUpDown1.Maximum = q;

                    }
                }
            }
            

        }
    }
}
