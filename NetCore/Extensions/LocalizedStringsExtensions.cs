using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SmintIo.PortalsAPI.Backend.Client.Generated
{
    public static class LocalizedStringsExtensions
    {
        public const string DefaultCulture = "x-default";

        public static string ResolveLocalizedString(this LocalizedStrings localizedStrings, CultureInfo cultureInfo)
        {
            if (localizedStrings == null)
                throw new ArgumentNullException(nameof(localizedStrings));

            if (localizedStrings.Count == 0)
                return null;

            string culture = cultureInfo.TwoLetterISOLanguageName;

            var localizedString = localizedStrings
                .Where(localizedString => string.Equals(localizedString.Culture, culture))
                .Select(localizedString => localizedString.Value)
                .FirstOrDefault();

            if (localizedString != null)
                return localizedString;

            localizedString = localizedStrings
                .Where(localizedString => string.Equals(localizedString.Culture, DefaultCulture))
                .Select(localizedString => localizedString.Value)
                .FirstOrDefault();

            if (localizedString != null)
                return localizedString;

            localizedString = localizedStrings
                .Where(localizedString => string.Equals(localizedString.Culture, "en"))
                .Select(localizedString => localizedString.Value)
                .FirstOrDefault();

            if (localizedString != null)
                return localizedString;

            return localizedStrings
                .Select(localizedString => localizedString.Value)
                .First();
        }
    }
}
