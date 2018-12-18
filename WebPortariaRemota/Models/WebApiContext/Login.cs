using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebPortariaRemota.Models.WebApiContext;

namespace WebPortariaRemota.Models.WebApiContext
{
    public class Login
    {
        private PortariaRemotaApiContext context = PortariaRemotaApiContext.Instance;

        public int LoginId { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        public async Task<HttpStatusCode> GetLogin(string userName, string userPass)
        {
            HttpResponseMessage response = await context.ClientApi.GetAsync(
                $"api/Logins?user={userName}&pass={userPass}");

            return response.StatusCode;
        }
    }
}
