using System;
using System.Collections.Generic;


namespace TSS.UniverseLogic
{
    [Serializable]
    public class Universe:IUniverse
    {
        IUniverseObject[,] universeMatrix;
        List<ICell> cellList = new List<ICell>(0);
        int width, height;
        long ticksCount ;
        int blockCellDesc ;
        ICell MostFitGenome_OneCell;
        int MostFitGenome_CellsCount;

        public Universe(int width, int height, int cellsStartCount)
        {
            this.width = width;
            this.height = height;
            universeMatrix = new IUniverseObject[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    AddUniverseObject(i, j, new UniverseObject()/*need_factory*/, false);
                }
            }
            MostFitGenome_CellsCount = 0;
            ticksCount = 0;
            blockCellDesc = 0;
            GenerateCells(cellsStartCount);
        }//=
        public void DoUniverseTick()
        {
            HandleAllCellsMoves();
            GenerateFood(ConstsUniverse.Special_FoodCountForTick);
            CheckAllCells();
            CalcMostFitCell();

            if (GetCellsCount() == 0)
                GenerateCells(1);

            ticksCount++;
        }
        public int GetHeight()
        {
            return height;
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetObjectDescriptor(int x, int y)
        {
            if (ValidateCords(x, y))
                return GetMatrixElement(x, y).GetDescriptor();
            return 0;
        }
        public int[,] GetAllDescriptors()
        {
            int[,] descriptors = new int[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    descriptors[i, j] = GetMatrixElement(i,j).GetDescriptor();
                }
            }
            return descriptors;
        }
        public int GetCellsCount()
        {
            return cellList.Count;
        }
        public long GetTicksCount()
        {
            return ticksCount;
        }
        public Tuple<ICell, int> GetMostFit()
        {
            return Tuple.Create<ICell, int>(
                MostFitGenome_OneCell,
                MostFitGenome_CellsCount
                );
        }
        bool ValidateCords(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }
        void SetMatrixElement(int x, int y, IUniverseObject universeObject)
        {
            universeMatrix[x,y] = universeObject;
            universeObject.SetCords(x, y);
        }
        IUniverseObject GetMatrixElement(int x, int y)
        {
            return universeMatrix[x,y];
        }
        bool AddUniverseObject(int x, int y, IUniverseObject universeObject, bool canReSetPrevObject)
        {
            if (GetMatrixElement(x, y) == null)
            {
                SetMatrixElement(x, y, universeObject);
                return true;
            }
            else if (canReSetPrevObject || GetMatrixElement(x, y).isDisposed())
            {
                GetMatrixElement(x, y).Dispose();
                SetMatrixElement(x, y, universeObject);
                return true;
            }
            return false;
        }
        void RelocateUniverseObject(int x1, int y1, int x2, int y2)
        {
            SetMatrixElement(x2, y2, GetMatrixElement(x1, y1));
            SetMatrixElement(x1, y1, new UniverseObject()/*need_factory*/);
        }//=
        void GenerateCells(int count)
        {
            if (cellList.Count > 0)
            {
                foreach(Cell cell in cellList.ToArray())
                    KillCell(cell.GetX(), cell.GetY());
            }
            List<ICell> bufCellList = new List<ICell>(0);
            for (int i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(width);
                int y = StableRandom.rd.Next(height);
                ICell cell = new Cell(new Genome())/*need_factory*/;
                if (AddUniverseObject(x, y, cell, false))
                    bufCellList.Add(cell);
            }
            cellList = bufCellList;
        }//=
        void GenerateFood(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = StableRandom.rd.Next(0, width);
                int y = StableRandom.rd.Next(0, height);
                AddUniverseObject(x, y, new Food(FoodType.defaultFood), false);
            }
        }
        void KillCell(int x, int y)
        {
            GetMatrixElement(x, y).Dispose();
            AddUniverseObject(x, y, new Food(FoodType.deadCell)/*need_factory*/, true);
        }
        void HandleCellMove(ICell cell)
        {
            int x1=cell.GetX(), y1=cell.GetY(), x2, y2;
            cell.CalcMoveDirectionAspiration(this);
            MoveDirection md = cell.GetMoveDisperation();
            switch (md)
            {
                case MoveDirection.up:
                    x2 = x1; y2 = y1 - 1;
                    break;
                case MoveDirection.down:
                    x2 = x1; y2 = y1 + 1;
                    break;
                case MoveDirection.left:
                    x2 = x1-1; y2 = y1;
                    break;
                case MoveDirection.right:
                    x2 = x1+1; y2 = y1;
                    break;
                default:
                    return;
            }

            if (!ValidateCords(x2, y2))
                return;

            IUniverseObject unObj = GetMatrixElement(x2, y2);
            int desc = unObj.GetDescriptor();

            if (unObj.isDisposed())
            {
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (desc == -2 || desc==-1)
            {
                cell.AddEnergy((unObj as IFood).GetEnergyLevel());
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (GetMatrixElement(x1, y1).GetDescriptor() == unObj.GetDescriptor())
            {
                cell.AddEnergy(ConstsUniverse.EnergyLevel_MovesFriendly);
                (unObj as ICell).AddEnergy(ConstsUniverse.EnergyLevel_MovesFriendly);
            }
            else
            {
                cell.AddEnergy((float)(ConstsUniverse.EnergyLevel_MovesAggression*0.8));
                (unObj as ICell).AddEnergy(-ConstsUniverse.EnergyLevel_MovesAggression);
            }

        }
        void HandleAllCellsMoves()
        {
            foreach (ICell cell in cellList)
            {
                HandleCellMove(cell);
            }
        }
        void CheckAllCells()
        {
            bool notUniverseOverflow = cellList.Count <= ConstsUniverse.CellsCount_MaxAtField;
            List<ICell> bufCellList = new List<ICell>(0);

            for(int i=0; i<cellList.Count; i++)
            {
                if (cellList[i].isDisposed())
                    continue;         
                if (cellList[i].IncAge() || cellList[i].DecEnergy())
                {
                    KillCell(cellList[i].GetX(), cellList[i].GetY());
                    continue;
                }
                else
                    bufCellList.Add(cellList[i]);

                if (notUniverseOverflow && cellList[i].CanReproduct() && cellList[i].GetDescriptor()!=blockCellDesc)
                {
                    List<MoveDirection> md = new List<MoveDirection>(0);
                    int x = cellList[i].GetX();
                    int y = cellList[i].GetY();
                    if (ValidateCords(x,y-1) && GetMatrixElement(x,y - 1).GetDescriptor()<100)
                        md.Add(MoveDirection.up);
                    if (ValidateCords(x, y + 1) && GetMatrixElement(x, y+1).GetDescriptor() < 100)
                        md.Add(MoveDirection.down);
                    if (ValidateCords(x-1, y ) && GetMatrixElement(x-1, y).GetDescriptor() < 100)
                        md.Add(MoveDirection.left);
                    if (ValidateCords(x+1, y ) && GetMatrixElement(x+1, y).GetDescriptor() < 100)
                        md.Add(MoveDirection.right);

                    MoveDirection choice = MoveDirection.stand;
                    if (md.Count>0)
                        choice = md[StableRandom.rd.Next(md.Count)];

                    if (choice == MoveDirection.stand)
                        continue;

                    ICell newCell = cellList[i].CreateChild();
                    switch (choice)
                    {
                        case MoveDirection.up:
                            AddUniverseObject(x, y - 1, newCell, true);
                            break;
                        case MoveDirection.down:
                            AddUniverseObject(x, y + 1, newCell, true);
                            break;
                        case MoveDirection.left:
                            AddUniverseObject(x - 1, y, newCell, true);
                            break;
                        case MoveDirection.right:
                            AddUniverseObject(x+1, y, newCell, true);
                            break;

                    }
                    bufCellList.Add(newCell);
                }
            }

            List<ICell> bufCellList2;
            if (ticksCount % 11 == 0)
            {
                bufCellList2 = new List<ICell>(0);
                while (bufCellList.Count > 0)
                {
                    int index = StableRandom.rd.Next(bufCellList.Count);
                    bufCellList2.Add(bufCellList[index]);
                    bufCellList.RemoveAt(index);
                }
            }
            else
                bufCellList2 = bufCellList;


            cellList =bufCellList2;
        }
        void CalcMostFitCell()
        {
            List<int> descriptors = GetCellsDescriptors();
            if (descriptors.Count <= 0)
                return;

            descriptors.Sort();

            List<int> uniqDescs = new List<int>(0);
            List<int> uniqDescsRepeats = new List<int>(0);
            uniqDescs.Add(descriptors[0]);
            uniqDescsRepeats.Add(1);
            int index = 0;
            for (int i = 1; i < descriptors.Count; i++)
            {
                if (descriptors[i] == uniqDescs[index])
                {
                    uniqDescsRepeats[index]++;
                }
                else
                {
                    uniqDescs.Add(descriptors[i]);
                    uniqDescsRepeats.Add(1);
                    index++;
                }
            }
            int highIndex = 0, highValue = uniqDescsRepeats[0];
            for (int i = 1; i < uniqDescs.Count; i++)
            {
                if (uniqDescsRepeats[i] > highValue)
                {
                    highValue = uniqDescsRepeats[i];
                    highIndex = i;
                }
            }

            if (highValue > ConstsUniverse.CellsCount_MaxWithOneType)
                blockCellDesc = uniqDescs[highIndex];
            else
                blockCellDesc = 0;

            MostFitGenome_OneCell = FindCellByDesc(uniqDescs[highIndex]);
            MostFitGenome_CellsCount = highValue;

        }
        ICell FindCellByDesc(int descriptor)
        {
            if (cellList.Count > 0)
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    if (cellList[i].GetDescriptor() == descriptor)
                        return cellList[i];
                }
                return cellList[0];
            }
            return null;
        }
        List<int> GetCellsDescriptors()
        {
            List<int> descriptors = new List<int>(0);
            for(int i = 0; i < cellList.Count; i++)
            {
                descriptors.Add(cellList[i].GetDescriptor());
            }
            return descriptors;
        }    
	}
}
