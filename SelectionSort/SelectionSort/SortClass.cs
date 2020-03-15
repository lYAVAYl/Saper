using System;


namespace SelectionSort
{
    class SortClass
    {


        public static void SelectionSort(ref int[] arr) // сортировка выбором
        {
            int min;
            int ind;

            //----------------------------------------------------------------------------------- start
            for (int i = 0; i < arr.Length - 1; i++) //сортировка массива
            {
                min = arr[i];
                ind = i;

                //---------------------------------------------------- start
                for (int k = i + 1; k < arr.Length; k++)
                {
                    if (arr[k] < min) // проверка на минимальное число
                    {
                        min = arr[k];
                        ind = k;
                    }


                } //-------------------------------------------------- finish

                arr[ind] = arr[i];
                arr[i] = min;

            }
            //----------------------------------------------------------------------------------- finish
        }

        public static void QuickSort(ref int[] arr, int first, int last)
        {
            double opor_num = arr[(last - first) / 2 + first]; // опорный элемент
            // (последний элемент - первый) / 2 + первый 
            // приплюсовываем первый элемент, чтобы учесть сдвиг
            // иначе (9-0)/2+0=4
            // далее рассмотрим часть от 5 до 9:(9-5)/2=2 -- это значение не входит в промежуток [5..9]
            // для этого прибавляем сдвиг (5): (9-5)/2+5=7 -- входит в промежуток [5..9]


            int temp;
            int i = first, j = last;
            while (i <= j)
            {
                while (arr[i] < opor_num && i <= last) ++i;
                while (arr[j] > opor_num && j >= first) --j;
                if (i <= j)
                {
                    temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                    ++i; --j;
                }
            }
            if (j > first) QuickSort(ref arr, first, j);
            if (i < last) QuickSort(ref arr, i, last);
        }

    }
}
