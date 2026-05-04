using ComicsLib.Models;
using ComicsServiceLib;

namespace ComicsInfraLib.Services
{
    public class HostSelectionService : IHostService
    {
        public string SelectHost(IEnumerable<Comic> comics, IEnumerable<string> hosts)
        {
            return hosts.ToDictionary(host=>host, host=>comics.Count(c=>c.Host==host))
                .MinBy(v=>v.Value).Key;
        }
    }
}
