using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewsApi
{
    public interface ITheNewsApiService
    {
        Task FetchAndSaveNewsAsync();

    }
}
