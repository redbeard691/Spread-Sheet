// <copyright file="Form1.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheet_Nickolas_Sturgeon
{
    using System.ComponentModel;
    using System.Windows.Forms;
    using SpreadSheet_Engine;

    /// <summary>
    /// Creates a spreadsheet windows and uses a demo.
    /// </summary>
    public partial class Form1 : Form
    {
        private MYSpreadSheet mySpreadSheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Starts up.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.SetupGridView();
            this.mySpreadSheet = new MYSpreadSheet(50, 26);
            this.UpdateEditMenuStrip();
            this.mySpreadSheet.CellPropertyChanged += this.OnHandleCellPropertyChanged;
        }

        private void OnHandleCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            GenericCell? refcell = sender as GenericCell;

            if (refcell != null)
            {
                if (e.PropertyName == "CellValue" || e.PropertyName == "CellText")
                {
                    this.dataGridView.Rows[refcell.RowIndex].Cells[refcell.ColumnIndex].Value = refcell.CellValue;
                }

                if (e.PropertyName == "BGColor")
                {
                    this.dataGridView.Rows[refcell.RowIndex].Cells[refcell.ColumnIndex].Style.BackColor = Color.FromArgb((int)refcell.BGColor);
                }
            }

            this.UpdateEditMenuStrip();
        }

        private void UpdateEditMenuStrip()
        {
            this.UndoToolStripMenuItem.Enabled = this.mySpreadSheet.CheckUndo();
            this.RedoToolStripMenuItem.Enabled = this.mySpreadSheet.CheckRedo();
            this.UndoToolStripMenuItem.Text = "Undo";
            this.RedoToolStripMenuItem.Text = "Redo";

            if (this.UndoToolStripMenuItem.Enabled)
            {
                this.UndoToolStripMenuItem.Text += $" {this.mySpreadSheet.PeekUndoCommand?.Description}";
            }

            if (this.RedoToolStripMenuItem.Enabled)
            {
                this.RedoToolStripMenuItem.Text += $" {this.mySpreadSheet.PeekRedoCommand?.Description}";
            }
        }

        private void SetupGridView()
        {
            for (char character = 'A'; character <= 'Z'; character++)
            {
                this.dataGridView.Columns.Add(new DataGridViewColumn
                {
                    Name = character.ToString(),
                    HeaderText = character.ToString(),
                    CellTemplate = new DataGridViewTextBoxCell(),
                });
            }

            for (int i = 0; i < 50; i++)
            {
                int rowIndex = this.dataGridView.Rows.Add();
                this.dataGridView.Rows[rowIndex].HeaderCell.Value = (i + 1).ToString();
            }

            this.dataGridView.RowHeadersWidth = 50;
        }

        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var cellValue = this.mySpreadSheet.GetCell(e.RowIndex, e.ColumnIndex)?.CellText;
            this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cellValue;
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var newCellText = this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            var cell = this.mySpreadSheet.GetCell(e.RowIndex, e.ColumnIndex);
            if (cell != null)
            {
                if (newCellText != null)
                {
                    if (cell.CellText != newCellText.ToString())
                    {
                        this.mySpreadSheet.ChangeCellText(cell.RowIndex, cell.ColumnIndex, newCellText.ToString());
                        this.UpdateEditMenuStrip();
                        this.mySpreadSheet.ChangeCell(e.RowIndex, e.ColumnIndex, newCellText.ToString());
                    }
                    else
                    {
                        this.dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.mySpreadSheet.GetCell(e.RowIndex, e.ColumnIndex)?.CellValue;
                    }
                }
            }
        }

        private void ChangeCellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    uint color = (uint)colorDialog.Color.ToArgb();
                    foreach (DataGridViewCell selectedCell in this.dataGridView.SelectedCells)
                    {
                        var cell = this.mySpreadSheet.GetCell(selectedCell.RowIndex, selectedCell.ColumnIndex) as GenericCell;
                        if (cell != null)
                        {
                            this.mySpreadSheet.ChangeCellColor(cell.RowIndex, cell.ColumnIndex, color);
                            cell.BGColor = color;
                        }
                    }
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "XML (*.xml)|*.xml",
                DefaultExt = "xml",
                AddExtension = true,
                CheckPathExists = true,
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                FileStream streamFile = new FileStream(saveFile.FileName, FileMode.Create);
                this.mySpreadSheet.Save(streamFile);
                streamFile.Close();
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "XML (*.xml)|*.xml",
                DefaultExt = "xml",
                AddExtension = true,
                CheckPathExists = true,
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                FileStream streamFile = new FileStream(openFile.FileName, FileMode.Open);
                this.mySpreadSheet.Load(streamFile);

                streamFile.Close();
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mySpreadSheet.Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mySpreadSheet.Redo();
        }

        private void DemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            for (int i = 0; i < 50; i++)
            {
                int randRow = random.Next(1, 50);
                int randColumn = random.Next(1, 26);

                this.mySpreadSheet.ChangeCell(randRow, randColumn, "Hello");
                this.dataGridView.Rows[randRow].Cells[randColumn].Value = this.mySpreadSheet.GetCell(randRow, randColumn)?.CellText;
            }

            for (int j = 0; j < 50; j++)
            {
                this.mySpreadSheet.ChangeCell(j, 1, "This is B" + (j + 1));
                this.dataGridView.Rows[j].Cells[1].Value = this.mySpreadSheet.GetCell(j, 1)?.CellText;
            }

            for (int j = 0; j < 50; j++)
            {
                this.mySpreadSheet.ChangeCell(j, 0, "=B" + (j + 1));
                this.dataGridView.Rows[j].Cells[0].Value = this.mySpreadSheet.GetCell(j, 0)?.CellText;
            }
        }
    }
}