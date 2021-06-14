using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Trajectory2
{
    public partial class MainWindow : Window
    {
        // Координаты для обычной параболы
        private double x;
        private double y;

        // Константы 
        private const double G = 9.81;

        // Канвас 
        private double aW;                  // Широта
        private double aH;                  // Высота
        private double coefficientWidth;
        private double coefficientHeight;

        // Параметры для таймера 
        private DispatcherTimer timer = new DispatcherTimer();
        private double time;

        // Начальные данные
        private double speed1;
        private double angle1;

        // Метод Эйлера
        private double x1;
        private double y1;

        private double speedX1;
        private double speedY1;

        private const double DeltaTime = 0.001;
        private const double ResistanceForce = 6;
        private const int Mass = 1;



        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(OnTimer); // Создаем эвент 
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10); // Частота обновления таймера 
        }

        private void Start()
        {
            x1 = y1 = time = 0; // Чистим данные 
            poliline.Points.Clear(); // Чистим полилайн 
            Canvas1(); // Считаем коффицент маштабирования 
            speedX1 = speed1 * Math.Cos(angle1);
            speedY1 = speed1 * Math.Sin(angle1);
            timer.Start(); // Запускам таймер 
        }

        private void Input() // Ввод данных введеных пользователем 
        {
            speed1 = Convert.ToDouble(speed.Text); // Скорость 
            angle1 = Convert.ToDouble(angle.Text); // Угол  
            angle1 = angle1 * Math.PI / 180;        // Перевод угла в радиальную систему 
        }

        private void Canvas1() // Размеры канваса 
        {
            aW = canvas.ActualWidth / 2;        // Считаем середину канваса 
            aH = canvas.ActualHeight / 2;       // Считаем середину канваса 

            coefficientWidth = aW;       // Считаем коэффицент маштобирования 
            coefficientHeight = aH;      // Аналогично 
        }

        private void Button_Click(object sender, RoutedEventArgs e) // При нажатии кнопки "Старт"
        {
            Input(); // Вводим данные пользователя 
            Start();
        }

        private void OnTimer(object sender, EventArgs e) // Таймер 
        {
            // Обычная парабола т.е. без сопротивления среды 
            time += 0.01; // Шаг времени в таймере

            ellipse.Fill = new SolidColorBrush(Color.FromRgb(156, 126, 186)); // Цвет элипса 
            x = speed1 * time * Math.Cos(angle1); // Координата по "Х"
            y = speed1 * time * Math.Sin(angle1) - G * time * time / 2; // Координата по "У"

            //poliline.Points.Add(new Point(coefficientWidth * x, aH + coefficientHeight * -y)); // Заносим координаты в полиайн 

            // Метод Эйлера

            x1 = x1 + DeltaTime * speedX1;
            y1 = y1 + DeltaTime * speedY1;

            speedX1 = speedX1 - DeltaTime * (ResistanceForce * speedX1 / Mass);
            speedY1 = speedY1 - DeltaTime * (G + ResistanceForce * speedY1 / Mass);



            poliline.Points.Add(new Point(coefficientWidth * x1, aH + coefficientHeight * -y1));
            Canvas.SetLeft(ellipse, poliline.Points.Last().X - ellipse.Width / 2.0); // Меняем положение элипса по последней точки в полилайн
            Canvas.SetTop(ellipse, poliline.Points.Last().Y - ellipse.Height / 2.0); // Аналогично 

            if (y1 <= 0) // Останавливаем таймер при достижении "У" отрицательных значений 
                timer.Stop();
        }
    }
}
