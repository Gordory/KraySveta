namespace KraySveta.External.ThatsMyBis
{
    public class ThatsMyBisConfig
    {
        public const string ConfigName = "ThatsMyBis";

        public string XSRF { get; set; }
        
        public string Session { get; set; }
        
        public string CSRF { get; set; }
    }
}