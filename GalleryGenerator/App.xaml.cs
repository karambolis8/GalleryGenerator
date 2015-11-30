using System.Windows;

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
         * podczas generowania zbierac statystyki zeby potem wyswietlnic podsumowanie 
         * i moze np liste zignorowanych formatow, liste plikow ktore rzucily bledem
         * 
         * do ustawień w aplikacji zrobić dymki z tłumaczeniami
         * 
         * zrobic submenu na akkordionie i z ikonka jakiegos plusika czy cos
         * 
         * zrobic tryb dogenerowywania podfolderow (z nadpisywaniem plikow lub nie) 
         * i tryb generowania calosci (wtedy bazowy plik ma inna nazwe i nie ma siblings w menu)
         *
         * chyba nie bedzie dzialalo jak bedzie sie przenosilo galerie - linki sa za mocne
         * 
         * foldery obrazkow nie konfigurowane - tylko z xmla
         * usunac this.options.MediumImgDir, this.options.ThumbImgDir i przeniesc do Configuration
         * usunac tez pozostale rzeczy wejsciowe zahardkodowane przynajmniej tymczasowo bez wpisow w Configuration 
         * zebym mogl se uruchamiac na drugim kompie
         * 
         * css, js, ico trzeba robic relative ../../ <- obliczac to z nesting na podstawie ilosci path.separatorow
         * 
         * jesli kopiowac oryginalne obrazki to trzeba tez kopiowac wszystkie pliki, nawet te nie listowane, bo wtedy np htm nie beda dzialaly
         * 
        */
    }
}
