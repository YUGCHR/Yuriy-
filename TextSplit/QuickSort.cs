using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSplit.Tests
{
    public class QuickSort
    {
        public QuickSort()
        {

        }

        public int ChapterNameDigitSortPrepare(int[] chapterNameIsDigitsOnly, int start, int end)//сначала находим наименьшее число в массиве чисел
        {
            int temp;//swap helper
            int marker = start;
            //MessageBox.Show("start = " + start.ToString() + "\r\n" + "end = " + end.ToString(), "ChapterNameDigitQuickSort started", MessageBoxButtons.OK, MessageBoxIcon.Information);
            for (int i = start; i <= end; i++)
            {
                if (chapterNameIsDigitsOnly[i] < chapterNameIsDigitsOnly[end])//chapterNameIsDigitsOnly[end] is pivot
                {
                    temp = chapterNameIsDigitsOnly[marker];//swap
                    chapterNameIsDigitsOnly[marker] = chapterNameIsDigitsOnly[i];
                    chapterNameIsDigitsOnly[i] = temp;
                    marker += 1;
                }
            }
            //put pivot (chapterNameIsDigitsOnly[endIndex]) between left anf right subarrays
            temp = chapterNameIsDigitsOnly[marker];
            chapterNameIsDigitsOnly[marker] = chapterNameIsDigitsOnly[end];
            chapterNameIsDigitsOnly[end] = temp;
            return marker;
        }

        public void ChapterNameDigitQuickSort(int[] chapterNameIsDigitsOnly, int start, int end)
        {
            //MessageBox.Show("start = " + start.ToString() + "\r\n" + "end = " + end.ToString(), "ChapterNameDigitQuickSort started", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (start >= end)
            {
                return;
            }
            int pivot = ChapterNameDigitSortPrepare(chapterNameIsDigitsOnly, start, end);
            ChapterNameDigitQuickSort(chapterNameIsDigitsOnly, start, pivot - 1);
            ChapterNameDigitQuickSort(chapterNameIsDigitsOnly, pivot + 1, end);
        }
    }
}
