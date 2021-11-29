using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NUnit.Framework;
using TileMapEditor;
using TileMapEditor.Models;
using TileMapEditor.ViewModels;
using TileMapEditor.Views;

namespace TileMapEditorTest
{
    public class Tests
    {
        private MainWindowViewModel _mainWindowViewModel;
        private GridEditorViewModel _gridEditorViewModel;
        private TilePickerViewModel _tilePickerViewModel;
        private TileViewModel _tileViewModel;

        private const int Empty = -1;
        private const int TopLeftCoast = 0;
        private const int TopCoast = 1;
        private const int TopRightCoast = 2;
        private const int LeftCoast = 14;
        private const int PlainGrass = 15;
        private const int RightCoast = 16;
        private const int BottomLeftCoast = 28;
        private const int BottomCoast = 29;
        private const int BottomRightCoast = 30;
        private const int Water = 31;
        private const int BottomRightCornerCoast = 18;
        private const int BottomLeftCornerCoast = 17;
        private const int TopRightCornerCoast = 4;
        private const int TopLeftCornerCoast = 3;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AssetStructure.EmptyPngPath = AppDomain.CurrentDomain.BaseDirectory + "../../../../TileMapEditor/Images/Empty.png";
            AssetStructure.CollisionPngPath = AppDomain.CurrentDomain.BaseDirectory + "../../../../TileMapEditor/Images/Collision.png";
            AssetStructure.TilesetPath = AppDomain.CurrentDomain.BaseDirectory + "../../../../TileMapEditor/Images/tilemap.png";
        }

        [SetUp]
        public void Setup()
        {
            _gridEditorViewModel = new GridEditorViewModel(10, 15);
        }

        [Test]
        public void Test_CreateNewMainViewModel()
        {
            
        }

        [Test]
        public void Test_CreateNewGrid()
        {
            const int rows = 10;
            const int columns = 20;
            _gridEditorViewModel = new GridEditorViewModel(rows, columns);
            var tiles = _gridEditorViewModel.MapTiles;


            Assert.AreEqual(rows, _gridEditorViewModel.Rows);
            Assert.AreEqual(columns, _gridEditorViewModel.Columns);
            Assert.IsNotEmpty(tiles);

            var rowNr = 0;
            var colNr = 0;
            foreach (var tile in tiles)
            {
                Assert.AreEqual(-1, tile.ImageIdBottom);
                Assert.AreEqual(-1, tile.ImageIdTop);
                Assert.AreEqual(false, tile.IsCollidable);
                Assert.AreEqual(new Point(rowNr, colNr), tile.TilePositionOnGrid);
                colNr++;
                if (colNr >= columns)
                {
                    colNr = 0;
                    rowNr++;
                }
            }
        }

        [Test]
        public void Test_CreateNewTilePicker()
        {
            const int rows = 10;
            const int columns = 10;
            const int tileWith = 16;
            const int tileHeight = 16;

            _tilePickerViewModel = new TilePickerViewModel(rows, columns, tileWith, tileHeight, AssetStructure.TilesetPath);

            Assert.AreEqual(rows * columns, _tilePickerViewModel.Tiles.Count);
        }

        [Test]
        public void Test_PlacePlainGrassAndUpdateTileNeighboursOnEmptyGrid()
        {
            var tileOnGrid = new MapTile
            {
                ImageIdBottom = PlainGrass,
                TilePositionOnGrid = new Point(5, 5)
            };

            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid);

            var leftOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, -1);
            var rightOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, 1);
            var aboveTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1);
            var aboveLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, -1);
            var aboveRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, +1);
            var belowTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 1);
            var belowLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, -1);
            var belowRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, +1);

            Assert.AreEqual(LeftCoast, leftOfTile.ImageIdBottom);
            Assert.AreEqual(RightCoast, rightOfTile.ImageIdBottom);
            Assert.AreEqual(TopCoast, aboveTile.ImageIdBottom);
            Assert.AreEqual(TopLeftCoast, aboveLeftCorner.ImageIdBottom);
            Assert.AreEqual(TopRightCoast, aboveRightCorner.ImageIdBottom);
            Assert.AreEqual(BottomCoast, belowTile.ImageIdBottom);
            Assert.AreEqual(BottomLeftCoast, belowLeftCorner.ImageIdBottom);
            Assert.AreEqual(BottomRightCoast, belowRightCorner.ImageIdBottom);
        }

        [Test]
        public void Test_PlacePlainGrassTenTimesToTheLeft()
        {
            for (var i = 0; i < 10; i++)
            {
                var tileOnGrid = new MapTile
                {
                    ImageIdBottom = PlainGrass,
                    TilePositionOnGrid = new Point(5, 13 - i)
                };

                var original = _gridEditorViewModel.GetTileFromPosition(tileOnGrid);
                original.ImageIdBottom = PlainGrass;

                _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid);
                var leftOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, -1);
                var rightOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, 1);
                var aboveTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1);
                var aboveLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, -1);
                var aboveRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, +1);
                var belowTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 1);
                var belowLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, -1);
                var belowRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, +1);

                Assert.AreEqual(LeftCoast, leftOfTile.ImageIdBottom);
                Assert.AreEqual(TopCoast, aboveTile.ImageIdBottom);
                Assert.AreEqual(TopLeftCoast, aboveLeftCorner.ImageIdBottom);
                Assert.AreEqual(BottomCoast, belowTile.ImageIdBottom);
                Assert.AreEqual(BottomLeftCoast, belowLeftCorner.ImageIdBottom);

                if (i == 0)
                {
                    Assert.AreEqual(RightCoast, rightOfTile.ImageIdBottom);
                    Assert.AreEqual(TopRightCoast, aboveRightCorner.ImageIdBottom);
                    Assert.AreEqual(BottomRightCoast, belowRightCorner.ImageIdBottom);
                }
                else
                {
                    Assert.AreEqual(PlainGrass, rightOfTile.ImageIdBottom);
                    Assert.AreEqual(TopCoast, aboveRightCorner.ImageIdBottom);
                    Assert.AreEqual(BottomCoast, belowRightCorner.ImageIdBottom);
                }
            }
        }

        [Test]
        public void Test_PlacePlainGrassInEachCornerAndCheckOutOfBounds()
        {
            var tileTopLeftCorner = new MapTile
            {
                ImageIdBottom = PlainGrass,
                TilePositionOnGrid = new Point(0,0)
            };
            _gridEditorViewModel.UpdateTileNeighbours(tileTopLeftCorner);

            var tileTopRightCorner = new MapTile
            {
                ImageIdBottom = PlainGrass,
                TilePositionOnGrid = new Point(0, 14)
            };
            _gridEditorViewModel.UpdateTileNeighbours(tileTopRightCorner);

            var tileBottomLeftCorner = new MapTile
            {
                ImageIdBottom = PlainGrass,
                TilePositionOnGrid = new Point(9, 0)
            };
            _gridEditorViewModel.UpdateTileNeighbours(tileBottomLeftCorner);

            var tileBottomRightCorner = new MapTile
            {
                ImageIdBottom = PlainGrass,
                TilePositionOnGrid = new Point(9, 14)
            };
            _gridEditorViewModel.UpdateTileNeighbours(tileBottomRightCorner);

            // SHOULD BE OUT OF BOUNDS

            // TOP LEFT TILE
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopLeftCorner, 0, -1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopLeftCorner, -1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopLeftCorner, -1, -1));

            // BOTTOM LEFT TILE
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomLeftCorner, 0, -1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomLeftCorner, 1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomLeftCorner, 1, -1));

            // TOP RIGHT TILE
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopRightCorner, 0, 1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopRightCorner, -1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileTopRightCorner, -1, 1));

            // BOTTOM RIGHT TILE
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomRightCorner, 0, 1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomRightCorner, 1));
            Assert.IsNull(_gridEditorViewModel.GetTileFromPosition(tileBottomRightCorner, 1, 1));
        }

        [Test]
        public void Test_RemoveGrassAndAllSurroundingTiles()
        {
            var tileOnGrid = new MapTile
            {
                TilePositionOnGrid = new Point(5, 5)
            };

            var original = _gridEditorViewModel.GetTileFromPosition(tileOnGrid);
            original.ImageIdBottom = PlainGrass;

            // Add surrounding
            _gridEditorViewModel.UpdateTileNeighbours(original);

            // Remove
            _gridEditorViewModel.UpdateRemoveTileNeighbours(original);

            var leftOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, -1);
            var rightOfTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 0, 1);
            var aboveTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1);
            var aboveLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, -1);
            var aboveRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, -1, +1);
            var belowTile = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, 1);
            var belowLeftCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, -1);
            var belowRightCorner = _gridEditorViewModel.GetTileFromPosition(tileOnGrid, +1, +1);

            Assert.AreEqual(Empty, leftOfTile.ImageIdBottom);
            Assert.AreEqual(Empty, aboveTile.ImageIdBottom);
            Assert.AreEqual(Empty, aboveLeftCorner.ImageIdBottom);
            Assert.AreEqual(Empty, belowTile.ImageIdBottom);
            Assert.AreEqual(Empty, belowLeftCorner.ImageIdBottom);
            Assert.AreEqual(Empty, rightOfTile.ImageIdBottom);
            Assert.AreEqual(Empty, aboveRightCorner.ImageIdBottom);
            Assert.AreEqual(Empty, belowRightCorner.ImageIdBottom);
        }

        [Test]
        public void Test_CreateBottomRightCornerWhenPlacingPlainGrass()
        {
            var tile01 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 5)
            };
            var tileOnGrid01 = _gridEditorViewModel.GetTileFromPosition(tile01);
            tileOnGrid01.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid01);

            var tile02 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 6)
            };
            var tileOnGrid02 = _gridEditorViewModel.GetTileFromPosition(tile02);
            tileOnGrid02.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid02);

            var tile03 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 6)
            };
            var tileOnGrid03 = _gridEditorViewModel.GetTileFromPosition(tile03);
            tileOnGrid03.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid03);


            var tileToCheck = new MapTile
            {
                TilePositionOnGrid = new Point(4, 5)
            };
            Assert.AreEqual(BottomRightCornerCoast, _gridEditorViewModel.GetTileFromPosition(tileToCheck).ImageIdBottom);
        }

        [Test]
        public void Test_CreateBottomLeftCornerWhenPlacingPlainGrass()
        {
            var tile01 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 5)
            };
            var tileOnGrid01 = _gridEditorViewModel.GetTileFromPosition(tile01);
            tileOnGrid01.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid01);

            var tile02 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 4)
            };
            var tileOnGrid02 = _gridEditorViewModel.GetTileFromPosition(tile02);
            tileOnGrid02.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid02);

            var tile03 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 4)
            };
            var tileOnGrid03 = _gridEditorViewModel.GetTileFromPosition(tile03);
            tileOnGrid03.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid03);


            var tileToCheck = new MapTile
            {
                TilePositionOnGrid = new Point(4, 5)
            };
            Assert.AreEqual(BottomLeftCornerCoast, _gridEditorViewModel.GetTileFromPosition(tileToCheck).ImageIdBottom);
        }

        [Test]
        public void Test_CreateTopLeftCornerWhenPlacingPlainGrass()
        {
            var tile01 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 5)
            };
            var tileOnGrid01 = _gridEditorViewModel.GetTileFromPosition(tile01);
            tileOnGrid01.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid01);

            var tile02 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 5)
            };
            var tileOnGrid02 = _gridEditorViewModel.GetTileFromPosition(tile02);
            tileOnGrid02.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid02);

            var tile03 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 6)
            };
            var tileOnGrid03 = _gridEditorViewModel.GetTileFromPosition(tile03);
            tileOnGrid03.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid03);


            var tileToCheck = new MapTile
            {
                TilePositionOnGrid = new Point(5, 6)
            };
            Assert.AreEqual(TopLeftCornerCoast, _gridEditorViewModel.GetTileFromPosition(tileToCheck).ImageIdBottom);
        }

        [Test]
        public void Test_CreateTopRightCornerWhenPlacingPlainGrass()
        {
            var tile01 = new MapTile
            {
                TilePositionOnGrid = new Point(5, 5)
            };
            var tileOnGrid01 = _gridEditorViewModel.GetTileFromPosition(tile01);
            tileOnGrid01.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid01);

            var tile02 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 5)
            };
            var tileOnGrid02 = _gridEditorViewModel.GetTileFromPosition(tile02);
            tileOnGrid02.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid02);

            var tile03 = new MapTile
            {
                TilePositionOnGrid = new Point(4, 4)
            };
            var tileOnGrid03 = _gridEditorViewModel.GetTileFromPosition(tile03);
            tileOnGrid03.ImageIdBottom = PlainGrass;
            _gridEditorViewModel.UpdateTileNeighbours(tileOnGrid03);


            var tileToCheck = new MapTile
            {
                TilePositionOnGrid = new Point(5, 4)
            };
            Assert.AreEqual(TopRightCornerCoast, _gridEditorViewModel.GetTileFromPosition(tileToCheck).ImageIdBottom);
        }

        [Test]
        public void Test_FillEmptyGridWithGrass()
        {
            foreach (var tile in _gridEditorViewModel.MapTiles)
            {
                Assert.AreEqual(Empty, tile.ImageIdBottom);
            }

            var tileSentFromViewToFillEmptySpace = new Tile
            {
                ImageId = PlainGrass
            };

            OnFillEmptyGridSpaceWithSelectedTile(tileSentFromViewToFillEmptySpace);

            foreach (var tile in _gridEditorViewModel.MapTiles)
            {
                Assert.AreEqual(PlainGrass, tile.ImageIdBottom);
            }
        }

        [Test]
        public void Test_FillEmptySpaceOnGridWithGrass()
        {
            foreach (var tile in _gridEditorViewModel.MapTiles)
            {
                Assert.AreEqual(Empty, tile.ImageIdBottom);
            }

            var tileToBeChanged01 = _gridEditorViewModel.GetTileFromPosition(new MapTile { TilePositionOnGrid = new Point(5, 5) });
            tileToBeChanged01.ImageIdBottom = BottomRightCornerCoast;

            var tileToBeChanged02 = _gridEditorViewModel.GetTileFromPosition(new MapTile { TilePositionOnGrid = new Point(3, 8) });
            tileToBeChanged02.ImageIdBottom = Water;

            var tileToBeChanged03 = _gridEditorViewModel.GetTileFromPosition(new MapTile { TilePositionOnGrid = new Point(7, 1) });
            tileToBeChanged03.ImageIdBottom = BottomCoast;

            var tileToBeChanged04 = _gridEditorViewModel.GetTileFromPosition(new MapTile { TilePositionOnGrid = new Point(8, 9) });
            tileToBeChanged04.ImageIdBottom = TopRightCornerCoast;

            var tileToBeChanged05 = _gridEditorViewModel.GetTileFromPosition(new MapTile { TilePositionOnGrid = new Point(3, 2) });
            tileToBeChanged05.ImageIdBottom = LeftCoast;

            var tileSentFromViewToFillEmptySpace = new Tile
            {
                ImageId = PlainGrass
            };

            OnFillEmptyGridSpaceWithSelectedTile(tileSentFromViewToFillEmptySpace);

            foreach (var tile in _gridEditorViewModel.MapTiles)
            {
                if (tile.TilePositionOnGrid == tileToBeChanged01.TilePositionOnGrid)
                {
                    Assert.AreNotEqual(PlainGrass, tileToBeChanged01.ImageIdBottom);
                }
                else if (tile.TilePositionOnGrid == tileToBeChanged02.TilePositionOnGrid)
                {
                    Assert.AreNotEqual(PlainGrass, tileToBeChanged02.ImageIdBottom);
                }
                else if (tile.TilePositionOnGrid == tileToBeChanged03.TilePositionOnGrid)
                {
                    Assert.AreNotEqual(PlainGrass, tileToBeChanged03.ImageIdBottom);
                }
                else if (tile.TilePositionOnGrid == tileToBeChanged04.TilePositionOnGrid)
                {
                    Assert.AreNotEqual(PlainGrass, tileToBeChanged04.ImageIdBottom);
                }
                else if (tile.TilePositionOnGrid == tileToBeChanged05.TilePositionOnGrid)
                {
                    Assert.AreNotEqual(PlainGrass, tileToBeChanged05.ImageIdBottom);
                }
                else
                {
                    Assert.AreEqual(PlainGrass, tile.ImageIdBottom);
                }
            }
        }

        // Copy of event from TileViewModel
        private void OnFillEmptyGridSpaceWithSelectedTile(Tile tileFromTileViewModel)
        {
            foreach (var tile in _gridEditorViewModel.MapTiles)
            {
                if (tileFromTileViewModel.LayerId == 0)
                {
                    if (tile.ImageIdBottom != -1)
                    {
                        continue;
                    }

                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdBottom = tileFromTileViewModel.ImageId;
                    tile.ImageSourceBottomLayer = tileFromTileViewModel.ImageSource;
                }
                else
                {
                    if (tile.ImageIdTop != -1)
                    {
                        continue;
                    }

                    tile.IsCollidable = tileFromTileViewModel.IsCollidable;
                    tile.ImageIdTop = tileFromTileViewModel.ImageId;
                    tile.ImageSourceTopLayer = tileFromTileViewModel.ImageSource;
                }
            }
        }
    }
}