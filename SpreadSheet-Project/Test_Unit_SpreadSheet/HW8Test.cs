using SpreadSheet_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Unit_SpreadSheet_Engine
{
    internal class HW8Test
    {
        [Test]
        public void UndoTest()
        {
            // Arrange
            var spreadsheet = new MYSpreadSheet(10, 10);
            var originalValue = "Original";
            var newValue = "New";
            spreadsheet.ChangeCell(0, 0, originalValue);
            spreadsheet.ChangeCell(0, 0, newValue);

            // Act
            spreadsheet.Undo();
            var cellValueAfterUndo = spreadsheet.GetCell(0, 0).CellText;

            // Assert
            Assert.AreEqual(originalValue, cellValueAfterUndo, "Undo should revert the cell value to the original.");
        }

        [Test]
        public void RedoTest()
        {
            // Arrange
            var spreadsheet = new MYSpreadSheet(10, 10);
            var originalValue = "Original";
            var newValue = "New";
            spreadsheet.ChangeCell(0, 0, originalValue);
            spreadsheet.ChangeCell(0, 0, newValue);

            // Act - Undo and then Redo
            spreadsheet.Undo();
            spreadsheet.Redo();
            var cellValueAfterRedo = spreadsheet.GetCell(0, 0).CellText;

            // Assert
            Assert.AreEqual(newValue, cellValueAfterRedo, "Redo should reapply the new value to the cell.");
        }
    }
}
