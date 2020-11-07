namespace PoolPuzzle
{
    public class Faucet
    {
        public Faucet()
        {
            Table wine = new Table();
            Hinge book = new Hinge();
            wine.Set(book);
            book.Set(wine);
            wine.Lamp(10);
            book.garden.Lamp("back in");
            book.bulb *= 2;
            wine.Lamp("minutes");
            wine.Lamp(book);
        }
    }

}