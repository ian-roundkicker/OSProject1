namespace OSProject1;
class Program
{
    private static readonly Object _lock = new();
    private static int chopsticks = 3;
    static void Main(string[] args) 
    {
        int monk_pop;
        try
        {
            Console.WriteLine("Enter the number of monks:");
            monk_pop = int.Parse(Console.ReadLine());
            if (monk_pop < 1) {
                throw new Exception("No monks?");
            }
            Console.WriteLine("Enter the number of chopsticks:");
            chopsticks = int.Parse(Console.ReadLine());
            if (chopsticks < 1) {
                throw new Exception("Monastaries don't function.");
            }
        }
        catch (System.Exception)
        {
            Console.WriteLine("Improper number entered: Defaults will be used instead");
            monk_pop = 30;
            chopsticks = 5;
        }
        Thread[] threads = new Thread[monk_pop];
        for (int i = 0; i < threads.Length; i++) 
        {
            int index = i; // initialize a variable to prevent bizarre duplicates or skipped numbers
            threads[i] = new(() => Work(index+1));
            threads[i].Start();
        }
    }

    static void Work(int x) 
    {
        Console.WriteLine("Monk " + x + " has arrived.");
        Random random = new();
        int decision;
        for (int i = 0; i < 10; i++)
        {
            decision = random.Next(1, 11);
            if (decision == 1) 
            { //the monk intends to eat
            
               lock(_lock) {
                   if (chopsticks == 0) 
                    {//no chopsticks?
                       /* monk has to wait until someone who finished this method returned chopsticks,
                       *  If we don't add the monk to a queue and wait, then we can enter deadlock.
                        */
                        Console.WriteLine("Monk " + x + "  wants to eat, but there are no chopsticks!");
                        Monitor.Wait(_lock);
                    }
                    //monk can eat
                    chopsticks--;
                    Console.WriteLine("Monk " + x + " is eating");
                    Console.WriteLine(chopsticks + " chopsticks remain.");
                }//monk eats
                Thread.Sleep(5000);
                lock (_lock) 
                {
                    chopsticks++;
                    Console.WriteLine("Monk " + x + "  is done eating.");
                    if (chopsticks==1) 
                   {
                        Console.WriteLine("\"Looks like meat's back on the menu, boys\"");
                        Monitor.Pulse(_lock);
                    }
                }
               //where to add monk returning operation. The monk can't just use the same lock, since someone else is in. Any ideas?
            }
            else if(decision > 6) 
            {
                Console.WriteLine("Monk " + x + "  is sleeping");
                Thread.Sleep(15000);
            }
            else 
            {
                Console.WriteLine("Monk " + x + "  is meditating");
                Thread.Sleep(10000);
            }
        }
        Console.WriteLine("Monk " + x + " is going on a pilgrimage: He will never return.");
    }
}
