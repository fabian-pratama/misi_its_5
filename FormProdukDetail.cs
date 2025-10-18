using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManajemenToko
{
    public partial class FormProdukDetail : Form
    {

        public int? ProdukId { get; set; } = null;
        public FormProdukDetail()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadDataProduk()
        {
            if (ProdukId == null) return;
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT NamaProduk, Harga, Stok, KategoriId, Deskripsi FROM Produk WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", ProdukId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNamaProduk.Text = reader["NamaProduk"].ToString();
                        txtHarga.Text = reader["Harga"].ToString();
                        txtStok.Text = reader["Stok"].ToString();
                        cmbKategori.SelectedValue = Convert.ToInt32(reader["KategoriId"]);
                        txtDeskripsi.Text = reader["Deskripsi"].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data produk: " + ex.Message);
                }
            }
        }

        private void FormProdukDetail_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Id, NamaKategori FROM Kategori";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> kategoriDict = new Dictionary<int,
                    string>();

                    while (reader.Read())
                    {
                        kategoriDict.Add((int)reader["Id"],
                        reader["NamaKategori"].ToString());
                    }

                    cmbKategori.DataSource = new BindingSource(kategoriDict, null);
                    cmbKategori.DisplayMember = "Value";
                    cmbKategori.ValueMember = "Key";
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat kategori: " + ex.Message);
                }
            }

            if (ProdukId.HasValue)
            {
                LoadDataProduk();
                this.Text = "Edit Produk";
            }
            else
            {
                this.Text = "Tambah Produk";
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool valid = true;
            if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
            {
                errorProvider1.SetError(txtNamaProduk, "Nama produk tidak boleh kosong.");
                valid = false;
            }
            if (!decimal.TryParse(txtHarga.Text, out decimal harga) || harga < 0)
            {
                errorProvider1.SetError(txtHarga, "Harga harus berupa angka positif.");
                valid = false;
            }
            if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
            {
                errorProvider1.SetError(txtStok, "Stok harus berupa angka ≥ 0.");
                valid = false;
            }
            if (!valid) return;

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query;
                    if (ProdukId.HasValue)
                    {
                        // UPDATE
                        query = @"UPDATE Produk SET NamaProduk = @nama, Harga = @harga, Stok = @stok, KategoriId = @kategori, Deskripsi = @deskripsi WHERE Id = @id";
                    }
                    else
                    {
                        query = @"INSERT INTO Produk (NamaProduk, Harga, Stok, KategoriId, Deskripsi) VALUES (@nama, @harga, @stok, @kategori, @deskripsi)";
                    }
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                    cmd.Parameters.AddWithValue("@harga", Convert.ToDecimal(txtHarga.Text));
                    cmd.Parameters.AddWithValue("@stok", Convert.ToInt32(txtStok.Text));
                    cmd.Parameters.AddWithValue("@kategori", ((KeyValuePair<int, string>)cmbKategori.SelectedItem).Key);
                    cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                    if (ProdukId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", ProdukId);
                    }

                    if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
                    {
                        MessageBox.Show("Nama produk tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Produk berhasil ditambahkan!");
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menambahkan produk: " + ex.Message);
                }
            }

            
        }

        private void txtHarga_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHarga.Text) && !int.TryParse(txtHarga.Text, out _))
            {
                MessageBox.Show("Harga tidak valid, tolong isi angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHarga.Clear();
            }
        }

        private void txtStok_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStok.Text) && !int.TryParse(txtStok.Text, out _))
            {
                MessageBox.Show("Stok tidak valid, tolong isi angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStok.Clear();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }
    }

}
        
