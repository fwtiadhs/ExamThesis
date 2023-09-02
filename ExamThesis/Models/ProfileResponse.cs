using Newtonsoft.Json;

namespace ExamThesis.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ProfileResponse
    {
        public string uid { get; set; }
        public string am { get; set; }
        public string regyear { get; set; }
        public string regsem { get; set; }

        [JsonProperty("givenName;lang-el")]
        public string givenNamelangel { get; set; }

        [JsonProperty("sn;lang-el")]
        public string snlangel { get; set; }

        [JsonProperty("fathersname;lang-el")]
        public string fathersnamelangel { get; set; }
        public string eduPersonAffiliation { get; set; }
        public string eduPersonPrimaryAffiliation { get; set; }
        public string title { get; set; }

        [JsonProperty("title;lang-el")]
        public string titlelangel { get; set; }

        [JsonProperty("cn;lang-el")]
        public string cnlangel { get; set; }
        public string cn { get; set; }
        public string sn { get; set; }
        public string givenName { get; set; }
        public string fathersname { get; set; }
        public string secondarymail { get; set; }
        public string telephoneNumber { get; set; }
        public string labeledURI { get; set; }
        public string id { get; set; }
        public string mail { get; set; }
        public string sem { get; set; }
        public string pwdChangedTime { get; set; }
        public SocialMedia socialMedia { get; set; }
        public string profilePhoto { get; set; }
    }

    public class SocialMedia
    {
        public List<object> socialMediaExtra { get; set; }
    }


}
