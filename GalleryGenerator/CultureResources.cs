using System.Globalization;
using System.Windows.Data;
using GalleryGenerator.Resources.Translations;

// http://www.codeproject.com/Articles/22967/WPF-Runtime-Localization

namespace GalleryGenerator
{
    public class CultureResources
    {
        private static ObjectDataProvider m_provider;

        public static ObjectDataProvider ResourceProvider
        {
            get { return m_provider ?? (m_provider = (ObjectDataProvider) App.Current.FindResource("Translations")); }
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            Translations.Culture = culture;
            ResourceProvider.Refresh();
        }
        public Translations GetResourceInstance()
        {
            return new Translations();
        }
    }
}