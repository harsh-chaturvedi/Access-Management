using System.Collections.Generic;
using System.Linq;

namespace Access_Management.Client.Model
{
    public class ResourceAuthorizationResponse
    {
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }

        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}