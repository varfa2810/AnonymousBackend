using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ICommonUtilities
    {
        Task<List<dynamic>> GetCountries();
        Task<List<dynamic>> GetStates(int countryId);
        Task<List<dynamic>> GetCities(int stateId);
        Task<List<dynamic>> GetAllRoles();
        Task<List<dynamic>> GetAllCompanyAdminDesignation();

    }
}
