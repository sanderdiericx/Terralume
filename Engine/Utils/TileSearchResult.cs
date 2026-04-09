namespace Terralume.Engine.Utils
{
    public struct TileSearchResult
    {
        public ushort[] TileIDs;
        public bool[] Tiles;

        public TileSearchResult(ushort[] tileIDs, bool[] tiles)
        {
            TileIDs = tileIDs;
            Tiles = tiles;
        }
    }
}
