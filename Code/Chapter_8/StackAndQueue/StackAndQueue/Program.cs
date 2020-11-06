using System;
using System.Collections.Generic;

namespace StackAndQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a Queue and add four strings to it
            Queue<string> myQueue = new Queue<string>();
            myQueue.Enqueue("first in line");
            myQueue.Enqueue("second in line");
            myQueue.Enqueue("third in line");
            myQueue.Enqueue("last in line");

            // Peek "looks" at the first item in the queue without removing it
            Console.WriteLine($"Peek() returned:\n{myQueue.Peek()}");

            // Dequeue pulls the next item from the FRONT of the queue
            Console.WriteLine(
                $"The first Dequeue() returned:\n{myQueue.Dequeue()}");
            Console.WriteLine(
                $"The second Dequeue() returned:\n{myQueue.Dequeue()}");

            // Clear removes all of the items from the queue
            Console.WriteLine($"Count before Clear():\n{myQueue.Count}");
            myQueue.Clear();
            Console.WriteLine($"Count after Clear():\n{myQueue.Count}");


            // Create a Stack and add four strings to it
            Stack<string> myStack = new Stack<string>();
            myStack.Push("first in line");
            myStack.Push("second in line");
            myStack.Push("third in line");
            myStack.Push("last in line");

            // Peek with a stack works just like it does with a queue
            Console.WriteLine($"Peek() returned:\n{myStack.Peek()}");

            // Pop pulls the next item from the BOTTOM of the stack
            Console.WriteLine(
                $"The first Pop() returned:\n{myStack.Pop()}");
            Console.WriteLine(
                $"The second Pop() returned:\n{myStack.Pop()}");

            Console.WriteLine($"Count before Clear():\n{myStack.Count}");
            myStack.Clear();
            Console.WriteLine($"Count after Clear():\n{myStack.Count}");
        }
    }
}
