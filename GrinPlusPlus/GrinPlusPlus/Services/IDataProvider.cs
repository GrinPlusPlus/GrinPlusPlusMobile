using GrinPlusPlus.Models;
using System.Collections.Generic;

namespace GrinPlusPlus.Services
{
    public interface IDataProvider
    {
        List<Transaction> GetAllData();
    }
}
