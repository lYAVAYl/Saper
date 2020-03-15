using System;
using System.Diagnostics;

namespace SelectionSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int[] ms = new int[1000000];
            Random rnd = new Random();

        //--------------------------------------------------------------------------------------------- start
            for(int i=0; i<ms.Length; i++) // присвоение рандомного значения (от -100 до 100) 
                                           // каждому элементу массива ms.
            {
                ms[i] = rnd.Next(-99999999, 999999999);
                //Console.WriteLine($"{i + 1} = {ms[i]}");
            }
            //--------------------------------------------------------------------------------------------- finish
            //Console.Write("\n\n");

            SortClass.QuickSort(ref ms, 0, ms.Length-1);

           // вывод отсортированного массива
           // for (int i=0; i<ms.Length; i++)
           // {
           //    Console.WriteLine($"{i+1} = {ms[i]}");
           // }



            sw.Stop();
            Console.Write("\n\n\n");
            Console.WriteLine(sw.Elapsed);



        }


        




    }
}
