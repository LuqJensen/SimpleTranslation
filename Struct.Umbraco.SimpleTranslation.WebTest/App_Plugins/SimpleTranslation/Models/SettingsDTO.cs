using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Struct.Umbraco.SimpleTranslation.Models
{
    public class SettingsDTO
    {
        public int UserId { get; set; }
        public List<int> AddLanguages { get; set; }
        public List<int> RemoveLanguages { get; set; }
    }
}