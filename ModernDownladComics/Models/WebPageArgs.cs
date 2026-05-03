using ComicsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDownladComics.Models
{
    public record WebPageArgs(string Url, RetrieveSource RetrieveSource, 
        TaskCompletionSource<string> Source);
}
