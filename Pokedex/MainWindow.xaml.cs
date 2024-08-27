using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Policy;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Pokedex.MainWindow.Pokemon;
using static Pokedex.MainWindow;
using System.IO;
using NAudio.Wave;
using NAudio.Vorbis;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Serialization;
using System.Globalization;
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
namespace Pokedex
{
    public class ApiService()
    {
        private static readonly HttpClient httpClient = new();
        public static async Task<string?> ObtenerDatos(string url)
        {
            try
            {
                // Realiza una solicitud GET a la API
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                // Lee el contenido de la respuesta como una cadena
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            catch (HttpRequestException e)
            {
                // Maneja errores de solicitud HTTP
                Console.WriteLine($"Error al obtener datos de la API: {e.Message}");
                return null;
            }
        }
    }
    public partial class MainWindow : Window
    {
        public class Pokemon
        {
            public class NameUrl
            {
                public string? Name { get; set; }
                public string? Url { get; set; }
            } //Clases con solo Name y Url
            public class Entrada_del_Juego
            {
                public string? Flavor_Text { get; set; }
                public NameUrl? Language { get; set; }
                public NameUrl? Version { get; set; }
            }
            public class Estadistica
            {
                public int Base_Stat { get; set; }
                public int Effort { get; set; }
                public NameUrl? Stat { get; set; }
            }
            public class Tipo
            {
                public int Slot { get; set; }
                public NameUrl? Type { get; set; }
            }
            public class Habilidad
            {
                public NameUrl Ability { get; set; }
                public bool Is_Hidden { get; set; }
                public int Slot { get; set; }
            }
            public class GeneroCientifico
            {
                public string? Genus { get; set; }
                public NameUrl? Language { get; set; } 
            }
            public class Nombres
            {
                public NameUrl Languages { get; set; }
                public string Name { get; set; }
            }
            public string? Name {  get; set; }
            public int Id {  get; set; }
            public double Height { get; set; }
            public double Weight { get; set; }
            public double Base_Happiness {  get; set; }
            public double Capture_Rate {  get; set; }
            public double Hatch_Counter { get; set; }
            public double Base_Experience { get; set; } //not implemented yet on UI
            public bool? Is_Baby { get; set; } //not implemented yet on UI
            public bool? Is_Legendary { get; set; } //not implemented yet on UI
            public bool? Is_Mythical { get; set; } //not implemented yet on UI
            public bool? Has_Gender_Differences { get; set; } //not implemented yet on UI
            public NameUrl? Evolves_From_Species { get; set; } //not implemented yet on UI
            public NameUrl? Generation { get; set; } //not implemented yet on UI
            public NameUrl? Habitat { get; set; } //not implemented yet on UI
            public NameUrl? Evolution_Chain { get; set; } //not implemented yet on UI  AND only has URL but not a NAME
            public NameUrl? Color { get; set; }
            public NameUrl? Shape { get; set; }
            public NameUrl? Growth_Rate { get; set; }
            public List<Entrada_del_Juego>? Flavor_Text_Entries { get; set; }
            public List<Estadistica>? Stats { get; set; }
            public List<Tipo>? Types { get; set; }
            public List<Habilidad>? Abilities { get; set; }
            public List<NameUrl>? Egg_Groups { get; set; }
            public List<GeneroCientifico>? Genera { get; set; } //not implemented yet on UI
            public List<Nombres>? Names { get; set; } //not implemented yet on UI
        }
        private int PokemonActual { get; set; } = 1;
        private int PaginaActual { get; set; } = 1;
        private string GritoActual { get; set; }
        private WaveOutEvent _waveOut;
        private VorbisWaveReader _vorbisReader;
        private string _tempFilePath;

        public MainWindow()
        {
            InitializeComponent();
            CargarPokemon();
        }
        public static string Mayus(string input)
        {
            if (string.IsNullOrEmpty(input)) {return input;}
            return char.ToUpper(input[0], CultureInfo.CurrentCulture)+input.Substring(1).ToLower(CultureInfo.CurrentCulture);
        }
        private async void CargarPokemon()
        {
            //recuperar info parte 1
            string url = $"https://pokeapi.co/api/v2/pokemon-species/{PokemonActual}";
            string? datos = await ApiService.ObtenerDatos(url);
            if (datos == null) { return; }
            Pokemon? Actual = JsonConvert.DeserializeObject<Pokemon>(datos);
            if (Actual == null) { return; }

            //llenado mitad izquierda
            if (Actual.Name == "nidoran-f") { Actual.Name = "nidoran♀"; }
            if (Actual.Name == "nidoran-m") { Actual.Name = "nidoran♂"; }
            NombrePokemon.Text = Mayus(Actual.Name);
            NumeroPokemon.Text = $"#{Actual.Id.ToString("D4")}";
            Colores.Text = Mayus(Actual.Color.Name);
            BaseFriendship.Text = $"{Actual.Base_Happiness}pts";
            CaptureRate.Text = $"{Actual.Capture_Rate}";
            if (Actual.Shape == null) { Actual.Shape = new(){ Name = "¿?" }; } else { PokeShape.Text = Mayus(Actual.Shape.Name); }
            if (Actual.Growth_Rate == null) { Actual.Growth_Rate = new(){ Name = "¿?" }; } else { GrowthRate.Text = Mayus(Actual.Growth_Rate.Name); }
            DescripciónPokemon.Text = (Actual.Flavor_Text_Entries.LastOrDefault(entry => entry.Language.Name == "en").Flavor_Text).Replace("\n"," ");
            string sprite = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Actual.Id}.png";
            GritoActual = $"https://raw.githubusercontent.com/PokeAPI/cries/main/cries/pokemon/latest/{Actual.Id}.ogg";
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(sprite, UriKind.Absolute);
            bitmap.EndInit();
            FotoPokemon.Source = bitmap;
            EggGroups.Text = "";
            foreach (NameUrl Grupo in Actual.Egg_Groups)
            {
                if (EggGroups.Text != "") { EggGroups.Text += ", "; }
                EggGroups.Text += $"{Mayus(Grupo.Name)}";
            }
            if (EggGroups.Text == "") { EggGroups.Text = "Empty"; }
            HatchCounter.Text = $"{Actual.Hatch_Counter} cycles";

            //recuperar info parte 2
            url = $"https://pokeapi.co/api/v2/pokemon/{PokemonActual}";
            datos = await ApiService.ObtenerDatos(url);
            if (datos == null) { return; }
            Actual = JsonConvert.DeserializeObject<Pokemon>(datos);
            if (Actual == null) { return; }

            //Tipos
            Tipo1.Fill = Brushes.Transparent;
            Tipo2.Fill = Brushes.Transparent;
            Tipo1Nombre.Text = "";
            Tipo2Nombre.Text = "";
            SolidColorBrush ColorearTipo(string tipo)
            {
                if (tipo == "normal")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#75515B");
                }
                else if (tipo == "fighting")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#994025");
                }
                else if (tipo == "flying")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#4A677D");
                }
                else if (tipo == "poison")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#5E2D88");
                }
                else if (tipo == "ground")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#A9702C");
                }
                else if (tipo == "rock")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#48180B");
                }
                else if (tipo == "bug")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#1C4B27");
                }
                else if (tipo == "ghost")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#33336B");
                }
                else if (tipo == "steel")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#5F756D");
                }
                else if (tipo == "fire")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#AB1F23");
                }
                else if (tipo == "water")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#1552E2");
                }
                else if (tipo == "grass")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#147B3D");
                }
                else if (tipo == "electric")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#E3E429");
                }
                else if (tipo == "psychic")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#A42A6C");
                }
                else if (tipo == "ice")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#86D2F5");
                }
                else if (tipo == "dragon")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#448B95");
                }
                else if (tipo == "dark")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#040706");
                }
                else if (tipo == "fairy")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#971944");
                }
                else if (tipo == "stellar")
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFrom("#D9EFFB");
                }
                else if (tipo == "unknown")
                {
                    return Brushes.Lime;
                }
                else
                {
                    return Brushes.Transparent;
                }
            }
            foreach (var entry in Actual.Types)
            {
                switch (entry.Slot)
                {
                    case 1:
                        Tipo1Nombre.Text = Mayus(entry.Type.Name);
                        if (entry.Type.Name == "electric") { Tipo1Nombre.Foreground = Brushes.Black; } else { Tipo1Nombre.Foreground = Brushes.White; }
                        Tipo1.Fill = ColorearTipo(entry.Type.Name);
                        break;
                    case 2:
                        Tipo2Nombre.Text = Mayus(entry.Type.Name);
                        if (entry.Type.Name == "electric") { Tipo2Nombre.Foreground = Brushes.Black; } else { Tipo2Nombre.Foreground = Brushes.White; }
                        Tipo2.Fill = ColorearTipo(entry.Type.Name);
                        break;
                    default:
                        break;
                }
            }

            //Stats
            Stats.HP = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "hp").Base_Stat;
            Stats.Speed = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "speed").Base_Stat;
            Stats.SpecialDefense = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "special-defense").Base_Stat;
            Stats.Defense = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "defense").Base_Stat;
            Stats.SpecialAttack = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "special-attack").Base_Stat;
            Stats.Attack = Actual.Stats.FirstOrDefault(entry => entry.Stat.Name == "attack").Base_Stat;

            PokeHeight.Text = $"{Actual.Height/10}m";
            PokeWeight.Text = $"{Actual.Weight/10}kg";


            Ability.Text = "";
            HiddenAbility.Text = "";
            foreach (Habilidad abilidad in Actual.Abilities)
            {
                if (abilidad.Is_Hidden == false)
                {
                    if (Ability.Text != "") { Ability.Text += " / "; }
                    Ability.Text += $"{Mayus(abilidad.Ability.Name)}";
                }
                else
                {
                    HiddenAbility.Text = Mayus(Actual.Abilities.FirstOrDefault(entry => entry.Is_Hidden == true).Ability.Name);
                }
            }
            if (HiddenAbility.Text == "") { HiddenAbility.Text = "None"; }

        }
        private void MoverVentana(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Apagado(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void SiguientePokemon(object sender, MouseButtonEventArgs e)
        {
            if (Aumento.Text == "") { Aumento.Text = "1"; }
            if ((PokemonActual + int.Parse(Aumento.Text)) <= 1025)
            {
                PokemonActual += int.Parse(Aumento.Text);
                CargarPokemon();
            }
            else
            {
                PokemonActual = 1025;
                Aumento.Text = "1";
                CargarPokemon();
            }
        }
        private void AnteriorPokemon(object sender, MouseButtonEventArgs e)
        {
            if (Aumento.Text == "") { Aumento.Text = "1"; }
            if ((PokemonActual-int.Parse(Aumento.Text)) >= 1)
            {
                PokemonActual -= int.Parse(Aumento.Text);
                CargarPokemon();
            }
            else
            {
                PokemonActual = 1;
                Aumento.Text = "1";
                CargarPokemon();
            }
        }
        private async void PlayAudio(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Descargar y reproducir el archivo
                _tempFilePath = await DownloadFileAsync(GritoActual);

                PlaySound(_tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private static async Task<string> DownloadFileAsync(string url)
        {
            using HttpClient client = new();
            byte[] fileBytes = await client.GetByteArrayAsync(url);
            string tempFilePath = System.IO.Path.GetTempFileName() + ".ogg";
            await File.WriteAllBytesAsync(tempFilePath, fileBytes);
            return tempFilePath;
        }
        private void PlaySound(string filePath)
        {
            if (_waveOut != null)
            {
                _waveOut.Stop();
                _waveOut.Dispose();
            }

            _vorbisReader = new VorbisWaveReader(filePath);
            _waveOut = new WaveOutEvent();
            _waveOut.Init(_vorbisReader);
            _waveOut.Volume = 0.1f;
            _waveOut.Play();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Elimina el archivo temporal al cerrar la aplicación
            try
            {
                if (File.Exists(_tempFilePath))
                {
                    File.Delete(_tempFilePath);
                }
            }
            catch
            {
                Console.WriteLine("Error al borrar archivos temporales");
            }
            // Libera los recursos de NAudio
            _waveOut?.Dispose();
            _vorbisReader?.Dispose();
        }
    }
}
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.