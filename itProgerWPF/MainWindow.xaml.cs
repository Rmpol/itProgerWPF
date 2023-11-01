using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using itProgerWPF.Models;
using System.Xml.Serialization;
using System.IO;

namespace itProgerWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string API_KEY = "b05a04a2a22761b3272cb462aa3a627e";

        public MainWindow()
        {
            InitializeComponent();
            MainScreen.IsChecked = true;
            if (!File.Exists("user.xml"))
                ShowAuthWindow();

         
            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Open))
            {
                AuthUser auth = (AuthUser)xml.Deserialize(file);
                UserNameLabel.Content = auth.Login;
            }
        }

        private void ShowAuthWindow()
        {
            Hide();
            AuthWindow window = new AuthWindow();
            window.Show();
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void GetWeatherBtn_Click(object sender, RoutedEventArgs e)
        {
            string city = UserCityTextBox.Text.Trim();
            if (city.Length < 2)
            {
                MessageBox.Show("Укажите верный город");
                return;
            }
            
            try
            {
                string data = await GetWeather(city);
                var json = JObject.Parse(data);
                string temp = json["main"]["temp"].ToString();
                WeatherResult.Content = $"Температура в городе {city}: {temp} градусов";
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Укажите верный город");
                WeatherResult.Content = "";
            }
        }

        private async Task<string> GetWeather(string city)
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";
            return await client.GetStringAsync(url);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string objName = ((RadioButton)sender).Name;
            StackPanel[] panels = { MainScreenPanel, CabinetScreenPanel, NotesScreenPanel };
            foreach (var panel in panels)
                panel.Visibility = Visibility.Hidden;
            switch (objName)
            {
                case "CabinetScreen": CabinetScreenPanel.Visibility = Visibility.Visible;break;
                case "MainScreen": MainScreenPanel.Visibility = Visibility.Visible; break;
                case "NotesScreen": NotesScreenPanel.Visibility = Visibility.Visible; break;
            }
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("user.xml");
                ShowAuthWindow();
        }

        private void UserChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLogin.Text.Trim();
            string email = UserEmail.Text.Trim();
            if (login.Equals("") || !email.Contains("@"))
            {
                MessageBox.Show("Вы что-то ввели неверно");
                return;
            }

            AppDbContext db = new AppDbContext();
            int countUser = db.Users.Count(el => el.Login == login);
            if (countUser != 0 && !login.Equals(UserNameLabel.Content))
            {
                MessageBox.Show("Такой логин уже занят");
                return;
            }

            User user = db.Users.FirstOrDefault(el => el.Login == UserNameLabel.Content.ToString());
            user.Email = email;
            user.Login = login;
            db.SaveChanges();
            UserNameLabel.Content = login;
            UserChangeBtn.Content = "Готово";

            AuthUser auth = new AuthUser(login, email);
            XmlSerializer xml = new XmlSerializer(typeof(AuthUser));
            using (FileStream file = new FileStream("user.xml", FileMode.Create))
            {
                xml.Serialize(file, auth);
            }
        }
    }
}
