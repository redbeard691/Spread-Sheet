// <copyright file="MYSpreadSheet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheet_Engine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Linq;
    using ExpressionTree;
    using SpreadSheet_Engine.CommandInterface;

    /// <summary>
    /// Logic to interact with the UI and abstract cell class.
    /// </summary>
    public class MYSpreadSheet
    {
        /// <summary>
        // A dictionary to keep track of dependencies
        // The key is the cell that is being observed, and the value is a list of cells that depend on the key cell.
        // </summary>
        public Dictionary<GenericCell, List<GenericCell>> Dependencies;

        private readonly SpreadSheeT_Cell[,] cellArray;
        private readonly int columnCount = 0;
        private readonly int rowCount = 0;

        private Stack<ISpreadSheetCommand> undoStack = new Stack<ISpreadSheetCommand>();
        private Stack<ISpreadSheetCommand> redoStack = new Stack<ISpreadSheetCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MYSpreadSheet"/> class.
        /// </summary>
        /// <param name="rowIndex">rowIndex.</param>
        /// <param name="columnIndex">columnIndex.</param>
        public MYSpreadSheet(int rowIndex, int columnIndex)
        {
            this.cellArray = new SpreadSheeT_Cell[rowIndex, columnIndex];

            // Initialize the dependencies dictionary
            this.Dependencies = new Dictionary<GenericCell, List<GenericCell>>();
            this.undoStack = new Stack<ISpreadSheetCommand>();
            this.redoStack = new Stack<ISpreadSheetCommand>();

            for (int i = 0; i < rowIndex; i++)
            {
                for (int j = 0; j < columnIndex; j++)
                {
                    this.cellArray[i, j] = new SpreadSheeT_Cell(i, j);
                    this.cellArray[i, j].PropertyChanged += this.OnCellPropertyChanged;
                }
            }

            this.columnCount = columnIndex;
            this.rowCount = rowIndex;
        }

        /// <summary>
        /// Lets subscriber of cell property being changed.
        /// </summary>
        public event PropertyChangedEventHandler CellPropertyChanged;

        /// <summary>
        /// Gets if the Redo Command has anything in stack.
        /// </summary>
        public ISpreadSheetCommand? PeekRedoCommand
        {
            get
            {
                return this.redoStack.Count > 0 ? this.redoStack.Peek() : null;
            }
        }

        /// <summary>
        /// Gets if the Undo stack has anything in it.
        /// </summary>
        public ISpreadSheetCommand? PeekUndoCommand
        {
            get
            {
                return this.undoStack.Count > 0 ? this.undoStack.Peek() : null;
            }
        }

        /// <summary>
        /// Gets number of columns in spreadsheet.
        /// </summary>
        public int ColumnCount
        {
            get { return this.columnCount; }
        }

        /// <summary>
        /// Gets number of rows in spreadsheet.
        /// </summary>
        public int RowCount
        {
            get { return this.rowCount; }
        }

        /// <summary>
        /// Adds command to Undo stack.
        /// </summary>
        /// <param name="command">has a command passed through.</param>
        public void AddUndo(ISpreadSheetCommand command)
        {
            this.undoStack.Push(command);
        }

        /// <summary>
        /// Will undo last command on spreadsheet.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count > 0)
            {
                var command = this.undoStack.Pop();
                command.UnExecute();
                this.redoStack.Push(command);
            }
        }

        /// <summary>
        /// Will redo the last command given on spreadsheet.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count > 0)
            {
                var command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
            }
        }

        /// <summary>
        /// Looks to see if there is anything on undo stack.
        /// </summary>
        /// <returns>Returns bool value.</returns>
        public bool CheckUndo()
        {
            if (this.undoStack.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Looks to see if there is anything on redo stack.
        /// </summary>
        /// <returns>returns bool.</returns>
        public bool CheckRedo()
        {
            if (this.redoStack.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set up for later code just return cell value.
        /// </summary>
        /// <param name="sender">send.</param>
        /// <param name="e">e.</param>
        ///
        public void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CellText")
            {
                if (sender is SpreadSheeT_Cell cell)
                {
                    this.RemoveAllDependencies(cell);
                    this.EvaluateCellFormula(cell.RowIndex, cell.ColumnIndex);

                    // If cell is circular ref or self ref then last two are skipped to prevent crashes.
                    if (cell.CellValue != "CIRCULAR_REF!!" && cell.CellValue != "Self_Reference!")
                    {
                        this.UpdateCellDependencies(cell);
                        this.ReEvaluateDepends(cell);
                    }
                }
            }

            this.CellPropertyChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Finds the desired cell and changes the cellText.
        /// </summary>
        /// <param name="row">row.</param>
        /// <param name="col">col.</param>
        /// <param name="text">text.</param>
        public void ChangeCell(int row, int col, string text)
        {
            var cell = this.cellArray[row, col];
            string originalText = cell.CellText;

            // Update the cell text
            cell.CellText = text;
        }

        /// <summary>
        /// adds to the undo stack the cells old text string before change.
        /// </summary>
        /// <param name="row">row number of cell.</param>
        /// <param name="col">column value of cell.</param>
        /// <param name="newText">new string being passed into cell.</param>
        public void ChangeCellText(int row, int col, string newText)
        {
            var cell = this.GetCell(row, col);
            if (cell != null)
            {
                var command = new ChangeTextCommand(cell, newText);
                command.Execute();  // Apply the change
                this.AddUndo(command);  // Add the command to the undo stack
            }
        }

        /// <summary>
        /// adds to the undo stack the cell old color before change.
        /// </summary>
        /// <param name="row">row number of cell.</param>
        /// <param name="col">column value of cell.</param>
        /// <param name="newColor">new background color of cell.</param>
        public void ChangeCellColor(int row, int col, uint newColor)
        {
            var cell = this.GetCell(row, col);
            if (cell != null)
            {
                var command = new ChangeColorCommands(cell, newColor);
                command.Execute();  // Apply the change
                this.AddUndo(command);  // Add the command to the undo stack
            }
        }

        /// <summary>
        /// Returns cell.
        /// </summary>
        /// <param name="row">row. </param>
        /// <param name="col">col. </param>
        /// <returns>this.cellArray or null.</returns>
        public GenericCell? GetCell(int row, int col)
        {
            if (row < 0 || col < 0) // need to look at col since its letters.
            {
                return null;
            }

            if (row >= 0 && row < this.RowCount && col >= 0 && col < this.ColumnCount)
            {
                return this.cellArray[row, col];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Goes through the spread sheet and saves all modified cell and save data to XML file.
        /// </summary>
        /// <param name="stream"> file location get passed through.</param>
        public void Save(Stream stream)
        {
            XElement xmlRoot = new XElement("spreadsheet");
            foreach (GenericCell cell in this.cellArray)
            {
                if (cell.IsModified() && cell != null)
                {
                    XElement xmlCell = new XElement("cell");
                    xmlCell.SetAttributeValue("name", cell.CellName());

                    XElement xmlText = new XElement("text", cell.CellText);
                    xmlCell.Add(xmlText);

                    XElement xmlColor = new XElement("bgcolor", cell.BGColor);
                    xmlCell.Add(xmlColor);

                    xmlRoot.Add(xmlCell);
                }
            }

            XDocument xmlDoc = new XDocument(xmlRoot);
            xmlDoc.Save(stream);
        }

        /// <summary>
        /// Loads XML data to spreadsheet logic.
        /// </summary>
        /// <param name="filePath"> File gets passed in.</param>
        public void Load(Stream filePath)
        {
            if (!filePath.CanRead)
            {
                return;
            }

            XDocument doc = XDocument.Load(filePath);

            this.ClearSpreadSheetData(); // Implement this method to clear current data

            foreach (XElement cellElement in doc.Descendants("cell"))
            {
                string? cellName = cellElement.Attribute("name")?.Value;
                string? bgColor = cellElement.Element("bgcolor")?.Value;
                string? text = cellElement.Element("text")?.Value;

                if (cellName != null && text != null)
                {
                    int columnCell = cellName[0] - 'A';
                    int rowCell = int.Parse(cellName.Substring(1));

                    GenericCell? cell = this.GetCell(rowCell, columnCell);
                    cell.CellText = text;
                    cell.BGColor = Convert.ToUInt32(bgColor);
                }
            }

            this.undoStack = new Stack<ISpreadSheetCommand>();
            this.redoStack = new Stack<ISpreadSheetCommand>();
        }

        /// <summary>
        /// Goes through the entire Spread Sheet and removes all data.
        /// </summary>
        public void ClearSpreadSheetData()
        {
            // Clear dependencies from dictionary
            this.Dependencies.Clear();

            // clear undo/redo stacks as well
            this.undoStack.Clear();
            this.redoStack.Clear();

            foreach (SpreadSheeT_Cell cell in this.cellArray)
            {
                if (cell.IsModified() && cell != null)
                {
                    // Resetting cell properties to default values
                    cell.CellText = string.Empty;
                    cell.BGColor = 0xFFFFFFFF;
                }
            }
        }

        /// <summary>
        /// Goes through every key in dictionary updating the cells that where effected by cell change.
        /// </summary>
        /// <param name="cell">passes in the cell that was change. to see which cells depended on that cell.</param>
        private void ReEvaluateDepends(GenericCell cell)
        {
            if (cell != null)
            {
                Queue<GenericCell>? dependentList = this.GetDependencyChain(cell);
                if (dependentList != null)
                {
                    while (dependentList.Count > 0)
                    {
                        GenericCell cellToUpdate = dependentList.Dequeue();

                        this.EvaluateCellFormula(cellToUpdate.RowIndex, cellToUpdate.ColumnIndex);
                        this.CellPropertyChanged?.Invoke(cellToUpdate, new PropertyChangedEventArgs("CellValue"));
                    }
                }
            }
        }

        /// <summary>
        /// Will start from cell being currently changed and follow
        /// the cell chain to all cells effected by this change and
        /// will add them to a queue in order.
        /// </summary>
        /// <param name="key">Current cell being modified. </param>
        /// <returns>List of all cells in chain effect.</returns>
        private Queue<GenericCell>? GetDependencyChain(GenericCell key)
        {
            Queue<GenericCell>? dependentCells = new Queue<GenericCell>();
            if (key != null)
            {
                void FindTheKeys(GenericCell indexCell)
                {
                    foreach (var pair in this.Dependencies)
                    {
                        GenericCell keyCell = pair.Key;
                        List<GenericCell> list = pair.Value;

                        if (list.Contains(indexCell))
                        {
                            dependentCells.Enqueue(keyCell);
                            FindTheKeys(keyCell);
                        }
                    }
                }

                FindTheKeys(key);
            }

            return dependentCells;
        }

        /// <summary>
        /// Takes in the formula expression, pull out the variables and passes them into a list along with the location of cell variables.
        /// Then passes it to AddDependency method to push variables to respected key lists in dictionary.
        /// </summary>
        /// <param name="depedentCell"> passes in cell with new expression created.</param>
        private void UpdateCellDependencies(GenericCell depedentCell)
        {
            string formula = depedentCell.CellText;

            if (formula.Length > 0 && formula[0] == '=')
            {
                formula = depedentCell.CellText.Substring(1);

                ExpressionTreeClass myExpressionTree = new ExpressionTreeClass(formula);
                List<GenericCell> referencedCells = new List<GenericCell>();
                List<string> myList = myExpressionTree.GetVariables();
                GenericCell? variableCell;

                // All the dependency cells in the expression gets put into a list.
                foreach (string variable in myList)
                {
                    int columnCell = variable[0] - 'A';
                    int rowCell = int.Parse(variable.Substring(1)) - 1;
                    variableCell = this.GetCell(rowCell, columnCell);
                    if (variableCell != null)
                    {
                        referencedCells.Add(variableCell);
                    }
                }

                this.AddDependency(referencedCells, depedentCell);
            }
        }

        /// <summary>
        /// Goes through and addes dependencies to apropriate key,
        /// then see if existing keys need their list updated.
        /// </summary>
        /// <param name="dependentList"> passes in new list of variables to be added to dictionary of dependences.</param>
        /// <param name="dependencyKey"> gives the cell value in which the formula expression is started, for this current event.</param>
        private void AddDependency(List<GenericCell> dependentList, GenericCell dependencyKey)
        {
            foreach (var cellGiving in dependentList)
            {
                if (dependencyKey != null && dependencyKey != cellGiving)
                {
                    if (!this.Dependencies.ContainsKey(dependencyKey))
                    {
                        this.Dependencies[dependencyKey] = new List<GenericCell>();
                    }

                    if (!this.Dependencies[dependencyKey].Contains(cellGiving))
                    {
                        this.Dependencies[dependencyKey].Add(cellGiving);
                    }

                    // if the provider in the list is also a dependent key,
                    // look at its list as well.
                    if (this.Dependencies.ContainsKey(cellGiving))
                    {
                        List<GenericCell> dependentBag = new List<GenericCell>(this.Dependencies[cellGiving]);

                        this.AddDependency(dependentBag, dependencyKey); // cellGiving
                    }
                }
            }
        }

        /// <summary>
        /// Removes all the dependencies attached to cell key.
        /// </summary>
        /// <param name="cellKey"> passes in the cell that is depended upon.</param>
        private void RemoveAllDependencies(GenericCell cellKey)
        {
            if (this.Dependencies.ContainsKey(cellKey))
            {
                this.Dependencies[cellKey] = new List<GenericCell>();
            }
        }

        /// <summary>
        /// Will take to cell values and see if they will create a circular reference.
        /// </summary>
        /// <param name="cell">Cell currently being changed.</param>
        /// <param name="cellToCheck">Cell to look at if it is referenced in cell chain.</param>
        /// <returns>Returns if statement is true or false.</returns>
        public bool IsCircularReference(GenericCell cell, GenericCell cellToCheck)
        {
            Queue<GenericCell>? dependencyList = this.GetDependencyChain(cell);

            if (dependencyList != null)
            {
                while (dependencyList.Count > 0)
                {
                    GenericCell cellToUpdate = dependencyList.Dequeue();
                    if (cellToCheck == cellToUpdate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates a formula in a string and returns the expression final result.
        /// </summary>
        /// <param name="row">row number.</param>
        /// <param name="col">column number.</param>
        private void EvaluateCellFormula(int row, int col)
        {
            var cell = this.GetCell(row, col) as SpreadSheeT_Cell;

            if (cell != null)
            {
                if (cell.CellText.Length > 0 && cell.CellText[0] == '=')
                {
                    string formula = cell.CellText.Substring(1);
                    ExpressionTreeClass myExpressionTree = new ExpressionTreeClass(formula);

                    // Set other variables as needed based on the formula dependencies
                    List<string> variableList;
                    variableList = myExpressionTree.GetVariables();

                    // After cell expression evaluated each variable is examined to be
                    // set or catch an error.
                    foreach (string item in variableList)
                    {
                        int columnCell = item[0] - 'A';
                        int rowCell = int.Parse(item.Substring(1)) - 1;

                        var variableCell = this.GetCell(rowCell, columnCell);
                        if (variableCell != null)
                        {
                            double variableValue;

                            // Checking for circular referencing before proceding.
                            if (this.IsCircularReference(cell, variableCell))
                            {
                                cell.SetValue("CIRCULAR_REF!!");
                                return;
                            }

                            // If statement to catch self referencing.
                            if (variableCell == cell)
                            {
                                cell.SetValue("Self_Reference!");
                                return;
                            }
                            else if (double.TryParse(variableCell.GetValue(), out variableValue))
                            {
                                myExpressionTree.SetVariable(item, variableValue);
                            }
                            else if (variableCell.CellValue == string.Empty)
                            {
                                myExpressionTree.SetVariable(item, 0);
                            }
                            else
                            {
                                // If the value is not a number, throw an exception
                                throw new InvalidOperationException($"The value of cell {item} is not a valid number and cannot be used in a formula.");
                            }
                        }
                        else
                        {
                            // If the referenced cell does not exist, throw an exception
                            throw new InvalidOperationException($"The referenced cell {item} does not exist.");
                        }
                    }

                    string evaluatedFormula = myExpressionTree.Evaluate().ToString();
                    cell.SetValue(evaluatedFormula);
                }
                else
                {
                    cell.SetValue(cell.CellText);
                }
            }
        }

        private class SpreadSheeT_Cell : GenericCell
        {
            public SpreadSheeT_Cell(int rowIndex, int columnIndex)
                : base(rowIndex, columnIndex)
            {
            }

            public void SetValue(string value)
            {
                this.cellValue = value;
                this.OnPropertyChanged(nameof(this.cellValue));
            }
        }
    }
}
