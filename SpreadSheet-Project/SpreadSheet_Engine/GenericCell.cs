// <copyright file="GenericCell.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheet_Engine
{
    using System.ComponentModel;

    /// <summary>
    /// This is my Abstract Cell class containing basic data for
    /// a spreadsheet cell.
    /// </summary>
    public abstract class GenericCell : INotifyPropertyChanged
    {
        /// <summary>
        /// Used for strings to go in the cells.
        /// </summary>
        protected string cellText = string.Empty;

        /// <summary>
        /// stores what color cells background should be.
        /// </summary>
        protected uint bgColor = 0xFFFFFFFF;

        /// <summary>
        /// Used for string with '=' at the beginning.
        /// </summary>
        protected string cellValue = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCell"/> class.
        /// </summary>
        /// <param name="rowIndex">rowIndex.</param>
        /// <param name="columnIndex">columnIndex.</param>
        public GenericCell(int rowIndex, int columnIndex)
        {
           this.cellText = string.Empty;
           this.RowIndex = rowIndex;
           this.ColumnIndex = columnIndex;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets cells column number.
        /// </summary>
        public int ColumnIndex
        { get; }

        /// <summary>
        /// Gets cells row number.
        /// </summary>
        public int RowIndex
        { get; }

        /// <summary>
        /// Gets or sets bgColor.
        /// </summary>
        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged(nameof(this.BGColor));
                }
            }
        }

        /// <summary>
        /// Gets or sets cellTexts.
        /// </summary>
        public string CellText
        {
            get
            {
                return this.cellText;
            }

            set
            {
                if (this.cellText != value)
                {
                    this.cellText = value;
                    this.OnPropertyChanged(nameof(this.CellText));
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Gets cellValue.
        /// </summary>
        public string CellValue
        {
            get { return this.cellValue; }
        }

        /// <summary>
        /// Gets cellValue through property.
        /// </summary>
        /// <returns>this.cellValue.</returns>
        public string GetValue()
        {
            return this.cellValue;
       }

        /// <summary>
        /// Looks to see if the current cell has been modified from generic state.
        /// </summary>
        /// <returns> if the statement is true or false. </returns>
        public bool IsModified()
        {
            if (this.CellText != string.Empty || this.BGColor != 0xFFFFFFFF)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the name of the cell ex. A1, C25, F9.
        /// </summary>
        /// <returns> returns cellname ex. A1, D4.</returns>
        public string CellName()
        {
            char columnCell = (char)(65 + (this.ColumnIndex % 26));
            string cellName = columnCell.ToString() + this.RowIndex;
            if (cellName != null)
            {
                return cellName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Allows subscriber of property change in a null-safe way.
        /// </summary>
        /// <param name="propertyName">propertyName.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
           this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}