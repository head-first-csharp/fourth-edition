using System;
using System.Collections.Generic;

namespace StackAndQueue2
{
    class Program
    {
        static void Main(string[] args)
        {

            Stack<string> myStack = new Stack<string>();
            myStack.Push("first in line");
            myStack.Push("second in line");
            myStack.Push("third in line");
            myStack.Push("last in line");

            Queue<string> myQueue = new Queue<string>(myStack);
            List<string> myList = new List<string>(myQueue);
            Stack<string> anotherStack = new Stack<string>(myList);

            Console.WriteLine($@"myQueue has {myQueue.Count} items
myList has {myList.Count} items
anotherStack has {anotherStack.Count} items");
        }
    }
}
