namespace ImmerseYourselfServer
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Console.Title = "ImmerseYourselfServer";
            
            Server.Start(5);
            
            Console.ReadKey();
        }
    }
}