using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Minestryger
{
    internal class Cell : Button
    {

        bool _isRevealed;
        bool _isFlagged;
        bool _isMine;
        int _adjacentMines;
        int _rowPos;
        int _colPos;
        Image _cellImage;


        public Cell(bool isMine, int adjacentMines, int rowPos, int colPos)
        {
            _isRevealed = false;
            _isFlagged = false;
            _isMine = isMine;
            _adjacentMines = adjacentMines;
            _rowPos = rowPos;
            _colPos = colPos;
        }
       

        public bool IsRevealed
        {
            get { return _isRevealed; }
            set { _isRevealed = value; }
        }
        public bool IsFlagged
        {
            get { return _isFlagged; }
            set { _isFlagged = value; }
        }
        public bool IsMine
        {
            get { return _isMine; }
            set { _isMine = value; }
        }
        public int AdjacentMines
        {
            get { return _adjacentMines; }
            set { _adjacentMines = value; }
        }
        public int RowPos
        {
            get { return _rowPos; }
            set { _rowPos = value; }
        }
        public int ColPos
        {
            get { return _colPos; }
            set { _colPos = value; }
        }
    }
}
