namespace Access_Management.Client.Model
{
    public class ResourceAuthorizationRequest
    {
        public string ApplicationDomain { get; set; }

        public AccessSource AccessSource { get; set; }
        public string Route { get; set; }
    }
}