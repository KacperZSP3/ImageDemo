// ============================================================================
// LEKCJA: Image control i źródła obrazów
// Plik: MainPage.xaml.cs — code-behind głównej strony
// ============================================================================
//
// Ten plik zawiera logikę C# obsługującą interaktywne elementy strony.
// Każda metoda jest opatrzona komentarzem wskazującym odpowiednią sekcję
// w notatce "Image_control".
//
// SPIS TREŚCI (odniesienia do lekcji):
//   - OnZmienAspectClicked()    → Sekcja 4: Skalowanie obrazów (Aspect)
//   - OnToggleAnimationClicked() → Sekcja 6: Animowane GIF-y 
//   - OnImageButtonClicked()     → Sekcja 7: ImageButton
//   - OnLoadFromStreamClicked()  → Sekcja 3.4: Ładowanie ze strumienia
//   - CreateImageFromCode()      → Sekcja 3.1-3.3: ImageSource i metody
//
// ============================================================================
namespace ImageDemo
{
    public partial class MainPage : ContentPage
    {
        // ========================================================================
        // POLA KLASY
        // ========================================================================

        /// <summary>
        /// Tablica trybów skalowania do cyklicznego przełączania.
        /// >>>Sekcja 4.1: Tryby skalowania
        /// Dostępne tryby: AspectFit, AspectFill, Fill, Center
        /// </summary>
        private readonly Aspect[] _trybySkalowania =
        {
        Aspect.AspectFit,   // Cały obraz widoczny, zachowane proporcje
        Aspect.AspectFill,  // Wypełnia obszar, przycina nadmiar
        Aspect.Fill,        // Rozciąga do pełnych wymiarów (zniekształca!)
        Aspect.Center       // Oryginalny rozmiar, wyśrodkowany
    };

        /// <summary>
        /// Indeks aktualnie wybranego trybu skalowania.
        /// </summary>
        private int _aktualnyIndeksTrybu = 0;

        /// <summary>
        /// Licznik kliknięć ImageButton.
        /// >>>Sekcja 7: ImageButton — obraz jako przycisk
        /// </summary>
        private int _licznikKlikniec = 0;


        // ========================================================================
        // KONSTRUKTOR
        // ========================================================================

        public MainPage()
        {
            InitializeComponent();
        }


        // ========================================================================
        // OBSŁUGA ZDARZEŃ
        // ========================================================================


        /// <summary>
        /// Zmienia tryb skalowania obrazu cyklicznie.
        /// >>>Sekcja 4: Skalowanie obrazów — właściwość Aspect
        ///
        /// Właściwość Aspect kontrolki Image przyjmuje jedną z wartości:
        ///   - AspectFit:  cały obraz mieści się w obszarze (mogą być puste paski)
        ///   - AspectFill: obraz wypełnia obszar, nadmiar jest przycinany
        ///   - Fill:       obraz rozciągany do dokładnych wymiarów (zniekształcenie!)
        ///   - Center:     obraz wyśrodkowany, oryginalny rozmiar
        ///
        /// Szczegóły:Sekcja 4.1 (Tryby skalowania)
        /// </summary>
        private void OnZmienAspectClicked(object sender, EventArgs e)
        {
            // Przejście do następnego trybu (operacja modulo zapewnia cykliczność)
            _aktualnyIndeksTrybu = (_aktualnyIndeksTrybu + 1) % _trybySkalowania.Length;

            // Ustawienie nowego trybu na kontrolce Image
            // >>> Sekcja 4.1 — Aspect jest właściwością typu BindableProperty,
            //     co oznacza, że można ją wiązać z danymi (data binding).
            ObrazDynamicznyAspect.Aspect = _trybySkalowania[_aktualnyIndeksTrybu];

            // Aktualizacja etykiety informującej o aktualnym trybie
            AktualnyAspectLabel.Text = $"Aktualny tryb: {_trybySkalowania[_aktualnyIndeksTrybu]}";
        }


        /// <summary>
        /// Włącza/wyłącza animację GIF.
        /// >>>Sekcja 6: Animowane GIF-y
        ///
        /// Właściwość IsAnimationPlaying (typ bool):
        ///   - true  = animacja jest odtwarzana
        ///   - false = animacja zatrzymana (domyślna wartość!)
        ///
        /// WAŻNE (Sekcja 6): Domyślnie GIF NIE jest animowany po załadowaniu.
        /// Trzeba jawnie ustawić IsAnimationPlaying = true.
        /// Ta właściwość nie ma efektu na obrazy inne niż GIF.
        /// </summary>
        private void OnToggleAnimationClicked(object sender, EventArgs e)
        {
            // Odwrócenie stanu animacji
            AnimowanyGif.IsAnimationPlaying = !AnimowanyGif.IsAnimationPlaying;

            // Aktualizacja interfejsu
            if (AnimowanyGif.IsAnimationPlaying)
            {
                GifStatusLabel.Text = "Animacja: włączona";
                GifToggleButton.Text = "Zatrzymaj animację";
            }
            else
            {
                GifStatusLabel.Text = "Animacja: zatrzymana";
                GifToggleButton.Text = "Uruchom animację";
            }
        }


        /// <summary>
        /// Obsługa kliknięcia na ImageButton.
        /// >>>Sekcja 7: ImageButton — obraz jako przycisk
        ///
        /// ImageButton łączy kontrolkę Image z funkcjonalnością Button.
        /// Obsługuje trzy zdarzenia:
        ///   - Clicked:  gdy użytkownik puści palec/kursor nad kontrolką
        ///   - Pressed:  gdy użytkownik naciśnie palcem/kursorem
        ///   - Released: gdy użytkownik puści palec/kursor
        ///
        /// ImageButton ma też dodatkowe właściwości wizualne:
        ///   - BorderColor, BorderWidth — obramowanie
        ///   - CornerRadius — zaokrąglenie rogów
        ///   - Padding — wewnętrzny margines
        ///
        /// Argument 'sender' to ImageButton, który wywołał zdarzenie.
        /// Dzięki temu jeden handler może obsługiwać wiele przycisków
        /// (Sekcja 7 w notatce).
        /// </summary>
        private void OnImageButtonClicked(object sender, EventArgs e)
        {
            _licznikKlikniec++;
            ImageButtonCounterLabel.Text = $"Kliknięcia ImageButton: {_licznikKlikniec}";
        }


        /// <summary>
        /// Ładuje obraz ze strumienia danych (HTTP).
        /// >>>Sekcja 3.4: Ładowanie obrazu ze strumienia
        ///
        /// ImageSource.FromStream() przyjmuje funkcję zwracającą Stream.
        /// Przydatne, gdy obraz pochodzi z:
        ///   - bazy danych (jako tablica bajtów)
        ///   - API zwracającego dane binarne
        ///   - dowolnego źródła dostarczającego strumień
        ///
        /// UWAGA (Sekcja 3.4): Na Androidzie cache jest WYŁĄCZONY
        /// przy ładowaniu ze strumienia — brak danych do klucza cache.
        ///
        /// Porównanie metod ładowania (Sekcja 3):
        ///   - FromFile()   → plik lokalny (Sekcja 3.2)
        ///   - FromUri()    → adres URL (Sekcja 3.3)
        ///   - FromStream() → strumień danych (Sekcja 3.4)
        /// </summary>
        private async void OnLoadFromStreamClicked(object sender, EventArgs e)
        {
            try
            {
                // Pobieramy obraz jako strumień z internetu
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync("https://aka.ms/campus.jpg");

                // Kopiujemy do MemoryStream, bo oryginalny stream HTTP
                // może zostać zamknięty po zakończeniu żądania
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // >>> Sekcja 3.4 — ImageSource.FromStream()
                // Metoda przyjmuje Func<Stream> (funkcję zwracającą strumień).
                // To pozwala na leniwe ładowanie — strumień jest tworzony
                // dopiero gdy kontrolka go potrzebuje.
                ObrazZeStrumienia.Source = ImageSource.FromStream(() => memoryStream);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd",
                    $"Nie udało się załadować obrazu ze strumienia: {ex.Message}",
                    "OK");
            }
        }


        // ========================================================================
        // METODY POMOCNICZE — TWORZENIE OBRAZÓW W C#
        // ========================================================================

        /// <summary>
        /// Przykłady tworzenia kontrolki Image wyłącznie w C# (bez XAML).
        /// >>>Sekcje 3.1–3.3
        ///
        /// Ta metoda NIE jest wywoływana w aplikacji — służy wyłącznie
        /// jako materiał referencyjny pokazujący różne sposoby tworzenia
        /// obrazów w kodzie C#.
        /// </summary>
        private void CreateImageFromCode_Przyklady()
        {
            // ----------------------------------------------------------------
            // >>> Sekcja 3.2: Obraz lokalny (FileImageSource)
            // ----------------------------------------------------------------

            // Sposób 1: Jawne użycie ImageSource.FromFile()
            // Zwraca obiekt FileImageSource
            var obraz1 = new Image
            {
                Source = ImageSource.FromFile("dotnet_bot.png")
            };

            // Sposób 2: Niejawna konwersja z string na ImageSource
            // .NET MAUI automatycznie rozpoznaje, że to plik lokalny
            var obraz2 = new Image
            {
                Source = "dotnet_bot.png"
            };


            // ----------------------------------------------------------------
            // >>> Sekcja 3.3: Obraz zdalny (UriImageSource)
            // ----------------------------------------------------------------

            // Sposób 1: Jawne użycie ImageSource.FromUri()
            // Wymaga obiektu Uri
            var obraz3 = new Image
            {
                Source = ImageSource.FromUri(new Uri("https://aka.ms/campus.jpg"))
            };

            // Sposób 2: Niejawna konwersja z string (z protokołem https://)
            var obraz4 = new Image
            {
                Source = "https://aka.ms/campus.jpg"
            };


            // ----------------------------------------------------------------
            // >>> Sekcja 5: Cache'owanie obrazów zdalnych
            // ----------------------------------------------------------------

            // Jawna konfiguracja UriImageSource z właściwościami cache
            var obraz5 = new Image();
            obraz5.Source = new UriImageSource
            {
                Uri = new Uri("https://aka.ms/campus.jpg"),

                // CacheValidity — jak długo obraz jest w cache
                // Format: new TimeSpan(dni, godziny, minuty, sekundy)
                CacheValidity = new TimeSpan(10, 0, 0, 0), // 10 dni

                // CachingEnabled — czy cache jest włączony (domyślnie true)
                CachingEnabled = true
            };


            // ----------------------------------------------------------------
            // >>> Sekcja 3.4: Obraz ze strumienia (StreamImageSource)
            // ----------------------------------------------------------------

            // ImageSource.FromStream() przyjmuje Func<Stream>
            // UWAGA: Na Androidzie cache jest wyłączony dla strumieni!
            // var obraz6 = new Image
            // {
            //     Source = ImageSource.FromStream(() => mojStrumien)
            // };


            // ----------------------------------------------------------------
            // >>> Sekcja 4.1: Ustawienie trybu skalowania
            // ----------------------------------------------------------------

            // Właściwość Aspect można ustawić na jeden z czterech trybów
            var obraz7 = new Image
            {
                Source = "dotnet_bot.png",
                Aspect = Aspect.AspectFit,      // Cały obraz, proporcje zachowane
                HeightRequest = 200,
                WidthRequest = 200
            };
            // Inne tryby: Aspect.AspectFill, Aspect.Fill, Aspect.Center


            // ----------------------------------------------------------------
            // >>> Sekcja 6: Animowany GIF
            // ----------------------------------------------------------------

            // IsAnimationPlaying musi być true, by GIF się animował
            var gifObraz = new Image
            {
                Source = "demo.gif",
                IsAnimationPlaying = true,      // Domyślnie false!
                HeightRequest = 100
            };


            // ----------------------------------------------------------------
            // >>> Sekcja 7: ImageButton w C#
            // ----------------------------------------------------------------

            var imageButton = new ImageButton
            {
                Source = "dotnet_bot.png",
                HeightRequest = 60,
                WidthRequest = 60,
                CornerRadius = 30,              // Okrągły kształt (połowa wymiaru)
                BorderColor = Colors.DarkBlue,
                BorderWidth = 2
            };

            // Podpięcie zdarzenia Clicked
            imageButton.Clicked += (s, e) =>
            {
                // Reakcja na kliknięcie
                DisplayAlert("Info", "ImageButton kliknięty!", "OK");
            };
        }
    }
}

