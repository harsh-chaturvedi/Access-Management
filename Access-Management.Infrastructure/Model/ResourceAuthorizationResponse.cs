using System.Collections.Generic;
using System.Linq;

namespace Access_Management.Infrastructure.Model
{
    public class ResourceAuthorizationResponse
    {
        public static ResourceAuthorizationResponse Success
        {
            get
            {
                return new ResourceAuthorizationResponse { Succeeded = true };
            }
        }
        public bool Succeeded { get; protected set; }
        public IEnumerable<IdentityError> Errors { get; private set; }

        public static ResourceAuthorizationResponse Failed(params IdentityError[] errors)
        {
            return new ResourceAuthorizationResponse()
            {
                Succeeded = false,
                Errors = errors
            };
        }
        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}