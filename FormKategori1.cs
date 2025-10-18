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
    public partial class FormKategori1 : Form
    {
        public FormKategori1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoadDataKategori1()
        {
            dgvKategori.Rows.Clear();
            dgvKategori.Columns.Clear();

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT 
                k.Id, 
                k.NamaKategori, 
                COUNT(p.Id) AS JumlahProduk
            FROM Kategori k
            LEFT JOIN Produk p ON k.Id = p.KategoriId
            GROUP BY k.Id, k.NamaKategori";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                dgvKategori.Columns.Add("Id", "ID");
                dgvKategori.Columns.Add("NamaKategori", "Nama Kategori");
                dgvKategori.Columns.Add("JumlahProduk", "Jumlah Produk"); 

                while (reader.Read())
                {
                    dgvKategori.Rows.Add(
                        reader["Id"],
                        reader["NamaKategori"],
                        reader["JumlahProduk"]
                    );
                }
                reader.Close();
            }
        }

        private void FormKategori1_Load(object sender, EventArgs e)
        {
            LoadDataKategori1();
            dgvKategori.ClearSelection();
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            string nama = txtNamaKategori.Text.Trim();

            if (string.IsNullOrWhiteSpace(nama))
            {
                MessageBox.Show("Nama kategori tidak boleh kosong.");
                return;
            }

            if (nama.Length < 3)
            {
                MessageBox.Show("Nama kategori minimal 3 karakter!");
                txtNamaKategori.Focus();
                return;
            }

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Kategori (NamaKategori) VALUES (@nama)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama", txtNamaKategori.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kategori berhasil ditambahkan!");
                txtNamaKategori.Clear();
                LoadDataKategori1();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvKategori.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih kategori terlebih dahulu.");
                return;
            }
            int id = Convert.ToInt32(dgvKategori.SelectedRows[0].Cells["Id"].Value);
            string nama = txtNamaKategori.Text.Trim();
            if (string.IsNullOrWhiteSpace(nama))
            {
                MessageBox.Show("Nama kategori tidak boleh kosong.");
                return;
            }
            if (nama.Length < 3)
            {
                MessageBox.Show("Nama kategori minimal 3 karakter!");
                txtNamaKategori.Focus();
                return;
            }

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Kategori SET NamaKategori = @nama WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kategori berhasil diubah.");
                txtNamaKategori.Clear();
                LoadDataKategori1();
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dgvKategori.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih kategori yang ingin dihapus.");
                return;
            }

            int id = Convert.ToInt32(dgvKategori.SelectedRows[0].Cells["Id"].Value);
            DialogResult confirm = MessageBox.Show(
            "Yakin ingin menghapus kategori ini?",
            "Konfirmasi",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection conn = Koneksi.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Kategori WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kategori berhasil dihapus.");
                    LoadDataKategori1();
                }
            }
        }

        private void dgvKategori_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKategori.SelectedRows.Count == 0) return;

            int kategoriId = Convert.ToInt32(dgvKategori.SelectedRows[0].Cells["Id"].Value);
            txtNamaKategori.Text = dgvKategori.SelectedRows[0].Cells["NamaKategori"].Value.ToString();

            dgvProdukTerkait.Rows.Clear();
            dgvProdukTerkait.Columns.Clear();

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                conn.Open();
                string query = "SELECT NamaProduk, Harga, Stok FROM Produk WHERE KategoriId = @kategoriId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kategoriId", kategoriId);
                SqlDataReader reader = cmd.ExecuteReader();
                dgvProdukTerkait.Columns.Add("NamaProduk", "Nama Produk");
                dgvProdukTerkait.Columns.Add("Harga", "Harga");
                dgvProdukTerkait.Columns.Add("Stok", "Stok");
                while (reader.Read())
                {
                    dgvProdukTerkait.Rows.Add(
                    reader["NamaProduk"],
                    reader["Harga"],
                    reader["Stok"]
                    );
                }
                reader.Close();
            }
        }
    }
}
