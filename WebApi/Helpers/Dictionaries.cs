using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApi.Helpers
{
    
    static class Dictionaries
    {
        //Interviu Status
        static Dictionary<string, string> _status = new Dictionary<string, string>
        {
            {"NotStarted", "Nėra Pradėtas"},
            {"Completed", "Atliktas"},
            {"Expired", "Nebuvo Atliktas"}
        };

        /// <summary>
        /// Access the Dictionary from external sources
        /// </summary>
        public static string GetStatus(string word)
        {
            // Try to get the result in the static Dictionary
            string result;
            if (_status.TryGetValue(word, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}