using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        // Размер массива элементов
        const int C = 30;
        const int L = 30;
        /*
         * Lines - высота поля
         * Columns - ширина поля
         * totalFlags - всего флагов можно поставить (кол-во бомб)
         * totalBombs - общее кол-во бомб
         */
        int Lines, Columns, totalFlags, totalBombs;

        bool timerRun = false; // условие запуска таймера
        bool firstClick = true; // проверка на первое нажатие в игре

        // Массив клеток
        Field[,] btns = new Field[L, C];

        // Цвета цифр клеток
        SolidColorBrush[] digitColors = new SolidColorBrush[8] {
            Brushes.Blue
            , Brushes.Green
            , Brushes.Red
            , Brushes.Navy
            , Brushes.Maroon
            , Brushes.Teal
            , Brushes.Black
            , Brushes.Gray };

        // Картинки для игры
        string[] icons = new string[] { "💣", "🙅‍", "🚩" };

        /// <summary>
        /// Заполнить поле
        /// </summary>
        /// <param name="Lines">Кол-во строк</param>
        /// <param name="Columns">Кол-во столбцов</param>
        void FillPlayground(int Lines, int Columns)
        {
            // Очистить поле
            playground.Children?.Clear();

            // Создать новый элемент
            Field field;
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    field = new Field();

                    // Установить позицию, события клика и тэн
                    field.Margin = new Thickness(j * 25 + j, i * 25 + i, 0, 0);
                    field.MouseLeftButtonDown += LeftClick;
                    field.MouseRightButtonDown += RightClick;
                    field.Tag = i + "-" + j;

                    // Добавить элемент в массив
                    btns[i, j] = field;

                    // Добавить элемент на поле
                    playground.Children.Add(field);
                }
            }

        }

        /// <summary>
        /// Установить флаг
        /// </summary>
        /// <param name="field">Ячейка, в которую устанавливается флаг</param>
        /// <param name="flagsQuantity">Общее кол-во флагов</param>
        private void PutFlag(Field field, ref int flagsQuantity)
        {
            flagsQuantity--;
            field.isFlag = true;
            field.Text = icons[2];

            field.MouseLeftButtonDown -= LeftClick;
        }

        /// <summary>
        /// Удалить флаг
        /// </summary>
        /// <param name="field">Ячейка из которой удаляется флаг</param>
        /// <param name="flagsQuantity">Общее кол-во флагов</param>
        private void RemoveFlag(Field field, ref int flagsQuantity)
        {
            flagsQuantity++;
            field.isFlag = false;
            field.Text = "";

            field.MouseLeftButtonDown += LeftClick;
        }

        /// <summary>
        /// Начать новую игру
        /// </summary>
        private void StartNewGame()
        {
            // Установка размера поля
            if (rbBeginner.IsChecked == true) // Сложность Beginner
            {
                Lines = 9; Columns = 9; totalBombs = 10;
            }
            else if (rbNormal.IsChecked == true) // Сложность Normal
            {
                Lines = 16; Columns = 16; totalBombs = 40;
            }
            else if (rbExpert.IsChecked == true) // Сложность Expert
            {
                Lines = 16; Columns = 30; totalBombs = 99;
            }
            else // Сложность, которую натсроил игрок
            {
                if (int.TryParse(tbxMyHeight.Text, out Lines))
                {                    
                    if (Lines < 2) Lines = 2;
                    else if (Lines > 30) Lines = 30;
                }
                else Lines = 20;

                if (int.TryParse(tbxMyWidth.Text, out Columns))
                {
                    if (Columns < 2) Columns = 2;
                    else if (Columns > 30) Columns = 30;
                }
                else Columns = 30;

                if (int.TryParse(tbxMyBombs.Text, out totalBombs))
                {
                    if (totalBombs < 1) totalBombs = 1;
                    else if (totalBombs > Lines * Columns - 1) totalBombs = Lines * Columns - 1;
                }
                else totalBombs = 120;
            }

            tbxMyHeight.Text = Lines.ToString();
            tbxMyWidth.Text = Columns.ToString();
            tbxMyBombs.Text = totalBombs.ToString();

            totalFlags = totalBombs;

            FillPlayground(Lines, Columns);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            flagNum.Text = $"{totalFlags:000}";
            gameTimer.Text = "000";

            timerRun = false;
            firstClick = true;

            StartNewGame();

            // Убрать возможность нажать на кнопку на 700мс, чтобы никто ничего не сломал
            btnStart.IsEnabled = false;
            btnNewGame.IsEnabled = false;
            BtnStop();
        }
        
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void LeftClick(object sender, MouseButtonEventArgs e)
        {
            // Преобразуем тип object в Field
            Field field = ((Field)sender);

            // По правилам, первое нажатие всегда верное
            // При первом нажатии всем элементам устанавливается кол-во бомб вокруг 
            // и где находятся бомбы
            if (firstClick)
            {
                // Запустить таймер заново
                timerRun = true;
                StartTimer();

                firstClick = false; // Следующие нажатия зависят только от игрока

                // Проверка размера поля, чтобы правильно пулучить диапозон случайных элементов
                int modi = Lines.ToString().Length == 1 ? 10 : 100;
                int modj = Columns.ToString().Length == 1 ? 10 : 100;

                while (totalBombs > 0) // Бомбы создаются, пока счётчик не обнулится
                {
                    // Получить случайный индекс элемента, который будет бомбой
                    int i = Math.Abs(Guid.NewGuid().GetHashCode() % modi);
                    int j = Math.Abs(Guid.NewGuid().GetHashCode() % modj);

                    // Проверка на корректность полученного индекса
                    if (i < Lines && j < Columns // Индексы входят в границы созданных элементов
                        && !btns[i, j].isBomb // Полученный по индексам элемент не является бомбой
                        && btns[i, j].Tag != field.Tag) // Нажатый элемент не может стать бомбой
                    {
                        // Установка свойства бомбы в true
                        btns[i, j].isBomb = true;
                        totalBombs--; // Обновление счётчика бомб

                        // Обновляем счётчик бомб соседних элементов------------------------
                        // Элементы сверху бомбы
                        if (i > 0)
                            for (int c = j - 1; c <= j + 1; c++)
                            {
                                if (c >= 0 && c < Columns && !btns[i - 1, c].isBomb)
                                    btns[i - 1, c].BombsNum++;
                            }

                        // Элементы снизу бомбы
                        if (i < Lines - 1)
                            for (int c = j - 1; c <= j + 1; c++)
                            {
                                if (c >= 0 && c < Columns && !btns[i + 1, c].isBomb)
                                    btns[i + 1, c].BombsNum++;
                            }

                        // Элементы по бокам бомбы
                        if (j - 1 >= 0 && !btns[i, j - 1].isBomb)
                            btns[i, j - 1].BombsNum++;
                        if (j + 1 < Columns && !btns[i, j + 1].isBomb)
                            btns[i, j + 1].BombsNum++;
                        // -----------------------------------------------------------------

                    }
                }
            }

            // Игрок нажал на клетку, которая не является бомбой
            if (!field.isBomb)
            {
                /*
                 --Берём клетку, смотрим чему равно BombsNum(число бомб вокруг).
                 --Если BombsNum == 0(бомб нет), то саму клетку удаляем из стека и записываем в список проверенных клеток,чтобы не добавлять в стек заново.
                 Записываем в стек все клетки вокруг выбранной и делаем с ними ту же проверку.
                 --Если BombsNum != 0(есть хот бы 1 бомба), то выводим в Text значение BombsNum, удаляем клетку из стека, записываем в список проверенных и идём дальше.
                 --Такой цикл происходит до тех пор, пока стек не опустеет (в итоге откроются все цифры вокруг выбранной клетки)
                 */
                Stack<Field> fieldsForCheck = new Stack<Field>(); // Стек найденных клеток
                List<Field> checkedFields = new List<Field>(); // Список проверенных клеток

                // Добавить выбранную клетку в стек
                fieldsForCheck.Push(field);
                // Цикл открытия клеток
                while (fieldsForCheck.Count > 0)
                {
                    Field lastFild = fieldsForCheck.Pop();

                    // Тег хранит позицию клетки в массиве
                    var ij = lastFild.Tag.ToString().Split('-');
                    int i = Convert.ToInt32(ij[0]);
                    int j = Convert.ToInt32(ij[1]);

                    // Проверка, установлен ли флаг на клетку
                    if (!lastFild.isFlag)
                    {
                        // Проверка кол-ва бомб округ
                        // Если бомб нет - добавляем соседние жлементы в стек
                        if (lastFild.BombsNum == 0)
                        {
                            lastFild.Background = Brushes.SlateGray;

                            // Элементы сверху
                            if (i > 0)
                                for (int c = j - 1; c <= j + 1; c++)
                                {
                                    if (c >= 0 && c < Columns && !btns[i - 1, c].isBomb && !checkedFields.Contains(btns[i - 1, c]))
                                        fieldsForCheck.Push(btns[i - 1, c]);
                                }

                            // Элементы сеизу
                            if (i < Lines - 1)
                                for (int c = j - 1; c <= j + 1; c++)
                                {
                                    if (c >= 0 && c < Columns && !btns[i + 1, c].isBomb && !checkedFields.Contains(btns[i + 1, c]))
                                        fieldsForCheck.Push(btns[i + 1, c]);
                                }

                            // Элементы по бокам
                            if (j - 1 >= 0 && !btns[i, j - 1].isBomb && !checkedFields.Contains(btns[i, j - 1]))
                                fieldsForCheck.Push(btns[i, j - 1]);
                            if (j + 1 < Columns && !btns[i, j + 1].isBomb && !checkedFields.Contains(btns[i, j + 1]))
                                fieldsForCheck.Push(btns[i, j + 1]);

                        }
                        else // Найден элемент рядом с которым есть бомба(ы)
                        {
                            lastFild.Background = Brushes.BurlyWood;
                            lastFild.Foreground = digitColors[lastFild.BombsNum-1];
                            lastFild.Text = lastFild.BombsNum.ToString();
                        }

                        // На данную клетку больше нельзя нажать  или установить флаг
                        lastFild.MouseRightButtonDown -= RightClick;
                        lastFild.MouseLeftButtonDown -= LeftClick;

                    }
                    // Добавить клетку в список проверенных
                    checkedFields.Add(lastFild);

                }
                // Очистка списка проверенных клеток после прохода цикла
                checkedFields.Clear();


                /*
                 --Проверка, открыл ли игрок все клетки
                 --Если игрок открол все клетки (неважно пометил ли он бомбы флагом или нет), то он сичтается победителем
                 */
                bool playerWin = true;
                for (int i = 0; i < Lines && playerWin; i++)
                {
                    for (int j = 0; j < Columns && playerWin; j++)
                    {
                        if (btns[i, j].isBomb) // Нашли бомбу
                        {
                            // UP
                            if (i > 0)
                                for (int c = j - 1; c <= j + 1; c++)
                                {
                                    if (c >= 0 && c < Columns && !btns[i - 1, c].isBomb && btns[i - 1, c].Text == "") playerWin = false;
                                }

                            // DOWN
                            if (i < Lines - 1)
                                for (int c = j - 1; c <= j + 1; c++)
                                {
                                    if (c >= 0 && c < Columns && !btns[i + 1, c].isBomb && btns[i + 1, c].Text == "") playerWin = false;
                                }

                            // MID
                            if (j - 1 >= 0 && !btns[i, j - 1].isBomb && btns[i, j - 1].Text == "") playerWin = false;
                            if (j + 1 < Columns && !btns[i, j + 1].isBomb && btns[i, j + 1].Text == "") playerWin = false;
                        }

                    }
                }

                // Игрок всё же выиграл
                if (playerWin)
                {
                    // Остановить таймер игры
                    timerRun = false;

                    // Поставить флаги на все бомбы без флагов
                    for (int i = 0; i < Lines; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            if (btns[i, j].isBomb && !btns[i, j].isFlag)
                            {
                                PutFlag(btns[i, j], ref totalFlags);
                                btns[i, j].MouseLeftButtonDown -= LeftClick;
                                btns[i, j].MouseRightButtonDown -= RightClick;
                            }
                        }
                    }

                    // Обновить общее число флагов
                    flagNum.Text = $"{totalFlags:000}";

                    MessageBox.Show("You WIN!");
                }

            }
            else // Игрок нажал на бомбу
            {
                // Остановить таймер
                timerRun = false;

                // Выделение бомбы, на которую нажал игрок
                field.Background = Brushes.Red;

                // Показать все бомбы
                for (int i = 0; i < Lines; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        Field item = btns[i, j];
                        item.MouseLeftButtonDown -= LeftClick;
                        item.MouseRightButtonDown -= RightClick;

                        // Установить картинку бомбы на клеточку
                        if (item.isBomb && !item.isFlag)
                        {
                            item.Text = icons[0];
                        }
                        // Установить картинку неверно выбранного места бомбы 
                        // (флаг был установлен на клетку без бомбы -> картинка неверной установки флага)
                        else if (!item.isBomb && item.isFlag)
                        {
                            item.Background = Brushes.Orange;

                            item.Text = icons[1];
                        }

                    }
                }

                MessageBox.Show("You Lose!");
            }




            playground.UpdateLayout();

        }


        /// <summary>
        /// Вставка флага в клетку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RightClick(object sender, MouseButtonEventArgs e)
        {
            Field field = ((Field)sender);
            if (field.isFlag) // Флаг установлен
            {
                RemoveFlag(field, ref totalFlags);
                flagNum.Text = $"{totalFlags:000}";
            }
            else // Флаг не установлен
            {
                PutFlag(field, ref totalFlags);
                flagNum.Text = $"{totalFlags:000}";
            }
        }

        private void checkForDigit(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void MineCheck(object sender, RoutedEventArgs e)
        {
            rbExclusive.IsChecked = true;
        }

        private void checkForSpace(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }


        
        private async Task BtnStop()
        {
            await Task.Run(() => ButtonDisable());
        }
        private void ButtonDisable()
        {
            Thread.Sleep(700);

            this.Dispatcher.BeginInvoke((Action)delegate
                {
                    btnStart.IsEnabled = true;
                    btnNewGame.IsEnabled = true;
                });
        }



        private async Task StartTimer()
        {
            await Task.Run(() => GameTimer());
        }
        /// <summary>
        /// Таймер игры
        /// </summary>
        private void GameTimer()
        {
            while (timerRun)
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    gameTimer.Text = $"{Convert.ToInt32(gameTimer.Text) + 1:000}";
                    if (gameTimer.Text == "999") timerRun = false;
                });
                if (!timerRun) MessageBox.Show(timerRun.ToString());
                Thread.Sleep(1000);
            }

        }
    }
}
