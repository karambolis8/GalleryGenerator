using System.Windows;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace GalleryGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*       

        //to ma byc background worker task, wiec musi jakos aktualizowac status
        //worker dziala tak, ze task workera ma zglaszac workerowi progress i poprzez jakis argument ma anulowac prace
        //wiec zrobic tak, ze engine ma event w ktorym zglasza progress
        //i ma property ktore sam sprawdza i ktore mozna ustawic na cancel czy cos - doczytac jakie zdazenia moga sie dziac
        //i w ten sposob bedzie loose coupled a w aplikacji okienkowej bedzie mozna to opakowac i podlaczyc

         * 
         * po skonczeniu upublicznic aplikacje
         * 
         * trzeba tez dac mozliwosc zapisywania ustawien uzytkownika (formaty, szerokosci)
         * 
         * zrobic mozliwosc automatycznego wysylania bledow do mnie i formularz do wyslania maila z uwagami i zalacznikiem (np screenshot)
         * 
         * brakuje zliczania progressu i statystyk
         * ma byc progressbar - kazdy plik to jedna jednostka, foldery sie nie licza, czyli najpierw odpalam "obliczanie czasu"
         * moze byc listowanie co jest obecnie obrabiane - mozna wymyslic logi jakies  
         * 
         * do ustawień w aplikacji zrobić dymki z tłumaczeniami
         * 
         * zrobic submenu na akkordionie i z ikonka jakiegos plusika czy cos
         * 
         * zrobic tryb dogenerowywania podfolderow (z nadpisywaniem plikow lub nie) 
         * i tryb generowania calosci (wtedy bazowy plik ma inna nazwe i nie ma siblings w menu)
         * 
         * jesli kopiowac oryginalne obrazki to trzeba tez kopiowac wszystkie pliki, 
         * nawet te nie listowane, bo wtedy np htm nie beda dzialaly
         * 
         * automatyczne wypelnianie nazwy galerii z ostatniego folderu wybranej sciezki
         * 
         * zrobić jakies takie logiki zeby zamiast ifologii z roznymi opcjami wkleic w konstruktorze odpowiednie logiki
         * zeby potem juz nie ifowac. przesledzic kod, moze to ma sens i pomoze np z sciezkami relatywnymi bo teraz jest rzezba
         * 
        */
    }
}
