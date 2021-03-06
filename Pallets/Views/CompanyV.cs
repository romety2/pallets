﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Pallets.Controllers;
using Pallets.Models;

namespace Pallets.Views
{
    public partial class CompanyV : Form
    {
        private App app;
        private DataC dc;
        private Data data;
        private CompanyMV fmv;
        public bool saved;

        public CompanyV()
        {
            dc = new DataC();
            InitializeComponent();
            saved = true;
        }

        public CompanyV(Data data)
        {
            dc = new DataC(data.Events);
            InitializeComponent();
            saved = true;
        }

        public CompanyV(App app)
        {
            dc = new DataC();
            this.app = app;
            app.Enabled = false;
            InitializeComponent();
            saved = true;
        }

        public CompanyV(Data data, App app)
        {
            dc = new DataC(data.Events);
            this.app = app;
            app.Enabled = false;
            InitializeComponent();
            saved = true;
        }

        private BindingList<Company> getCompanies()
        {
            data = dc.getData();
            return data.Companies;
        }

        private void getView()
        {
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Firma";
            dataGridView1.Columns[1].Width = 250;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
        }


        private void checkViewActualCompany()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                label3.Text = "";
                button6.Enabled = false;
            }
            else
                button6.Enabled = true;
        }

        public void addData(string name, string description)
        {
            int id;
            if (dataGridView1.Rows.Count != 0 && String.Compare(name, dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value.ToString(), true) < 0)
                id = dataGridView1.CurrentCellAddress.Y + 1;
            else
                id = dataGridView1.CurrentCellAddress.Y;
            label4.Text = "Dodano firmę " + name + "!";
            saved = false;
            BindingSource companies = new BindingSource();
            companies.DataSource = dc.addCompany(name, description).OrderBy(o => o.Name);
            dataGridView1.DataSource = companies;
            if (dataGridView1.Rows.Count != 1 && textBox1.Text == "")
                dataGridView1.CurrentCell = dataGridView1[1, id];
            else
                textBox1.Text = "";
            checkViewActualCompany();
        }

        public void editData(string name, string description)
        {
            label4.Text = "Zmieniono dane firmy " + name + "!";
            label3.Text = name;
            saved = false;
            dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value = name;
            dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[2].Value = description;
            dc.editEventCompany((ulong)dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value, name);
            checkViewActualCompany();
        }

        public void deleteSmallWindow()
        {
            fmv = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fmv == null)
            {
                fmv = new CompanyMV(this);
                fmv.Text = "Dodaj firmę";
                fmv.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (fmv == null && dataGridView1.Rows.Count != 0)
            {
                fmv = new CompanyMV(this, dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value.ToString(), dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[2].Value.ToString());
                fmv.Text = "Zmień nazwę firmy";
                fmv.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (fmv == null && dataGridView1.Rows.Count != 0)
            {
                DialogResult dr = MessageBox.Show("Na pewno usunąć " + dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value + "?", "Pytanie", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    label4.Text = "Usunięto firmę " + dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value + "!";
                    saved = false;
                    dc.deleteCompany((ulong)dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value);
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentCellAddress.Y);
                    checkViewActualCompany();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label4.Text = "Zapisano zmiany!";
            saved = true;
            dc.saveData();
            app.refreshData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CompanyV_Load(object sender, EventArgs e)
        {
            BindingSource companies = new BindingSource();
            companies.DataSource = getCompanies().OrderBy(o => o.Name).ToList();
            dataGridView1.DataSource = companies;
            getView();
            checkViewActualCompany();
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows.Count != 0)
                label3.Text = dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value.ToString();
            dataGridView1.CurrentRow.Selected = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = getCompanies()
                .Where(o => o.Name.ToLower().Contains(textBox1.Text.ToLower()) == true)
                .OrderBy(o => o.Name).ToList();
            dataGridView1.DataSource = bs;
            checkViewActualCompany();
        }

        private void CompanyV_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                DialogResult dr = MessageBox.Show("Zapisać zmiany?", "Pytanie", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel)
                    e.Cancel = true;
                else
                {
                    if (dr == DialogResult.Yes)
                    {
                        saved = true;
                        dc.saveData();
                    }
                    app.Enabled = true;
                }
            }
            else
                app.Enabled = true;
            app.refreshData();
            app.refreshDataGridView();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CompanyVV name = new CompanyVV((ulong)dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value, data, this);
            name.Show();
        }
    }
}
