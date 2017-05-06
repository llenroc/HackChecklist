using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.HackChecklist.UWP.Services
{
    public class LocalDataService
    {
        public const string FirstLaunchProperty = "firstlaunch";
        public const string FailedProperty = "checklistfailed";
        public const string PassedProperty = "checlistpassed";

        private static ApplicationDataContainer localSettings => ApplicationData.Current.LocalSettings;

        public static bool IsSet(string property)
        {
            return localSettings.Values.TryGetValue(property, out var value)
                && value is bool b
                && b;
        }

        public static void Set(string property)
        {
            localSettings.Values[property] = true;
        }

        public static void Clear(string property)
        {
            localSettings.Values.Remove(property);
        }
    }
}
