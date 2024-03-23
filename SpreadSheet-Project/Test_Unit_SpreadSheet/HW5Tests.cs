using SpreadSheet_Engine;

namespace Test_Unit_SpreadSheet
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Constructor_InitializesCellArrayWithCorrectDimensions()
        {
        
            int rowCount = 10;
            int columnCount = 5;

            MYSpreadSheet spreadsheet = new(rowCount, columnCount);
            Assert.Multiple(() =>
            {
                Assert.That(spreadsheet.RowCount, Is.EqualTo(rowCount));
                Assert.That(spreadsheet.ColumnCount, Is.EqualTo(columnCount));
            });
        }

        [Test]
        public void ChangeCell_SetsCellText()
        {
            
            MYSpreadSheet spreadsheet = new(10, 5);
            int row = 0;
            int col = 0;
            string text = "Hello, World";

            spreadsheet.ChangeCell(row, col, text);

            Assert.That(spreadsheet.GetCell(row, col)?.CellText, Is.EqualTo(text));
        }

        [Test]
        public void GetCell_ReturnsCellOrNull()
        {
     
            MYSpreadSheet spreadsheet = new(10, 5);
            int row = 0;
            int col = 0;

            GenericCell? cell = spreadsheet.GetCell(row, col);

            Assert.That(cell, Is.Not.Null); // Cell should exist
            Assert.Multiple(() =>
            {
                Assert.That(cell?.RowIndex, Is.EqualTo(row));
                Assert.That(cell?.ColumnIndex, Is.EqualTo(col));
            });
        }

        [Test]
        public void GetCell_ReturnsNullForInvalidCoordinates()
        {
            
            MYSpreadSheet spreadsheet = new(10, 5);
            int row = -1;
            int col = 10;

            GenericCell? cell = spreadsheet.GetCell(row, col);

            Assert.That(cell, Is.Null); // Cell should not exist
        }
    }
}