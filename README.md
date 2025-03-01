# Operating Systems Project 1 - Monk Simulation  
This program is written in C# and is loosely based on the Dining Philosophers problem. The user can enter how many monks and chopsticks they want present in the Monastery, then can wait as the Monks randomly take actions.  
These actions and the chance for each to occur is as follows:  
10% chance for each Monk to go eat for 5 seconds. The Monk will grab one chopstick and eat. If there are 0 chopsticks, he will wait until someone who is eating will return. After eating, the Monk will return his one chopstick.  
50% chance for the Monk to meditate for 10 seconds. The Monk will meditate in 10 second intervals, checking if one Monk is eating. If the monk meditates for a full minute, he will go out on a Pilgrimage. If all monks are sleeping, the highest priority monk will immediate go to eat for 10 seconds instead of five.  
40% chance for the Monk to sleep for 15 seconds.  

This project was designed to display three features:  
Multithreading - Each monk runs on their own thread.  
Resource Protection - Each shared variable has a lock designed to prevent race conditions, allowing all monks stable access to resources  
Deadlock timeout and detection - Monks will go on a pilgrimage if they meditate for a full minute. If all available monks are meditating,they will all recognize their situation and the first monk will resolve it.  

Each monk will partake in 10 activities, and go out on a pilgrimage from which they will never return.  

##Installation and Setup  
1. Clone this repository into Visual Studio Code:  
2. https://github.com/ian-roundkicker/OSProject1.git  
3. For using C# Programs in Visual Studio Code, you will need the C# SDK  

Or alternatively, you could just copy the source code of Program.cs into the C# IDE of your choice.  


If you wish to use part 2, you will need to be in a Linux Environment.  
These are the commands I used:  
**`ps aux | awk '{print $1, $2, $11}' | sort`** - List the User, Process ID, and source of all processes  
**`seq 100000 | awk 'BEGIN {OFS=","} {print $1, "data" $1, rand()}' > largefile.csv`** - Generate 100,000 lines of a CSV file  
**`(time (cat largefile.csv | tee /dev/null | wc -l)) &> log.txt`** - Read the entirety of largefile.csv, count the number of lines, and output it as log.txt   
**`(time (cat largefile.csv | awk '{system("sleep 0.01"); print}' | tee /dev/null | wc -l)) &> log.txt & sleep 2; pkill -9 cat`** - Read the entirety of largefile.csv, count the number of lines, and output it as log.txt, but there is a delay of 0.01 seconds after each line is read and the cat process is killed in 2 seconds  
