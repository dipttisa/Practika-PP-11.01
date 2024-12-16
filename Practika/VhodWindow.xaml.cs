using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Practika
{
    public partial class VhodWindow : Window
    {
        private int failedAttempts = 0; // Счётчик попыток
        private string captchaText = ""; // Текущая CAPTCHA
        private DispatcherTimer lockTimer = new DispatcherTimer();
        private string connectionString;

        public VhodWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["PractikaDB"].ConnectionString;
            MessageBox.Show($"Строка подключения: {connectionString}");
            LoadUserLogins();
            lockTimer.Interval = TimeSpan.FromSeconds(30);
            lockTimer.Tick += LockTimer_Tick;
        }

        // Загрузка логинов из БД
        private void LoadUserLogins()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT login FROM users";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoginComboBox.Items.Add(reader["login"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки логинов: {ex.Message}");
            }
        }


        // Генерация CAPTCHA
        private string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder captcha = new StringBuilder();
            for (int i = 0; i < 10; i++)
                captcha.Append(chars[random.Next(chars.Length)]);

            return captcha.ToString();
        }

        private void RefreshCaptcha()
        {
            captchaText = GenerateCaptcha();
            CaptchaLabel.Content = $"CAPTCHA: {captchaText}";
        }

       /* // Проверка пароля и логина
        private bool AuthenticateUser(string login, string password)
        {
            try
            {
                string hashedPassword = ComputeSha256Hash(password);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM users WHERE login = @login AND password = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        return (int)command.ExecuteScalar() == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }*/


        // Проверка пароля и логина (без хэширования)
        private bool AuthenticateUser(string login, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM users WHERE login = @login AND password = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password); // Передаём пароль в чистом виде

                        return (int)command.ExecuteScalar() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}");
                return false;
            }
        }


        /* // Хеширование пароля
         private string ComputeSha256Hash(string rawData)
         {
             using (SHA256 sha256Hash = SHA256.Create())
             {
                 byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                 StringBuilder builder = new StringBuilder();
                 foreach (byte b in bytes)
                     builder.Append(b.ToString("x2"));

                 return builder.ToString();
             }
         }*/

        /* // Вход
         private void LoginButton_Click(object sender, RoutedEventArgs e)
         {
             string login = LoginComboBox.Text;
             string password = PasswordBox.Password;

             if (failedAttempts >= 1 && CaptchaTextBox.Text != captchaText)
             {
                 MessageBox.Show("CAPTCHA введена неверно.");
                 RefreshCaptcha();
                 return;
             }

             if (AuthenticateUser(login, password))
             {
                 MessageBox.Show("Вход успешен!");
                 this.Close();
             }
             else
             {
                 failedAttempts++;
                 if (failedAttempts == 1)
                 {
                     CaptchaLabel.Visibility = Visibility.Visible;
                     CaptchaTextBox.Visibility = Visibility.Visible;
                     RefreshCaptchaButton.Visibility = Visibility.Visible;
                     RefreshCaptcha();
                 }

                 if (failedAttempts >= 3)
                 {
                     LockWindow();
                 }
                 else
                 {
                     MessageBox.Show($"Неверный логин или пароль. Осталось попыток: {3 - failedAttempts}");
                 }
             }
         }*/


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginComboBox.Text;
            string password = PasswordBox.Password;

            if (failedAttempts >= 1 && CaptchaTextBox.Text != captchaText)
            {
                MessageBox.Show("CAPTCHA введена неверно.");
                RefreshCaptcha();
                return;
            }

            // Проверка логина и пароля без хэширования
            if (AuthenticateUser(login, password))
            {
                MessageBox.Show("Вход успешен!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Закрытие текущего окна
                this.Close();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts == 1)
                {
                    CaptchaLabel.Visibility = Visibility.Visible;
                    CaptchaTextBox.Visibility = Visibility.Visible;
                    RefreshCaptchaButton.Visibility = Visibility.Visible;
                    RefreshCaptcha();
                }

                if (failedAttempts >= 3)
                {
                    LockWindow();
                }
                else
                {
                    MessageBox.Show($"Неверный логин или пароль. Осталось попыток: {3 - failedAttempts}");
                }
            }
        }



        // Блокировка окна
        private void LockWindow()
        {
            MessageBox.Show("Слишком много неудачных попыток. Окно заблокировано на 30 секунд.");
            LoginButton.IsEnabled = false;
            lockTimer.Start();
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            lockTimer.Stop();
            failedAttempts = 0;
            LoginButton.IsEnabled = true;
            MessageBox.Show("Блокировка снята. Попробуйте снова.");
        }

        private void RefreshCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshCaptcha();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
