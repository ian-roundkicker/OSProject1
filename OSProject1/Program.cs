namespace OSProject1;
class Program
{
    private static readonly Object chopstick_lock = new();
    private static int chopsticks = 3;
    private static Object[] decision_locks = null!;
    private static int[] decision_viewable = null!;
    private static int monk_pop;
    private static readonly Object monk_pop_lock = new();
    static void Main(string[] args) 
    {
        Thread[] threads;
        lock (monk_pop_lock)
        {
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
            threads = new Thread[monk_pop];
        }
        decision_viewable = new int[threads.Length];
        decision_locks = new object[threads.Length];
        for (int i = 0; i < threads.Length; i++) 
        {
            int index = i; // initialize a variable to prevent bizarre duplicates or skipped numbers
            decision_locks[i] = new();
            threads[i] = new(() => Work(index+1));
            threads[i].Start();
        }
    }

    static void Work(int x) 
    {
        Console.WriteLine("Monk " + x + " has arrived.");
        Random random = new();
        int decision;
        bool all_meditating = false; // tracks if all monks are currently meditating to fix the deadlock
        bool has_priority = PriorityCheck(x-1); // tracks if the monk has priority, we'll call him PriorityMonk
        bool emergency_eat = false; // only the PriorityMonk can have this set to true
        for (int i = 0; i < 10; i++)
        {
            if (!emergency_eat)
            {
                decision = random.Next(1, 11);
            }
            else // the priorityMonk will always go to eat if everyone else is stuck in meditation
            {
                decision = 1;
            }
            lock(decision_locks[x-1]) {
                decision_viewable[x-1] = decision;
            }
            if (decision == 1) 
            { //the monk intends to eat
                
                lock(chopstick_lock) {
                   if (chopsticks == 0) 
                    {//no chopsticks?
                       /* monk has to wait until someone who finished this method returned chopsticks,
                       *  If we don't add the monk to a queue and wait, then we can enter deadlock.
                        */
                        Console.WriteLine("Monk " + x + " wants to eat, but there are no chopsticks!");
                        Monitor.Wait(chopstick_lock);
                    }
                    //monk can eat
                    chopsticks--;
                    Console.WriteLine("Monk " + x + " is eating");
                    Console.WriteLine(chopsticks + " chopsticks remain.");
                }//monk eats
                if (emergency_eat)
                { // if this is the priorityMonk and everyone else is in Meditation, he will eat for 10 seconds instead of five to free all of his comrades
                    Thread.Sleep(10000);
                    emergency_eat = false;
                }
                else
                {
                    Thread.Sleep(5000);
                }
                lock (chopstick_lock) 
                {
                    chopsticks++;
                    Console.WriteLine("Monk " + x + " is done eating.");
                    if (chopsticks==1) 
                   {
                        Console.WriteLine("\"Looks like meat's back on the menu, boys\"");
                        Monitor.Pulse(chopstick_lock);
                    }
                }
            }
            else if(decision > 6) 
            {
                Console.WriteLine("Monk " + x + " is sleeping");
                Thread.Sleep(15000);
            }
            else 
            {
                Console.WriteLine("Monk " + x + " is meditating");
                for (int k = 0; k < 6; k++) {// for loop functions as the timeout mechanism
                    Thread.Sleep(10000);
                    lock (monk_pop_lock)
                    {
                        if (monk_pop == 1)
                        {
                            goto wake_up_label;
                        }
                    }
                    all_meditating = true;
                    for (int j = 0; j < decision_viewable.Length; j++)
                    {
                        lock(decision_locks[j]) 
                        {
                            if (decision_viewable[j] == 1)
                            {
                                all_meditating = false;
                                goto wake_up_label;
                            }
                            else if (decision_viewable[j] > 6) {
                                all_meditating = false;
                            }
                        }
                    }
                    if (all_meditating)
                    {
                        Console.WriteLine("------------WE-WILL-ASCEND-PAST-OUR-NEED-FOR-FOOD------------");
                        has_priority = PriorityCheck(x-1);
                        if (has_priority)
                        {
                            Console.WriteLine("What is this madness? I must save the monastary!");
                            emergency_eat = true;
                            goto wake_up_label;
                        }
                    }
                }
                goto pilgrimage_label;
                wake_up_label:;
            }
        }
        pilgrimage_label:
        decision = -1;
        lock (decision_locks[x-1]) 
        {
            decision_viewable[x-1] = decision;
        }
        lock (monk_pop_lock)
        {
            monk_pop--;
        }
        Console.WriteLine("Monk " + x + " is going on a pilgrimage: He will never return.");
    }

    static bool PriorityCheck(int index) {
        bool is_priority = true;
        for (int i = 0; i < index; i++)
        {
            lock(decision_locks[i]) {
                if (decision_viewable[i]!=-1) {
                    is_priority = false;
                }
            }
        }
        if (is_priority)
        {
            Console.WriteLine("Monk " + (index+1) + " has priority.");
        }
        return is_priority;
    }
}
